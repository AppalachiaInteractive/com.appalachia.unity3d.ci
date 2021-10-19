using System;
using System.Collections.Generic;
using System.Linq;
using Appalachia.CI.Integration.Assemblies;
using Appalachia.CI.Integration.Assets;
using Appalachia.CI.Integration.FileSystem;
using Appalachia.CI.Integration.Packages;
using UnityEngine;

namespace Appalachia.CI.Integration.Repositories
{
    [Serializable]
    public class RepositoryDirectoryMetadata : IEquatable<RepositoryDirectoryMetadata>
    {
        private const string ASSET = "asset";
        private const string CONFIG = "config";
        private const string DATA = "data";
        private const string GIT = ".git";
        private const string PACKAGEJSON = "package.json";
        private const string SRC = "src";
        private const string URL = "url";
        private const string VERSION = "version";

        public List<AssemblyDefinitionMetadata> assemblies;
        public NpmPackage npmPackage;

        private static bool _allPrepared;
        private static Dictionary<string, RepositoryDirectoryMetadata> _instances;
        public AppaDirectoryInfo root;

        private AppaDirectoryInfo _assetsDirectory;

        private AppaDirectoryInfo _dataDirectory;

        private AppaDirectoryInfo _gitDirectory;
        private string _packageName;
        private string _packageVersion;

        private string _repoName;

        private AppaDirectoryInfo _srcDirectory;

        private RepositoryDirectoryMetadata()
        {
            assemblies = new List<AssemblyDefinitionMetadata>();
        }

        private RepositoryDirectoryMetadata(
            AppaDirectoryInfo root,
            AppaDirectoryInfo gitDirectory,
            AppaDirectoryInfo assetsDirectory,
            AppaDirectoryInfo dataDirectory,
            AppaDirectoryInfo srcDirectory,
            NpmPackage npmPackage)
        {
            this.root = root;
            this.gitDirectory = gitDirectory;
            this.assetsDirectory = assetsDirectory;
            this.dataDirectory = dataDirectory;
            this.srcDirectory = srcDirectory;
            this.npmPackage = npmPackage;
            assemblies = new List<AssemblyDefinitionMetadata>();
        }

        public bool HasPackage => npmPackage != null;

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

