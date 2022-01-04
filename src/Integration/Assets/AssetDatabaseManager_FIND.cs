#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using Appalachia.CI.Integration.Extensions;
using Appalachia.CI.Integration.FileSystem;
using Appalachia.Utility.Extensions;
using Appalachia.Utility.Reflection.Extensions;
using Appalachia.Utility.Strings;
using Unity.Profiling;
using Object = UnityEngine.Object;

namespace Appalachia.CI.Integration.Assets
{
    public static partial class AssetDatabaseManager
    {
        #region Static Fields and Autoproperties

        private static char[] _extensionTrims;

        private static Dictionary<string, List<string>> _assetPathsByExtension;
        private static Dictionary<string, List<string>> _projectPathsByExtension;

        private static Dictionary<Type, Dictionary<string, Object>> _firstLookupCache;
        private static string[] _allAssetPaths;
        private static string[] _allProjectPaths;

        #endregion

        public static string[] FindAssetGuids<T>(string searchString = null)
            where T : Object
        {
            ThrowIfInvalidState();
            using (_PRF_FindAssetGuids.Auto())
            {
                InitializeAssetPathData();

                return FindAssetGuids(typeof(T), searchString);
            }
        }

        public static string[] FindAssetGuids(Type t, string searchString = null)
        {
            ThrowIfInvalidState();
            using (_PRF_FindAssetGuids.Auto())
            {
                InitializeAssetPathData();

                var guids = FindAssets(FormatSearchString(t, searchString));

                return guids;
            }
        }

        public static List<string> FindAssetPaths(string searchString = null)
        {
            ThrowIfInvalidState();
            using (_PRF_FindAssetPaths.Auto())
            {
                InitializeAssetPathData();

                return FindAssetPaths(null, searchString);
            }
        }

        public static List<string> FindAssetPaths<T>(string searchString = null)
        {
            ThrowIfInvalidState();
            using (_PRF_FindAssetPaths.Auto())
            {
                InitializeAssetPathData();

                return FindAssetPaths(typeof(T), searchString);
            }
        }

        public static List<string> FindAssetPaths(Type t, string searchString = null)
        {
            ThrowIfInvalidState();
            using (_PRF_FindAssetPaths.Auto())
            {
                InitializeAssetPathData();

                var guids = FindAssetGuids(t, searchString);
                var paths = new List<string>(guids.Length);

                for (var index = 0; index < guids.Length; index++)
                {
                    var guid = guids[index];
                    var path = GUIDToAssetPath(guid).CleanFullPath();

                    if (!AppaFile.Exists(path))
                    {
                        continue;
                    }

                    paths.Add(path);
                }

                return paths;
            }
        }

        public static List<string> FindAssetPathsByExtension(string extension)
        {
            ThrowIfInvalidState();
            using (_PRF_FindAssetPathsByExtension.Auto())
            {
                InitializeAssetPathData();

                var cleanExtension = extension.CleanExtension();

                if (_assetPathsByExtension.ContainsKey(cleanExtension))
                {
                    return _assetPathsByExtension[cleanExtension];
                }

                return null;
            }
        }

        public static List<string> FindAssetPathsByFileName(string name)
        {
            ThrowIfInvalidState();
            using (_PRF_FindAssetPathsByFileName.Auto())
            {
                InitializeAssetPathData();

                var output = new List<string>();

                foreach (var path in _allAssetPaths)
                {
                    if (name == AppaPath.GetFileName(path))
                    {
                        output.Add(path);
                    }
                }

                return output;
            }
        }

        public static List<string> FindAssetPathsBySubstring(string substring)
        {
            ThrowIfInvalidState();
            using (_PRF_FindAssetPathsBySubstring.Auto())
            {
                InitializeAssetPathData();

                var output = new List<string>();

                foreach (var path in _allAssetPaths)
                {
                    if (path.Contains(substring))
                    {
                        output.Add(path);
                    }
                }

                return output;
            }
        }

        public static List<Object> FindAssets(Type t, string searchString = null)
        {
            ThrowIfInvalidState();
            using (_PRF_FindAssets.Auto())
            {
                var paths = FindAssetPaths(t, searchString);
                var results = new List<Object>(paths.Count);

                for (var i = 0; i < paths.Count; i++)
                {
                    var path = paths[i];

                    var type = GetMainAssetTypeAtPath(path);

                    if (t.ImplementsOrInheritsFrom(type))
                    {
                        var cast = LoadAssetAtPath(path, t);
                        results.Add(cast);
                    }
                }

                return results;
            }
        }

        public static List<T> FindAssets<T>(string searchString = null)
            where T : Object
        {
            ThrowIfInvalidState();
            using (_PRF_FindAssets.Auto())
            {
                InitializeAssetPathData();

                var t = typeof(T);

                var paths = FindAssetPaths<T>(searchString);
                var results = new List<T>(paths.Count);

                for (var i = 0; i < paths.Count; i++)
                {
                    var path = paths[i];

                    var type = GetMainAssetTypeAtPath(path);

                    if (t.ImplementsOrInheritsFrom(type))
                    {
                        var cast = LoadAssetAtPath<T>(path);
                        results.Add(cast);
                    }

                    var subAssets = LoadAllAssetRepresentationsAtPath(path);

                    foreach (var subAsset in subAssets)
                    {
                        var subAssetType = subAsset.GetType();

                        if (t.ImplementsOrInheritsFrom(subAssetType))
                        {
                            var cast = subAsset as T;
                            results.Add(cast);
                        }
                    }
                }

                return results;
            }
        }

