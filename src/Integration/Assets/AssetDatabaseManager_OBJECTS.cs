#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using Appalachia.CI.Integration.Core;
using Appalachia.CI.Integration.FileSystem;
using Unity.Profiling;
using Object = UnityEngine.Object;

namespace Appalachia.CI.Integration.Assets
{
    public static partial class AssetDatabaseManager
    {
        #region Static Fields and Autoproperties

        private static readonly ProfilerMarker _PRF_GetAllMonoScripts =
            new(_PRF_PFX + nameof(GetAllMonoScripts));

        private static readonly ProfilerMarker _PRF_InitializeTypeData =
            new(_PRF_PFX + nameof(InitializeTypeData));

        private static readonly ProfilerMarker _PRF_GetAllAssetGuids =
            new(_PRF_PFX + nameof(GetAllAssetGuids));

        private static readonly ProfilerMarker _PRF_GetAllAssetPaths =
            new(_PRF_PFX + nameof(GetAllAssetPaths));

        private static readonly ProfilerMarker _PRF_GetProjectAssetPaths =
            new(_PRF_PFX + nameof(GetProjectAssetPaths));

        private static readonly ProfilerMarker _PRF_GetAllAssetTypes =
            new(_PRF_PFX + nameof(GetAllAssetTypes));

        private static readonly ProfilerMarker _PRF_GetAssetGuid =
            new ProfilerMarker(_PRF_PFX + nameof(GetAssetGuid));

        private static Dictionary<string, AssetPath[]> _pathsByTypeName;

        private static Dictionary<string, string[]> _guidsByTypeName;
        private static Dictionary<string, Type[]> _typesByTypeName;

        #endregion

        public static string[] GetAllAssetGuids(Type type = null)
        {
            ThrowIfInvalidState();
            using (_PRF_GetAllAssetGuids.Auto())
            {
                var typeName = type?.Name ?? nameof(Object);
                InitializeTypeData(typeName);

                return _guidsByTypeName[typeName];
            }
        }

        public static AssetPath[] GetAllAssetPaths<T>()
            where T : Object
        {
            ThrowIfInvalidState();
            using (_PRF_GetAllAssetPaths.Auto())
            {
                return GetAllAssetPaths(typeof(T));
            }
        }

        public static AssetPath[] GetAllAssetPaths(Type type)
        {
            ThrowIfInvalidState();
            using (_PRF_GetAllAssetPaths.Auto())
            {
                var typeName = type?.Name ?? nameof(Object);
                InitializeTypeData(typeName);

                return _pathsByTypeName[typeName];
            }
        }

        public static Type[] GetAllAssetTypes(Type type = null)
        {
            ThrowIfInvalidState();
            using (_PRF_GetAllAssetTypes.Auto())
            {
                var typeName = type?.Name ?? nameof(Object);
                InitializeTypeData(typeName);

                return _typesByTypeName[typeName];
            }
        }

        public static string GetAssetGuid(Object asset)
        {
            ThrowIfInvalidState();
            using (_PRF_GetAssetGuid.Auto())
            {
                if (TryGetGUIDAndLocalFileIdentifier(asset, out var guid, out var _))
                {
                    return guid;
                }

                return null;
            }
        }

        public static AssetPath[] GetProjectAssetPaths(Type type = null)
        {
            ThrowIfInvalidState();
            using (_PRF_GetProjectAssetPaths.Auto())
            {
                var assetPaths = GetAllAssetPaths(type);

                var filteredAssetPaths = assetPaths.Where(
                                                        p => p.relativePath.StartsWith("Assets/") &&
                                                             p.relativePath.Contains("Appalachia")
                                                    )
                                                   .ToArray();

                Array.Sort(filteredAssetPaths);

                return filteredAssetPaths;
            }
        }

        private static void InitializeTypeData(string typeName)
        {
            ThrowIfInvalidState();
            using (_PRF_InitializeTypeData.Auto())
            {
                _typesByTypeName ??= new Dictionary<string, Type[]>();
                _pathsByTypeName ??= new Dictionary<string, AssetPath[]>();
                _guidsByTypeName ??= new Dictionary<string, string[]>();

                if (!_typesByTypeName.ContainsKey(typeName))
                {
                    var searchTerm = SearchStringBuilder.Build.AddTypeName(typeName).Finish();

                    var guids = FindAssets(searchTerm);

                    var sglength = guids.Length;

                    var paths = new AssetPath[sglength];
                    var types = new Type[sglength];

                    for (var i = 0; i < sglength; i++)
                    {
                        var guid = guids[i];
                        var path = GUIDToAssetPath(guid);
                        var type = GetMainAssetTypeAtPath(path);

                        paths[i] = path;
                        types[i] = type;
                    }

                    _typesByTypeName.Add(typeName, types);
                    _pathsByTypeName.Add(typeName, paths);
                    _guidsByTypeName.Add(typeName, guids);
                }
            }
        }

        #region Profiling

        #endregion
    }
}

#endif