        public string repoName
        {
            get
            {
                if (_repoName != null)
                {
                    return _repoName;
                }

                if (gitDirectory is not {Exists: true})
                {
                    return null;
                }

                var repoFiles = gitDirectory.GetFiles();
                var repoConfig = repoFiles.First(f => f.Name == CONFIG);

                var repoConfigStrings = new List<string>();

                repoConfigStrings.AddRange(repoConfig.ReadAllLines());

                for (var stringIndex = 0; stringIndex < repoConfigStrings.Count; stringIndex++)
                {
                    var repoConfigString = repoConfigStrings[stringIndex];

                    //url = https://github.com/AppalachiaInteractive/com.appalachia.unity3d.audio.git
                    if (!repoConfigString.Contains(URL))
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

        public AppaDirectoryInfo assetsDirectory
        {
            get
            {
                if (_assetsDirectory == null)
                {
                    if (root == null)
                    {
                        return null;
                    }

                    _assetsDirectory = new AppaDirectoryInfo(AppaPath.Combine(root.FullPath, ASSET));
                }

                return _assetsDirectory;
            }
            set => _assetsDirectory = value;
        }

        public AppaDirectoryInfo dataDirectory
        {
            get
            {
                if (_dataDirectory == null)
                {
                    if (root == null)
                    {
                        return null;
                    }

                    _dataDirectory = new AppaDirectoryInfo(AppaPath.Combine(root.FullPath, DATA));
                }

                return _dataDirectory;
            }
            set => _dataDirectory = value;
        }

        public AppaDirectoryInfo gitDirectory
        {
            get
            {
                if (_gitDirectory == null)
                {
                    if (root == null)
                    {
                        return null;
                    }

                    _gitDirectory = new AppaDirectoryInfo(AppaPath.Combine(root.FullPath, GIT));
                }

                return _gitDirectory;
            }
            set => _gitDirectory = value;
        }

        public AppaDirectoryInfo srcDirectory
        {
            get
            {
                if (_srcDirectory == null)
                {
                    if (root == null)
                    {
                        return null;
                    }

                    _srcDirectory = new AppaDirectoryInfo(AppaPath.Combine(root.FullPath, SRC));
                }

                return _srcDirectory;
            }
            set => _srcDirectory = value;
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

            return Equals((RepositoryDirectoryMetadata) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = root != null ? root.FullPath.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^
                           (gitDirectory != null ? gitDirectory.FullPath.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^
                           (srcDirectory != null ? srcDirectory.FullPath.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            if (!HasPackage)
            {
                return root.FullPath;
            }

            return $"{root.FullPath}: {PackageVersion}";
        }

        public void SavePackageJson(bool useTestFiles, bool reimport)
        {
            var packageJsonPath = AppaPath.Combine(root.FullPath, "package.json");

            if (useTestFiles)
            {
                packageJsonPath += ".test";
            }

            var json = npmPackage.ToJson();

            AppaFile.WriteAllText(packageJsonPath, json);

            if (reimport)
            {
                AssetDatabaseManager.ImportAsset(packageJsonPath);
            }
        }

        public bool Equals(RepositoryDirectoryMetadata other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(root?.FullPath,         other.root?.FullPath) &&
                   Equals(gitDirectory?.FullPath, other.gitDirectory?.FullPath) &&
                   Equals(srcDirectory?.FullPath, other.srcDirectory?.FullPath);
        }

        public static RepositoryDirectoryMetadata Create(
            AppaDirectoryInfo root,
            AppaDirectoryInfo gitDirectory,
            AppaDirectoryInfo assetsDirectory,
            AppaDirectoryInfo dataDirectory,
            AppaDirectoryInfo srcDirectory,
            NpmPackage packageJson)
        {
            if (_instances == null)
            {
                _instances = new Dictionary<string, RepositoryDirectoryMetadata>();
            }

            if (_instances.ContainsKey(root.FullPath))
            {
                return _instances[root.FullPath];
            }

            var newInstance = new RepositoryDirectoryMetadata(
                root,
                gitDirectory,
                assetsDirectory,
                dataDirectory,
                srcDirectory,
                packageJson
            );

            _instances.Add(root.FullPath, newInstance);

            return newInstance;
        }

        public static RepositoryDirectoryMetadata Empty()
        {
            return new();
        }

        public static RepositoryDirectoryMetadata Find(string name)
        {
            foreach (var instance in _instances)
            {
                var repo = instance.Value;

                if (name == repo.PackageName)
                {
                    return repo;
                }
            }

            return null;
        }

        public static IEnumerable<RepositoryDirectoryMetadata> FindAll()
        {
            PrepareAll();

            return _instances.Values;
        }

        public static RepositoryDirectoryMetadata FromRoot(AppaDirectoryInfo directory)
        {
            if (_instances == null)
            {
                _instances = new Dictionary<string, RepositoryDirectoryMetadata>();
            }

            if (_instances.ContainsKey(directory.FullPath))
            {
                return _instances[directory.FullPath];
            }

            var git = new AppaDirectoryInfo(AppaPath.Combine(directory.FullPath,   GIT));
            var asset = new AppaDirectoryInfo(AppaPath.Combine(directory.FullPath, ASSET));
            var data = new AppaDirectoryInfo(AppaPath.Combine(directory.FullPath,  DATA));
            var src = new AppaDirectoryInfo(AppaPath.Combine(directory.FullPath,   SRC));

            NpmPackage package = null;

            var packageJsonPath = AppaPath.Combine(directory.FullPath, "package.json");

            if (AppaFile.Exists(packageJsonPath))
            {
                var text = AppaFile.ReadAllText(packageJsonPath);
                try
                {
                    package = NpmPackage.FromJson(text);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to parse package.json at [{packageJsonPath}]: {ex.Message}");
                    throw;
                }
            }

            var result = Create(directory, git, asset, data, src, package);

            if (result.assemblies == null)
            {
                result.assemblies = new List<AssemblyDefinitionMetadata>();
            }

            return result;
        }

        public static RepositoryDirectoryMetadata FromRootPath(string path)
        {
            var directory = new AppaDirectoryInfo(path);
            return FromRoot(directory);
        }

        public static bool operator ==(RepositoryDirectoryMetadata left, RepositoryDirectoryMetadata right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(RepositoryDirectoryMetadata left, RepositoryDirectoryMetadata right)
        {
            return !Equals(left, right);
        }

        public static void PrepareAll()
        {
            if (_allPrepared)
            {
                return;
            }

            var packageJsons = AssetDatabaseManager.FindAssetPathsByFileName("package.json");

            foreach (var packageJson in packageJsons)
            {
                var directory = AppaPath.GetDirectoryName(packageJson);

                FromRootPath(directory);
            }

            _allPrepared = true;
        }
    }
}
