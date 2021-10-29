using System;
using System.Collections.Generic;
using System.Linq;
using Appalachia.CI.Constants;
using Appalachia.CI.Integration.Assemblies;
using Appalachia.CI.Integration.Assets;
using Appalachia.CI.Integration.Core;
using Appalachia.CI.Integration.FileSystem;
using Appalachia.CI.Integration.Packages;
using Appalachia.CI.Integration.Packages.NpmModel;
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

        #endregion

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

        public HashSet<RepositoryDependency> dependencies;
        public HashSet<RepositoryDependency> missingDependencies;

        public List<AssemblyDefinitionMetadata> assemblies;

        public NpmPackage npmPackage;

        private AppaDirectoryInfo _assetsDirectory;

        private AppaDirectoryInfo _dataDirectory;

        private AppaDirectoryInfo _gitDirectory;

        private AppaDirectoryInfo _srcDirectory;

        private Object _packageJsonAsset;

        private string _packageName;

        private string _packageVersion;

        private string _repoName;

        public override string Id => Name;
        public override string Name => PackageName ?? RepoName;
        public override string Path => directory.RelativePath;

        public bool HasPackage => npmPackage != null;

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

        public string RepoName
        {
            get
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

                var repoFiles = GitDirectory.GetFiles();
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

        public AppaDirectoryInfo AssetsDirectory
        {
            get
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

        public void PopulateDependencies()
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

        public void PopulateDependency(RepositoryDependency dependency)
        {
            var repoMatch = FindById(dependency.name);

            if (repoMatch != null)
            {
                dependency.repository = repoMatch;
            }
        }

        public void SavePackageJson(bool useTestFiles, bool reimport)
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

        public override void InitializeForAnalysis()
        {
        }

        public override string ToString()
        {
            if (!HasPackage)
            {
                return directory.RelativePath;
            }

            return $"{directory.RelativePath}: {PackageVersion}";
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
            }
        }

        public static RepositoryMetadata Empty()
        {
            return new();
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
    }
}
