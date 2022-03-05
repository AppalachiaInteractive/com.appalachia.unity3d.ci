#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
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
        public static string[] FindAssetGuids<T>(string searchString = null)
            where T : Object
        {
            ThrowIfInvalidState();
            using (_PRF_FindAssetGuids.Auto())
            {
                return FindAssetGuids(typeof(T), searchString);
            }
        }

        public static string[] FindAssetGuids(Type t, string searchString = null)
        {
            ThrowIfInvalidState();
            using (_PRF_FindAssetGuids.Auto())
            {
                var guids = FindAssets(FormatSearchString(t, searchString));

                return guids;
            }
        }

        public static List<AssetPath> FindAssetPaths(string searchString = null)
        {
            ThrowIfInvalidState();
            using (_PRF_FindAssetPaths.Auto())
            {
                return FindAssetPaths(null, searchString);
            }
        }

        public static List<AssetPath> FindAssetPaths<T>(string searchString = null)
        {
            ThrowIfInvalidState();
            using (_PRF_FindAssetPaths.Auto())
            {
                return FindAssetPaths(typeof(T), searchString);
            }
        }

        public static List<AssetPath> FindAssetPaths(Type t, string searchString = null)
        {
            ThrowIfInvalidState();
            using (_PRF_FindAssetPaths.Auto())
            {
                var guids = FindAssetGuids(t, searchString);
                var paths = new List<AssetPath>(guids.Length);

                for (var index = 0; index < guids.Length; index++)
                {
                    var guid = guids[index];
                    var path = GUIDToAssetPath(guid);

                    if (!path.FileExists)
                    {
                        continue;
                    }

                    paths.Add(path);
                }

                return paths;
            }
        }

        public static List<AssetPath> FindAssetPathsByExtension(string extension)
        {
            ThrowIfInvalidState();
            using (_PRF_FindAssetPathsByExtension.Auto())
            {
                var cleanExtension = extension.CleanExtension();

                if (AssetPathsByExtension.TryGetValue(cleanExtension, out var result))
                {
                    return result;
                }

                return null;
            }
        }

        public static List<AssetPath> FindAssetPathsByFileName(string name)
        {
            ThrowIfInvalidState();
            using (_PRF_FindAssetPathsByFileName.Auto())
            {
                var output = new List<AssetPath>();

                foreach (var path in AllAssetPaths)
                {
                    if (name == path.FileName)
                    {
                        output.Add(path);
                    }
                }

                return output;
            }
        }

        public static List<AssetPath> FindAssetPathsBySubstring(string substring)
        {
            ThrowIfInvalidState();
            using (_PRF_FindAssetPathsBySubstring.Auto())
            {
                var output = new List<AssetPath>();

                foreach (var path in AllAssetPaths)
                {
                    if (path.AbsolutePath.Contains(substring))
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

                    var type = GetMainAssetTypeAtPath(path.RelativePath);

                    if (t.ImplementsOrInheritsFrom(type))
                    {
                        var cast = LoadAssetAtPath(path.RelativePath, t);
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

                Object result;

                if (searchString != null)
                {
                    if (_firstLookupCache[t].TryGetValue(searchString, out result))
                    {
                        return result;
                    }
                }

                var results = FindAssets(t, searchString);
                var sortedResults = results.OrderByDescending(v => v.name == searchString ? 1 : 0);
                result = sortedResults.FirstOrDefault();

                if (searchString != null)
                {
                    _firstLookupCache[t].Add(searchString, result);
                }

                return result;
            }
        }

        public static List<AssetPath> FindProjectPathsByExtension(string extension)
        {
            ThrowIfInvalidState();
            using (_PRF_FindProjectPathsByExtension.Auto())
            {
                var cleanExtension = extension.CleanExtension();

                if (ProjectPathsByExtension.TryGetValue(cleanExtension, out var result))
                {
                    return result;
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

        private static readonly ProfilerMarker _PRF_FindFirstAsset =
            new ProfilerMarker(_PRF_PFX + nameof(FindFirstAssetMatch));

        #endregion
    }
}

#endif
