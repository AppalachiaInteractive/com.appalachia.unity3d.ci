using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Appalachia.CI.Constants;
using Appalachia.CI.Integration.Assemblies;
using Appalachia.CI.Integration.Assets;
using Appalachia.CI.Integration.Core;
using Appalachia.CI.Integration.Core.Shell;
using Appalachia.CI.Integration.Extensions;
using Appalachia.CI.Integration.FileSystem;
using Appalachia.CI.Integration.Packages;
using Appalachia.CI.Integration.Packages.NpmModel;
using Appalachia.CI.Integration.Repositories.Publishing;
using Appalachia.CI.Integration.SourceControl;
using Appalachia.Utility.Execution;
using Appalachia.Utility.Extensions;
using Unity.EditorCoroutines.Editor;
using Unity.Profiling;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Appalachia.CI.Integration.Repositories
{
    [Serializable]
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public class RepositoryMetadata : IntegrationMetadata<RepositoryMetadata>
    {
        #region Profiling And Tracing Markers

        private const string _PRF_PFX = nameof(RepositoryMetadata) + ".";

        private static readonly ProfilerMarker _PRF_InitializeInternal =
            new(_PRF_PFX + nameof(InitializeInternal));

        private static readonly ProfilerMarker _PRF_FindAllInternal = new(_PRF_PFX + nameof(FindAllInternal));

        private static readonly ProfilerMarker _PRF_FinalizeInternal =
            new(_PRF_PFX + nameof(FinalizeInternal));

        private static readonly ProfilerMarker _PRF_PackageVersion =
            new ProfilerMarker(_PRF_PFX + nameof(PackageVersion));

        private static readonly ProfilerMarker _PRF_DistributableFile =
            new ProfilerMarker(_PRF_PFX + nameof(DistributableFile));

        private static readonly ProfilerMarker _PRF_DistributableVersion =
            new ProfilerMarker(_PRF_PFX + nameof(DistributableVersion));

        private static readonly ProfilerMarker _PRF_PublishedVersion =
            new ProfilerMarker(_PRF_PFX + nameof(PublishedVersion));

        private static readonly ProfilerMarker
            _PRF_RepoName = new ProfilerMarker(_PRF_PFX + nameof(RepoName));

        private static readonly ProfilerMarker _PRF_AssetsDirectory =
            new ProfilerMarker(_PRF_PFX + nameof(AssetsDirectory));

        private static readonly ProfilerMarker
            _PRF_RealPath = new ProfilerMarker(_PRF_PFX + nameof(RealPath));

        private static readonly ProfilerMarker _PRF_PopulateDependencies =
            new ProfilerMarker(_PRF_PFX + nameof(PopulateDependencies));

        private static readonly ProfilerMarker _PRF_SavePackageJson =
            new ProfilerMarker(_PRF_PFX + nameof(SavePackageJson));

        private static readonly ProfilerMarker _PRF_PopulateDependency =
            new ProfilerMarker(_PRF_PFX + nameof(PopulateDependency));

        private static readonly ProfilerMarker _PRF_SetPackageVersion =
            new ProfilerMarker(_PRF_PFX + nameof(SetPackageVersion));

        #endregion

        public const long TARGET_FILE_MAX_SIZE = (long) 1024 * 1024 * 8;

        public const long TARGET_MAX_SIZE = (long) 1024 * 1024 * 50;

        static RepositoryMetadata()
        {
            IntegrationMetadataRegistry<RepositoryMetadata>.Register(
                10,
                FindAllInternal,
                ProcessAll,
                FinalizeInternal
            );
        }

        private RepositoryMetadata()
        {
            dependencies = new HashSet<RepositoryDependency>();
            missingDependencies = new HashSet<RepositoryDependency>();
            assemblies = new List<AssemblyDefinitionMetadata>();
        }

        public HashSet<RepositoryMetadata> dependents;
        public HashSet<RepositoryDependency> dependencies;
        public HashSet<RepositoryDependency> missingDependencies;

        public List<AssemblyDefinitionMetadata> assemblies;

        public NpmPackage npmPackage;

        private AppaDirectoryInfo _assetsDirectory;

        private AppaDirectoryInfo _dataDirectory;

        private AppaDirectoryInfo _gitDirectory;

        private AppaDirectoryInfo _srcDirectory;

        private AppaFileInfo _distributableFile;

        private bool _lookingForVersion;

        private bool? _hasDistributableFile;

        private IgnoreFile _gitIgnore;

        private IgnoreFile _npmIgnore;

        private Object _packageJsonAsset;

        private PublishMetadataCollection _publishStatus;

        private string _distributableVersion;

        private string _packageName;

        private string _packageVersion;
        private string _publishedVersion;

        private string _realPath;

        private string _repoName;

        public override string Id => Name;
        public override string Name => PackageName ?? RepoName;
        public override string Path => directory.RelativePath;

        public AppaFileInfo DistributableFile
        {
            get
            {
                using (_PRF_DistributableFile.Auto())
                {
                    if ((_distributableFile == null) && !_hasDistributableFile.HasValue)
                    {
                        if (!directory.Exists)
                        {
                            _hasDistributableFile = false;
                            return _distributableFile;
                        }

                        var searchPath = AppaPath.Combine(Path, "dist");

                        if (!Directory.Exists(searchPath))
                        {
                            _hasDistributableFile = false;
                            return _distributableFile;
                        }

                        var packagedPath = AppaDirectory
                                          .EnumerateFiles(searchPath, "*.tgz", SearchOption.TopDirectoryOnly)
                                          .OrderBy(f => f)
                                          .ToList()
                                          .FirstOrDefault();

                        if (packagedPath != null)
                        {
                            _hasDistributableFile = true;
                            _distributableFile = new AppaFileInfo(packagedPath);
                        }
                        else
                        {
                            _hasDistributableFile = false;
                        }
                    }

                    return _distributableFile;
                }
            }
        }

        public bool HasPackage => npmPackage != null;

        public bool? HasDistributableFile => _hasDistributableFile;

        public IgnoreFile GitIgnore
        {
            get
            {
                if (_gitIgnore == null)
                {
                    var gitIgnorePath = AppaPath.Combine(Path, ".gitignore");

                    if (AppaFile.Exists(gitIgnorePath))
                    {
                        _gitIgnore = new IgnoreFile(gitIgnorePath);
                    }
                }

                return _gitIgnore;
            }
        }

        public IgnoreFile NpmIgnore
        {
            get
            {
                if (_npmIgnore == null)
                {
                    var npmIgnorePath = AppaPath.Combine(Path, ".npmignore");

                    if (AppaFile.Exists(npmIgnorePath))
                    {
                        _npmIgnore = new IgnoreFile(npmIgnorePath);
                    }
                }

                return _npmIgnore;
            }
        }

        public Object PackageJsonAsset
        {
            get
            {
                if (_packageJsonAsset != null)
                {
                    return _packageJsonAsset;
                }

                _packageJsonAsset = AssetDatabaseManager.LoadAssetAtPath(NpmPackagePath, typeof(TextAsset));

                return _packageJsonAsset;
            }
        }

        public PublishMetadataCollection publishStatus
        {
            get
            {
                _publishStatus ??= new PublishMetadataCollection();
                return _publishStatus;
            }
        }

        public string DistributableVersion
        {
            get
            {
                using (_PRF_DistributableVersion.Auto())
                {
                    if ((_distributableVersion == null) &&
                        (!HasDistributableFile.HasValue || HasDistributableFile.Value))
                    {
                        var file = DistributableFile;

                        if (file == null)
                        {
                            return null;
                        }

                        _distributableVersion = file.FullPath.ParseNpmPackageVersion();
                    }

                    return _distributableVersion;
                }
            }
        }

        public string NameAndVersion => $"{Name}@{PackageVersion}";

        public string NpmPackagePath => AppaPath.Combine(directory.FullPath, "package.json");

        public string PackageName
        {
            get
            {
                if (npmPackage == null)
                {
                    return null;
                }

                if (_packageName == null)
                {
                    _packageName = npmPackage.Name;
                }

                return _packageName;
            }
        }

        public string PackageVersion
        {
            get
            {
                using (_PRF_PackageVersion.Auto())
                {
                    if (npmPackage == null)
                    {
                        return null;
                    }

                    if (_packageVersion == null)
                    {
                        _packageVersion = npmPackage.Version;
                    }

                    return _packageVersion;
                }
            }
        }

        public string PublishedVersion
        {
            get
            {
                using (_PRF_PublishedVersion.Auto())
                {
                    if (IsAppalachiaManaged)
                    {
                        SetPackageVersion();
                    }

                    return _publishedVersion;
                }
            }
        }

        public string RealPath
        {
            get
            {
                using (_PRF_RealPath.Auto())
                {
                    if (_realPath == null)
                    {
                        _realPath = Path;

                        if (_realPath.StartsWith("Packages"))
                        {
                            _realPath = _realPath.Replace("Packages", "Library/PackageCache");
                            _realPath += $"@{PackageVersion}";
                        }
                    }

                    return _realPath;
                }
            }
        }

        public string RepoName
        {
            get
            {
                using (_PRF_RepoName.Auto())
                {
                    if (_repoName != null)
                    {
                        return _repoName;
                    }

                    if (GitDirectory is not {Exists: true})
                    {
                        _repoName = PackageName;
                        return _repoName;
                    }

                    var repoFiles = GitDirectory.EnumerateFiles();
                    var repoConfig = repoFiles.First(f => f.Name == APPASTR.config);

                    var repoConfigStrings = new List<string>();

                    repoConfigStrings.AddRange(repoConfig.ReadAllLines());

                    for (var stringIndex = 0; stringIndex < repoConfigStrings.Count; stringIndex++)
                    {
                        var repoConfigString = repoConfigStrings[stringIndex];

                        //url = https://github.com/AppalachiaInteractive/com.appalachia.unity3d.audio.git
                        if (!repoConfigString.Contains(APPASTR.url))
                        {
                            continue;
                        }

                        var clean = repoConfigString.Trim();

                        var lastSlash = clean.LastIndexOf('/');
                        var subset = clean.Substring(lastSlash + 1);
                        subset = subset.Substring(0, subset.Length - 4);

                        _repoName = subset;
                        break;
                    }

                    if (_repoName == null)
                    {
                        _repoName = string.Empty;
                    }

                    return _repoName;
                }
            }
        }

        public AppaDirectoryInfo AssetsDirectory
        {
            get
            {
                using (_PRF_AssetsDirectory.Auto())
                {
                    if (_assetsDirectory == null)
                    {
                        if (directory == null)
                        {
                            return null;
                        }

                        _assetsDirectory =
                            new AppaDirectoryInfo(AppaPath.Combine(directory.FullPath, APPASTR.asset));
                    }

                    return _assetsDirectory;
                }
            }
            set => _assetsDirectory = value;
        }

        public AppaDirectoryInfo DataDirectory
        {
            get
            {
                if (_dataDirectory == null)
                {
                    if (directory == null)
                    {
                        return null;
                    }

                    _dataDirectory =
                        new AppaDirectoryInfo(AppaPath.Combine(directory.FullPath, APPASTR.data));
                }

                return _dataDirectory;
            }
            set => _dataDirectory = value;
        }

        public AppaDirectoryInfo GitDirectory
        {
            get
            {
                if (_gitDirectory == null)
                {
                    if (directory == null)
                    {
                        return null;
                    }

                    _gitDirectory = new AppaDirectoryInfo(AppaPath.Combine(directory.FullPath, APPASTR._git));
                }

                return _gitDirectory;
            }
            set => _gitDirectory = value;
        }

        public AppaDirectoryInfo SrcDirectory
        {
            get
            {
                if (_srcDirectory == null)
                {
                    if (directory == null)
                    {
                        return null;
                    }

                    _srcDirectory = new AppaDirectoryInfo(AppaPath.Combine(directory.FullPath, APPASTR.src));
                }

                return _srcDirectory;
            }
            set => _srcDirectory = value;
        }

        public static bool operator ==(RepositoryMetadata left, RepositoryMetadata right)
        {
            return Equals(left, right);
        }

        public static bool operator >(RepositoryMetadata left, RepositoryMetadata right)
        {
            return Comparer<RepositoryMetadata>.Default.Compare(left, right) > 0;
        }

        public static bool operator >=(RepositoryMetadata left, RepositoryMetadata right)
        {
            return Comparer<RepositoryMetadata>.Default.Compare(left, right) >= 0;
        }

        public static bool operator !=(RepositoryMetadata left, RepositoryMetadata right)
        {
            return !Equals(left, right);
        }

        public static bool operator <(RepositoryMetadata left, RepositoryMetadata right)
        {
            return Comparer<RepositoryMetadata>.Default.Compare(left, right) < 0;
        }

        public static bool operator <=(RepositoryMetadata left, RepositoryMetadata right)
        {
            return Comparer<RepositoryMetadata>.Default.Compare(left, right) <= 0;
        }

        public static RepositoryMetadata Empty()
        {
            return new();
        }

        private static IEnumerable<RepositoryMetadata> FindAllInternal()
        {
            using (_PRF_FindAllInternal.Auto())
            {
                var packageJsons = AssetDatabaseManager.FindAssetPathsByFileName("package.json");

                foreach (var packageJson in packageJsons)
                {
                    var directoryPath = AppaPath.GetDirectoryName(packageJson);
                    var directory = new AppaDirectoryInfo(directoryPath);

                    var instance = new RepositoryMetadata();

                    instance.InitializeInternal(directory);

                    yield return instance;
                }
            }
        }

        private static void FinalizeInternal()
        {
            using (_PRF_FinalizeInternal.Auto())
            {
                var assemblies = AssemblyDefinitionMetadata.Instances;

                foreach (var assembly in assemblies)
                {
                    assembly.repository.assemblies.Add(assembly);
                }
            }
        }

        public override bool Equals(RepositoryMetadata other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(directory?.FullPath,    other.directory?.FullPath) &&
                   Equals(GitDirectory?.FullPath, other.GitDirectory?.FullPath) &&
                   Equals(SrcDirectory?.FullPath, other.SrcDirectory?.FullPath);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((RepositoryMetadata) obj);
        }

        public override int CompareTo(RepositoryMetadata other)
        {
            if (ReferenceEquals(this, other))
            {
                return 0;
            }

            if (ReferenceEquals(null, other))
            {
                return 1;
            }

            var packageNameComparison = string.Compare(
                _packageName,
                other._packageName,
                StringComparison.Ordinal
            );
            if (packageNameComparison != 0)
            {
                return packageNameComparison;
            }

            return string.Compare(_packageVersion, other._packageVersion, StringComparison.Ordinal);
        }

        public override int CompareTo(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return 1;
            }

            if (ReferenceEquals(this, obj))
            {
                return 0;
            }

            return obj is RepositoryMetadata other
                ? CompareTo(other)
                : throw new ArgumentException($"Object must be of type {nameof(RepositoryMetadata)}");
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = directory != null ? directory.FullPath.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^
                           (GitDirectory != null ? GitDirectory.FullPath.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^
                           (SrcDirectory != null ? SrcDirectory.FullPath.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            if (!HasPackage)
            {
                return directory.RelativePath;
            }

            return $"{directory.RelativePath}: {PackageVersion}";
        }

        public override void InitializeForAnalysis()
        {
            _distributableFile = null;
            _distributableVersion = null;
            _gitIgnore = null;
            _hasDistributableFile = null;
            _lookingForVersion = false;
            _npmIgnore = null;
            _packageName = null;
            _packageVersion = null;
            _publishedVersion = null;
        }

        public IEnumerable<AppaFileInfo> GetLargestFiles(int count)
        {
            if (!SrcDirectory.Exists)
            {
                return Enumerable.Empty<AppaFileInfo>();
            }

            var files = SrcDirectory.EnumerateFiles("*", SearchOption.AllDirectories);

            if (AssetsDirectory.Exists)
            {
                var filesA = AssetsDirectory.EnumerateFiles("*", SearchOption.AllDirectories);
                files = files.Concat(filesA);
            }

            if (DataDirectory.Exists)
            {
                var filesD = DataDirectory.EnumerateFiles("*", SearchOption.AllDirectories);
                files = files.Concat(filesD);
            }

            return files.OrderByDescending(f => f.Length)
                        .Where(
                             f => !(f.FullPath.IsPathIgnored(GitIgnore) ||
                                    f.FullPath.IsPathIgnored(NpmIgnore))
                         )
                        .Take(count);
        }

        public IEnumerator ConvertToPackage(bool suspendImport, bool executeClient, bool dryRun = true)
        {
            
                Debug.Log($"Converting [{Name}] from a repository to a package.");
               
                
                foreach (var dependency in this.dependencies)
                {
                    if (dependency.repository.IsAppalachiaManaged)
                    {
                        var subEnum = dependency.repository.ConvertToPackage(false, executeClient, dryRun);

                        while (subEnum.MoveNext())
                        {
                            yield return subEnum;
                        }
                    }
                }

                var root = Path;
                var meta = root + ".meta";

                var newRoot = root + "~";

                if (dryRun)
                {
                    Debug.Log($"MOVEDIR: [{root}] to [{newRoot}]");
                    Debug.Log($"DELETE: [{meta}]");
                }
                else
                {
                    AppaDirectory.Move(root, newRoot);
                    AppaFile.Delete(meta);
                }

                if (executeClient)
                {
                    if (dryRun)
                    {
                        Debug.Log($"PKGMANAGER: Adding [{NameAndVersion}]");
                    }
                    else
                    {
                        var addition = UnityEditor.PackageManager.Client.Add(NameAndVersion);

                        while (!addition.IsCompleted)
                        {
                            yield return null;
                        }
                    }
                }
            
        }

        public void PopulateDependencies()
        {
            using (_PRF_PopulateDependencies.Auto())
            {
                if (dependencies == null)
                {
                    dependencies = new HashSet<RepositoryDependency>();
                }

                dependencies.Clear();

                if (npmPackage?.Dependencies != null)
                {
                    foreach (var dependency in npmPackage.Dependencies)
                    {
                        var newDep = new RepositoryDependency(dependency.Key, dependency.Value);

                        PopulateDependency(newDep);

                        dependencies.Add(newDep);
                    }
                }

                foreach (var dependency in missingDependencies)
                {
                    PopulateDependency(dependency);
                }
            }
        }

        public void PopulateDependency(RepositoryDependency dependency)
        {
            using (_PRF_PopulateDependency.Auto())
            {
                var repoMatch = FindById(dependency.name);

                if (repoMatch != null)
                {
                    dependency.repository = repoMatch;
                }
            }
        }
        
        private void PopulateDependents()
        {
            if (dependents != null)
            {
                return;
            }

            dependents = new HashSet<RepositoryMetadata>();

            var repositories = FindAll();

            foreach (var repository in repositories)
            {
                repository.PopulateDependencies();
                
                if (repository.dependencies.Any(d => d.repository == this))
                {
                    dependents.Add(repository);
                }
            }
        }
        
        public void SavePackageJson(bool useTestFiles, bool reimport)
        {
            using (_PRF_SavePackageJson.Auto())
            {
                var savePath = NpmPackagePath;

                if (useTestFiles)
                {
                    savePath += ".test";
                }

                var json = npmPackage.ToJson();

                AppaFile.WriteAllText(savePath, json);

                if (reimport)
                {
                    AssetDatabaseManager.ImportAsset(savePath);
                }
            }
        }

        public void SetPackageVersion()
        {
            using (_PRF_SetPackageVersion.Auto())
            {
                if ((_publishedVersion == null) && !_lookingForVersion)
                {
                    if (!IsAppalachiaManaged || !IsAsset || !directory.Exists)
                    {
                        return;
                    }

                    _lookingForVersion = true;

                    var result = new ShellResult();

                    var enumerator = SystemShell.Instance.Execute(
                        "npm v | grep latest",
                        this,
                        false,
                        result,
                        onComplete: () =>
                        {
                            try
                            {
                                _publishedVersion = result.output.ParseNpmPackageVersion();
                                ExecuteReanalyzeNecessary();
                            }
                            catch
                            {
                                _lookingForVersion = false;
                            }
                        }
                    );

                    enumerator.ToSafe().ExecuteAsEditorCoroutine();
                }
            }
        }

        protected override IEnumerable<string> GetIds()
        {
            yield return PackageName;
            yield return RepoName;
        }

        private void InitializeInternal(AppaDirectoryInfo dir)
        {
            using (_PRF_InitializeInternal.Auto())
            {
                directory = dir;
                GitDirectory = new AppaDirectoryInfo(AppaPath.Combine(directory.FullPath,    APPASTR._git));
                AssetsDirectory = new AppaDirectoryInfo(AppaPath.Combine(directory.FullPath, APPASTR.asset));
                DataDirectory = new AppaDirectoryInfo(AppaPath.Combine(directory.FullPath,   APPASTR.data));
                SrcDirectory = new AppaDirectoryInfo(AppaPath.Combine(directory.FullPath,    APPASTR.src));

                var npmPackagePath = AppaPath.Combine(directory.FullPath, "package.json");

                if (AppaFile.Exists(npmPackagePath))
                {
                    var text = AppaFile.ReadAllText(npmPackagePath);
                    try
                    {
                        npmPackage = NpmPackage.FromJson(text);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Failed to parse package.json at [{npmPackagePath}]: {ex.Message}");
                        throw;
                    }
                }

                PopulateDependencies();
                SetPackageVersion();
            }
        }
    }
}
