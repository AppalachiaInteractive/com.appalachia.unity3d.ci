using System.Collections.Generic;
using Appalachia.CI.Integration.Extensions;
using Appalachia.CI.Integration.FileSystem;
using Appalachia.CI.Integration.Repositories;
using Appalachia.Utility.Logging;
using Unity.Profiling;
using UnityEngine;

namespace Appalachia.CI.Integration
{
    public static class ProjectLocations
    {
        #region Profiling And Tracing Markers

        private const string _PRF_PFX = nameof(ProjectLocations) + ".";

        private static readonly ProfilerMarker _PRF_GetAssetRepository =
            new(_PRF_PFX + nameof(GetAssetRepository));

        private static AppaDirectoryInfo _assetAppaDirectory;
        private static AppaDirectoryInfo _projectAppaDirectory;
        private static Dictionary<AppaDirectoryInfo, RepositoryMetadata> _repoDirectoryLookup;
        private static Dictionary<string, AppaDirectoryInfo> _thirdPartyAssetAppaDirectory;
        private static Dictionary<string, RepositoryMetadata> _assetRepoLookup;
        private static Dictionary<string, string> _thirdPartyAssetDirectoryPath;
        private static Dictionary<string, string> _thirdPartyAssetDirectoryPathRelative;
        private static string _assetDirectoryPath;
        private static string _assetDirectoryPathRelative;
        private static string _packagesDirectoryPath;
        private static string _packagesDirectoryPathRelative;
        private static string _projectDirectoryPath;
        private static string _projectDirectoryPathRelative;

        private static readonly ProfilerMarker _PRF_GetAssetsAppaDirectory =
            new(_PRF_PFX + nameof(GetAssetsAppaDirectory));

        private static readonly ProfilerMarker _PRF_GetAssetsDirectoryPath =
            new(_PRF_PFX + nameof(GetAssetsDirectoryPath));

        private static readonly ProfilerMarker _PRF_GetAssetsDirectoryPathRelative =
            new(_PRF_PFX + nameof(GetAssetsDirectoryPathRelative));

        private static readonly ProfilerMarker _PRF_GetProjectDirectoryPath =
            new(_PRF_PFX + nameof(GetProjectDirectoryPath));

        private static readonly ProfilerMarker _PRF_GetProjectDirectoryPathRelative =
            new(_PRF_PFX + nameof(GetProjectDirectoryPathRelative));

        private static readonly ProfilerMarker _PRF_GetThirdPartyAssetsAppaDirectory =
            new(_PRF_PFX + nameof(GetThirdPartyAssetsAppaDirectory));

        private static readonly ProfilerMarker _PRF_GetThirdPartyAssetsDirectoryPath =
            new(_PRF_PFX + nameof(GetThirdPartyAssetsDirectoryPath));

        private static readonly ProfilerMarker _PRF_GetThirdPartyAssetsDirectoryPathRelative =
            new(_PRF_PFX + nameof(GetThirdPartyAssetsDirectoryPathRelative));

        private static readonly ProfilerMarker _PRF_GetPackagesDirectoryPath =
            new(_PRF_PFX + nameof(GetPackagesDirectoryPath));

        private static readonly ProfilerMarker _PRF_GetPackagesDirectoryPathRelative =
            new(_PRF_PFX + nameof(GetPackagesDirectoryPathRelative));

        #endregion

        public const string ThirdPartyDataFolder = ThirdPartyFolder + " Data";
        public const string ThirdPartyFolder = "Third-Party";

        public static RepositoryMetadata GetAssetRepository(string assetPath, bool invalidateCache = true)
        {
            using (_PRF_GetAssetRepository.Auto())
            {
                if (string.IsNullOrWhiteSpace(assetPath))
                {
                    return null;
                }

                if (invalidateCache || (_assetRepoLookup == null))
                {
                    _assetRepoLookup = new Dictionary<string, RepositoryMetadata>();
                }

                if (_assetRepoLookup.ContainsKey(assetPath))
                {
                    return _assetRepoLookup[assetPath];
                }

                var directoryInfo = new AppaDirectoryInfo(assetPath);

                try
                {
                    while (!directoryInfo.IsInRepositoryRoot())
                    {
                        directoryInfo = directoryInfo.Parent;

                        if (directoryInfo == null)
                        {
                            return null;
                        }
                    }
                }
                catch
                {
                    AppaLog.Error(assetPath);
                }

                if (_repoDirectoryLookup == null)
                {
                    _repoDirectoryLookup = new Dictionary<AppaDirectoryInfo, RepositoryMetadata>();
                }

                var rootDirectory = directoryInfo.Parent;

                if (_repoDirectoryLookup.ContainsKey(rootDirectory))
                {
                    return _repoDirectoryLookup[rootDirectory];
                }

                var repo = RepositoryMetadata.FindInDirectory(rootDirectory);

                _assetRepoLookup.Add(assetPath, repo);
                _repoDirectoryLookup.Add(rootDirectory, repo);

                return repo;
            }
        }

