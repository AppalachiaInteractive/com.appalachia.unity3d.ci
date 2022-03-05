#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using Appalachia.CI.Integration.FileSystem;
using Unity.Profiling;
using Object = UnityEngine.Object;

namespace Appalachia.CI.Integration.Assets
{
    public static partial class AssetDatabaseManager
    {
        #region Static Fields and Autoproperties

        private static char[] _extensionTrims;

        private static Dictionary<string, List<AssetPath>> _assetPathsByExtension;
        private static Dictionary<string, List<AssetPath>> _projectPathsByExtension;

        private static Dictionary<Type, Dictionary<string, Object>> _firstLookupCache;

        private static List<AssetPath> _allAssetPaths;
        private static List<AssetPath> _allProjectPaths;

        #endregion

        private static Dictionary<string, List<AssetPath>> AssetPathsByExtension
        {
            get
            {
                using (_PRF_AssetPathsByExtension.Auto())
                {
                    InitializeAssetPathsByExtension();

                    return _assetPathsByExtension;
                }
            }
        }

        private static Dictionary<string, List<AssetPath>> ProjectPathsByExtension
        {
            get
            {
                using (_PRF_ProjectPathsByExtension.Auto())
                {
                    InitializeProjectPathsByExtension();

                    return _projectPathsByExtension;
                }
            }
        }

        private static List<AssetPath> AllAssetPaths
        {
            get
            {
                using (_PRF_AllAssetPaths.Auto())
                {
                    InitializeAllAssetPaths();

                    return _allAssetPaths;
                }
            }
        }

        private static List<AssetPath> AllProjectPaths
        {
            get
            {
                using (_PRF_AllProjectPaths.Auto())
                {
                    InitializeAllProjectPaths();

                    return _allProjectPaths;
                }
            }
        }

        private static void InitializeAllAssetPaths()
        {
            using (_PRF_InitializeAllAssetPaths.Auto())
            {
                if ((_allAssetPaths == null) || (_allAssetPaths.Count == 0))
                {
                    _assetPathsByExtension = null;

                    var relativePaths = UnityEditor.AssetDatabase.GetAllAssetPaths();

                    _allAssetPaths = new List<AssetPath>(relativePaths.Length);

                    for (var i = 0; i < relativePaths.Length; i++)
                    {
                        var relativePath = relativePaths[i];

                        _allAssetPaths.Add(AssetPath.FromRelativePath(relativePath));
                    }
                }
            }
        }

        private static void InitializeAllProjectPaths()
        {
            using (_PRF_InitializeAllProjectPaths.Auto())
            {
                if ((_allProjectPaths == null) || (_allProjectPaths.Count == 0))
                {
                    _projectPathsByExtension = null;

                    var packageLocation = ProjectLocations.GetPackagesDirectoryPath();
                    var assetsLocation = ProjectLocations.GetAssetsDirectoryPath();

                    var packageFilePaths = AppaDirectory.GetFiles(
                        packageLocation,
                        "*.*",
                        SearchOption.AllDirectories
                    );
                    var assetFilePaths = AppaDirectory.GetFiles(
                        assetsLocation,
                        "*.*",
                        SearchOption.AllDirectories
                    );

                    _allProjectPaths = new List<AssetPath>(packageFilePaths.Length + assetFilePaths.Length);

                    foreach (var filePath in packageFilePaths)
                    {
                        if (filePath.IsExcluded)
                        {
                            continue;
                        }

                        _allProjectPaths.Add(filePath);
                    }

                    foreach (var filePath in assetFilePaths)
                    {
                        if (filePath.IsExcluded)
                        {
                            continue;
                        }

                        _allProjectPaths.Add(filePath);
                    }
                }
            }
        }

        private static void InitializeAssetPathsByExtension()
        {
            using (_PRF_InitializeAssetPathsByExtension.Auto())
            {
                var allAssetPaths = AllAssetPaths;

                if ((_assetPathsByExtension == null) || (_assetPathsByExtension.Count == 0))
                {
                    _assetPathsByExtension = new Dictionary<string, List<AssetPath>>();

                    for (var pathIndex = 0; pathIndex < allAssetPaths.Count; pathIndex++)
                    {
                        var asset = allAssetPaths[pathIndex];
                        var extension = asset.Extension;

                        if (string.IsNullOrWhiteSpace(extension))
                        {
                            continue;
                        }

                        if (!_assetPathsByExtension.ContainsKey(extension))
                        {
                            _assetPathsByExtension.Add(extension, new List<AssetPath>());
                        }

                        _assetPathsByExtension[extension].Add(asset);
                    }
                }
            }
        }

        private static void InitializeProjectPathsByExtension()
        {
            using (_PRF_InitializeProjectPathsByExtension.Auto())
            {
                var allProjectPaths = AllProjectPaths;

                if ((_projectPathsByExtension == null) || (_projectPathsByExtension.Count == 0))
                {
                    _projectPathsByExtension = new Dictionary<string, List<AssetPath>>();

                    for (var pathIndex = 0; pathIndex < allProjectPaths.Count; pathIndex++)
                    {
                        var asset = allProjectPaths[pathIndex];
                        var extension = asset.Extension;

                        if (string.IsNullOrWhiteSpace(extension))
                        {
                            continue;
                        }

                        if (!_projectPathsByExtension.ContainsKey(extension))
                        {
                            _projectPathsByExtension.Add(extension, new List<AssetPath>());
                        }

                        _projectPathsByExtension[extension].Add(asset);
                    }
                }
            }
        }

        #region Profiling

        private static readonly ProfilerMarker _PRF_InitializeAssetPathsByExtension =
            new ProfilerMarker(_PRF_PFX + nameof(InitializeAssetPathsByExtension));

        private static readonly ProfilerMarker _PRF_AllAssetPaths =
            new ProfilerMarker(_PRF_PFX + nameof(AllAssetPaths));

        private static readonly ProfilerMarker _PRF_AllProjectPaths =
            new ProfilerMarker(_PRF_PFX + nameof(AllProjectPaths));

        private static readonly ProfilerMarker _PRF_AssetPathsByExtension =
            new ProfilerMarker(_PRF_PFX + nameof(AssetPathsByExtension));

        private static readonly ProfilerMarker _PRF_ProjectPathsByExtension =
            new ProfilerMarker(_PRF_PFX + nameof(ProjectPathsByExtension));

        private static readonly ProfilerMarker _PRF_InitializeAllAssetPaths =
            new ProfilerMarker(_PRF_PFX + nameof(InitializeAllAssetPaths));

        private static readonly ProfilerMarker _PRF_InitializeAllProjectPaths =
            new ProfilerMarker(_PRF_PFX + nameof(InitializeAllProjectPaths));

        private static readonly ProfilerMarker _PRF_InitializeProjectPathsByExtension =
            new ProfilerMarker(_PRF_PFX + nameof(InitializeProjectPathsByExtension));

        #endregion
    }
}

#endif
