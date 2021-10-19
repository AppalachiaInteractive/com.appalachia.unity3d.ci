using System.Collections.Generic;
using Appalachia.CI.Integration.Extensions;
using Appalachia.CI.Integration.FileSystem;
using Appalachia.CI.Integration.Repositories;
using UnityEngine;

namespace Appalachia.CI.Integration
{
    public static class ProjectLocations
    {
        public const string ThirdPartyDataFolder = ThirdPartyFolder + "/Data";
        public const string ThirdPartyFolder = "Third-Party";
        private static AppaDirectoryInfo _assetAppaDirectory;
        private static AppaDirectoryInfo _projectAppaDirectory;
        private static Dictionary<AppaDirectoryInfo, RepositoryDirectoryMetadata> _repoDirectoryLookup;
        private static Dictionary<string, AppaDirectoryInfo> _thirdPartyAssetAppaDirectory;
        private static Dictionary<string, RepositoryDirectoryMetadata> _assetRepoLookup;
        private static Dictionary<string, string> _thirdPartyAssetDirectoryPath;
        private static Dictionary<string, string> _thirdPartyAssetDirectoryPathRelative;

        private static string _assetDirectoryPath;

        private static string _assetDirectoryPathRelative;
        private static string _projectDirectoryPath;
        private static string _projectDirectoryPathRelative;

        public static AppaDirectoryInfo GetAssetsAppaDirectory()
        {
            if (_assetAppaDirectory != null)
            {
                return _assetAppaDirectory;
            }

            _assetAppaDirectory = new AppaDirectoryInfo(GetAssetsDirectoryPath());
            return _assetAppaDirectory;
        }

        public static AppaDirectoryInfo GetThirdPartyAssetsAppaDirectory(string partyName)
        {
            if (_thirdPartyAssetAppaDirectory == null)
            {
                _thirdPartyAssetAppaDirectory = new Dictionary<string, AppaDirectoryInfo>();
            }

            if (_thirdPartyAssetAppaDirectory.ContainsKey(partyName))
            {
                return _thirdPartyAssetAppaDirectory[partyName];
            }

            var thirdParty = GetThirdPartyAssetsDirectoryPath(partyName);

            var thirdPartyInfo = new AppaDirectoryInfo(thirdParty);
            _thirdPartyAssetAppaDirectory.Add(partyName, thirdPartyInfo);

            if (!thirdPartyInfo.Exists)
            {
                thirdPartyInfo.Create();
            }

            return thirdPartyInfo;
        }

        public static RepositoryDirectoryMetadata GetAssetRepository(
            string assetPath,
            bool invalidateCache = true)
        {
            if (string.IsNullOrWhiteSpace(assetPath))
            {
                return null;
            }

            if (invalidateCache || (_assetRepoLookup == null))
            {
                _assetRepoLookup = new Dictionary<string, RepositoryDirectoryMetadata>();
            }

            if (_assetRepoLookup.ContainsKey(assetPath))
            {
                return _assetRepoLookup[assetPath];
            }

            var directoryInfo = new AppaDirectoryInfo(assetPath);

            while (!directoryInfo.HasSiblingDirectory(".git", out _) &&
                   !directoryInfo.HasSiblingFile("package.json", out _))
            {
                directoryInfo = directoryInfo.Parent;

                if (directoryInfo == null)
                {
                    return null;
                }
            }

            if (_repoDirectoryLookup == null)
            {
                _repoDirectoryLookup = new Dictionary<AppaDirectoryInfo, RepositoryDirectoryMetadata>();
            }

            var rootDirectory = directoryInfo.Parent;

            if (_repoDirectoryLookup.ContainsKey(rootDirectory))
            {
                return _repoDirectoryLookup[rootDirectory];
            }

            var repo = RepositoryDirectoryMetadata.FromRoot(rootDirectory);

            _assetRepoLookup.Add(assetPath, repo);
            _repoDirectoryLookup.Add(rootDirectory, repo);

            return repo;
        }

        public static string GetAssetsDirectoryPath()
        {
            if (_assetDirectoryPath != null)
            {
                return _assetDirectoryPath;
            }

            _assetDirectoryPath = Application.dataPath.CleanFullPath();
            return _assetDirectoryPath;
        }

        public static string GetAssetsDirectoryPathRelative()
        {
            if (_assetDirectoryPathRelative != null)
            {
                return _assetDirectoryPathRelative;
            }

            _assetDirectoryPathRelative = "Assets";
            return _assetDirectoryPathRelative;
        }

        public static string GetProjectDirectoryPath()
        {
            if (_projectDirectoryPath != null)
            {
                return _projectDirectoryPath;
            }

            _projectDirectoryPath = GetAssetsDirectoryPath().Replace("/Assets", string.Empty);
            return _projectDirectoryPath;
        }

        public static string GetProjectDirectoryPathRelative()
        {
            if (_projectDirectoryPathRelative != null)
            {
                return _projectDirectoryPathRelative;
            }

            _projectDirectoryPathRelative = string.Empty;
            return _projectDirectoryPathRelative;
        }

        public static string GetThirdPartyAssetsDirectoryPath(string partyName)
        {
            if (_thirdPartyAssetDirectoryPath == null)
            {
                _thirdPartyAssetDirectoryPath = new Dictionary<string, string>();
            }

            if (_thirdPartyAssetDirectoryPath.ContainsKey(partyName))
            {
                return _thirdPartyAssetDirectoryPath[partyName];
            }

            var basePath = GetAssetsDirectoryPath();
            var thirdPartyPath = AppaPath.Combine(basePath, ThirdPartyDataFolder, partyName).CleanFullPath();
            var thirdPartyInfo = new AppaDirectoryInfo(thirdPartyPath);

            if (!thirdPartyInfo.Exists)
            {
                thirdPartyInfo.Create();
            }

            _thirdPartyAssetDirectoryPath.Add(partyName, thirdPartyPath);

            return thirdPartyPath;
        }

        public static string GetThirdPartyAssetsDirectoryPathRelative(string partyName)
        {
            if (_thirdPartyAssetDirectoryPathRelative == null)
            {
                _thirdPartyAssetDirectoryPathRelative = new Dictionary<string, string>();
            }

            if (_thirdPartyAssetDirectoryPathRelative.ContainsKey(partyName))
            {
                return _thirdPartyAssetDirectoryPathRelative[partyName];
            }

            var basePath = GetAssetsDirectoryPathRelative();
            var thirdPartyPath = AppaPath.Combine(basePath, ThirdPartyDataFolder, partyName).CleanFullPath();
            var thirdPartyInfo = new AppaDirectoryInfo(thirdPartyPath);

            if (!thirdPartyInfo.Exists)
            {
                thirdPartyInfo.Create();
            }

            _thirdPartyAssetDirectoryPathRelative.Add(partyName, thirdPartyPath);

            return thirdPartyPath;
        }
    }
}