        public static AppaDirectoryInfo GetAssetsAppaDirectory()
        {
            using (_PRF_GetAssetsAppaDirectory.Auto())
            {
                if (_assetAppaDirectory != null)
                {
                    return _assetAppaDirectory;
                }

                _assetAppaDirectory = new AppaDirectoryInfo(GetAssetsDirectoryPath());
                return _assetAppaDirectory;
            }
        }

        public static string GetAssetsDirectoryPath()
        {
            using (_PRF_GetAssetsDirectoryPath.Auto())
            {
                if (_assetDirectoryPath != null)
                {
                    return _assetDirectoryPath;
                }

                _assetDirectoryPath = Application.dataPath.CleanFullPath();
                return _assetDirectoryPath;
            }
        }

        public static string GetAssetsDirectoryPathRelative()
        {
            using (_PRF_GetAssetsDirectoryPathRelative.Auto())
            {
                if (_assetDirectoryPathRelative != null)
                {
                    return _assetDirectoryPathRelative;
                }

                _assetDirectoryPathRelative = "Assets";
                return _assetDirectoryPathRelative;
            }
        }

        public static string GetPackagesDirectoryPath()
        {
            using (_PRF_GetPackagesDirectoryPath.Auto())
            {
                if (_packagesDirectoryPath != null)
                {
                    return _packagesDirectoryPath;
                }

                var assetsDirectoryPathRelative = GetAssetsDirectoryPathRelative();
                var packagesDirectoryPathRelative = GetPackagesDirectoryPathRelative();

                _packagesDirectoryPath = GetAssetsDirectoryPath()
                   .Replace($"{assetsDirectoryPathRelative}", packagesDirectoryPathRelative);

                return _packagesDirectoryPath;
            }
        }

        public static string GetPackagesDirectoryPathRelative()
        {
            using (_PRF_GetPackagesDirectoryPathRelative.Auto())
            {
                if (_packagesDirectoryPathRelative != null)
                {
                    return _packagesDirectoryPathRelative;
                }

                _packagesDirectoryPathRelative = "Library/PackageCache";
                return _packagesDirectoryPathRelative;
            }
        }

        public static string GetProjectDirectoryPath()
        {
            using (_PRF_GetProjectDirectoryPath.Auto())
            {
                if (_projectDirectoryPath != null)
                {
                    return _projectDirectoryPath;
                }

                var assetsDirectoryPathRelative = GetAssetsDirectoryPathRelative();
                var projectDirectoryPathRelative = GetProjectDirectoryPathRelative();

                _projectDirectoryPath = GetAssetsDirectoryPath()
                   .Replace($"{assetsDirectoryPathRelative}", projectDirectoryPathRelative);
                return _projectDirectoryPath;
            }
        }

        public static string GetProjectDirectoryPathRelative()
        {
            using (_PRF_GetProjectDirectoryPathRelative.Auto())
            {
                if (_projectDirectoryPathRelative != null)
                {
                    return _projectDirectoryPathRelative;
                }

                _projectDirectoryPathRelative = string.Empty;
                return _projectDirectoryPathRelative;
            }
        }

        public static AppaDirectoryInfo GetThirdPartyAssetsAppaDirectory(string partyName)
        {
            using (_PRF_GetThirdPartyAssetsAppaDirectory.Auto())
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
        }

        public static string GetThirdPartyAssetsDirectoryPath(string partyName)
        {
            using (_PRF_GetThirdPartyAssetsDirectoryPath.Auto())
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
                var thirdPartyPath =
                    AppaPath.Combine(basePath, ThirdPartyDataFolder, partyName).CleanFullPath();
                var thirdPartyInfo = new AppaDirectoryInfo(thirdPartyPath);

                if (!thirdPartyInfo.Exists)
                {
                    thirdPartyInfo.Create();
                }

                _thirdPartyAssetDirectoryPath.Add(partyName, thirdPartyPath);

                return thirdPartyPath;
            }
        }

        public static string GetThirdPartyAssetsDirectoryPathRelative(string partyName)
        {
            using (_PRF_GetThirdPartyAssetsDirectoryPathRelative.Auto())
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
                var thirdPartyPath =
                    AppaPath.Combine(basePath, ThirdPartyDataFolder, partyName).CleanFullPath();
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
}