        public static T FindFirstAssetMatch<T>(string searchString)
            where T : Object
        {
            ThrowIfInvalidState();
            using (_PRF_FindFirstAsset.Auto())
            {
                return FindFirstAssetMatch(typeof(T), searchString) as T;
            }
        }

        public static Object FindFirstAssetMatch(Type t, string searchString = null)
        {
            ThrowIfInvalidState();
            using (_PRF_FindFirstAsset.Auto())
            {
                _firstLookupCache ??= new Dictionary<Type, Dictionary<string, Object>>();

                if (!_firstLookupCache.ContainsKey(t))
                {
                    _firstLookupCache.Add(t, new Dictionary<string, Object>());
                }

                if (searchString != null)
                {
                    if (_firstLookupCache[t].ContainsKey(searchString))
                    {
                        return _firstLookupCache[t][searchString];
                    }
                }

                var results = FindAssets(t, searchString);
                var sortedResults = results.OrderByDescending(v => v.name == searchString ? 1 : 0);
                var result = sortedResults.FirstOrDefault();

                if (searchString != null)
                {
                    _firstLookupCache[t].Add(searchString, result);
                }

                return result;
            }
        }

        public static List<string> FindProjectPathsByExtension(string extension)
        {
            ThrowIfInvalidState();
            using (_PRF_FindProjectPathsByExtension.Auto())
            {
                InitializeAssetPathData();

                var cleanExtension = extension.CleanExtension();

                if (_projectPathsByExtension.ContainsKey(cleanExtension))
                {
                    return _projectPathsByExtension[cleanExtension];
                }

                return null;
            }
        }

        private static string FormatSearchString(Type t, string searchString)
        {
            ThrowIfInvalidState();
            using (_PRF_FormatSearchString.Auto())
            {
                if (searchString.IsNullOrWhiteSpace() && (t == null))
                {
                    throw new ArgumentNullException(nameof(searchString));
                }

                if (t == null)
                {
                    return searchString;
                }

                var typename = t.Name;
                return ZString.Format("t:{0} {1}", typename, searchString ?? string.Empty);
            }
        }

        private static void InitializeAssetPathData()
        {
            ThrowIfInvalidState();
            using (_PRF_InitializeAssetPathData.Auto())
            {
                if ((_allAssetPaths == null) || (_allAssetPaths.Length == 0))
                {
                    _allAssetPaths = GetAllAssetPaths();

                    _assetPathsByExtension = null;
                }

                if ((_allProjectPaths == null) || (_allProjectPaths.Length == 0))
                {
                    _allProjectPaths = GetAllProjectPaths();

                    _projectPathsByExtension = null;
                }

                if ((_projectPathsByExtension == null) || (_projectPathsByExtension.Count == 0))
                {
                    _projectPathsByExtension = new Dictionary<string, List<string>>();

                    foreach (var asset in _allProjectPaths)
                    {
                        var extension = AppaPath.GetExtension(asset).CleanExtension();

                        if (string.IsNullOrWhiteSpace(extension))
                        {
                            continue;
                        }

                        if (!_projectPathsByExtension.ContainsKey(extension))
                        {
                            _projectPathsByExtension.Add(extension, new List<string>());
                        }

                        _projectPathsByExtension[extension].Add(asset);
                    }
                }

                if ((_assetPathsByExtension == null) || (_assetPathsByExtension.Count == 0))
                {
                    _assetPathsByExtension = new Dictionary<string, List<string>>();

                    foreach (var asset in _allAssetPaths)
                    {
                        var extension = AppaPath.GetExtension(asset).CleanExtension();

                        if (string.IsNullOrWhiteSpace(extension))
                        {
                            continue;
                        }

                        if (!_assetPathsByExtension.ContainsKey(extension))
                        {
                            _assetPathsByExtension.Add(extension, new List<string>());
                        }

                        _assetPathsByExtension[extension].Add(asset);
                    }
                }
            }
        }

        #region Profiling

        private static readonly ProfilerMarker _PRF_FindAssetGuids = new(_PRF_PFX + nameof(FindAssetGuids));
        private static readonly ProfilerMarker _PRF_FindAssetPaths = new(_PRF_PFX + nameof(FindAssetPaths));

        private static readonly ProfilerMarker _PRF_FindAssetPathsByExtension =
            new(_PRF_PFX + nameof(FindAssetPathsByExtension));

        private static readonly ProfilerMarker _PRF_FindAssetPathsByFileName =
            new(_PRF_PFX + nameof(FindAssetPathsByFileName));

        private static readonly ProfilerMarker _PRF_FindAssetPathsBySubstring =
            new(_PRF_PFX + nameof(FindAssetPathsBySubstring));

        private static readonly ProfilerMarker _PRF_FindProjectPathsByExtension =
            new(_PRF_PFX + nameof(FindProjectPathsByExtension));

        private static readonly ProfilerMarker _PRF_FormatSearchString =
            new(_PRF_PFX + nameof(FormatSearchString));

        private static readonly ProfilerMarker _PRF_InitializeAssetPathData =
            new(_PRF_PFX + nameof(InitializeAssetPathData));

        private static readonly ProfilerMarker _PRF_FindFirstAsset =
            new ProfilerMarker(_PRF_PFX + nameof(FindFirstAssetMatch));

        #endregion
    }
}

#endif
