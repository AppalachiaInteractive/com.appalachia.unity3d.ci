using System;
using System.Collections.Generic;
using Appalachia.CI.Integration.Cleaning;
using Appalachia.CI.Integration.Extensions;
using Appalachia.CI.Integration.FileSystem;
using Unity.Profiling;
using Object = UnityEngine.Object;

namespace Appalachia.CI.Integration.Assets
{
    public static partial class AssetDatabaseManager
    {
        #region Profiling And Tracing Markers

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

        private static readonly ProfilerMarker _PRF_CleanExtension = new(_PRF_PFX + nameof(CleanExtension));

        private static readonly ProfilerMarker _PRF_FormatSearchString =
            new(_PRF_PFX + nameof(FormatSearchString));

        private static readonly ProfilerMarker _PRF_InitializeAssetPathData =
            new(_PRF_PFX + nameof(InitializeAssetPathData));

        #endregion

        private static char[] _extensionTrims;

        private static Dictionary<string, List<string>> _assetPathsByExtension;
        private static Dictionary<string, List<string>> _projectPathsByExtension;
        private static string[] _allAssetPaths;
        private static string[] _allProjectPaths;
        private static StringCleanerWithContext<char[]> _extensionCleaner;

        public static List<Object> FindAssets(Type t, string searchString = null)
        {
            using (_PRF_FindAssets.Auto())
            {
                var paths = FindAssetPaths(t, searchString);
                var results = new List<Object>(paths.Length);

                for (var i = 0; i < paths.Length; i++)
                {
                    var path = paths[i];

                    var type = GetMainAssetTypeAtPath(path);

                    if (t.IsAssignableFrom(type))
                    {
                        var cast = LoadAssetAtPath(path, t);
                        results.Add(cast);
                    }
                }

                return results;
            }
        }

        public static List<string> FindAssetPathsByExtension(string extension)
        {
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

        public static List<string> FindProjectPathsByExtension(string extension)
        {
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

        public static List<T> FindAssets<T>(string searchString = null)
            where T : Object
        {
            using (_PRF_FindAssets.Auto())
            {
                InitializeAssetPathData();

                var t = typeof(T);

                var paths = FindAssetPaths<T>(searchString);
                var results = new List<T>(paths.Length);

                for (var i = 0; i < paths.Length; i++)
                {
                    var path = paths[i];

                    var type = GetMainAssetTypeAtPath(path);

                    if (t.IsAssignableFrom(type))
                    {
                        var cast = LoadAssetAtPath<T>(path);
                        results.Add(cast);
                    }
                }

                return results;
            }
        }

        public static string[] FindAssetGuids<T>(string searchString = null)
            where T : Object
        {
            using (_PRF_FindAssetGuids.Auto())
            {
                InitializeAssetPathData();

                return FindAssetGuids(typeof(T), searchString);
            }
        }

        public static string[] FindAssetGuids(Type t, string searchString = null)
        {
            using (_PRF_FindAssetGuids.Auto())
            {
                InitializeAssetPathData();

                var guids = FindAssets(FormatSearchString(t, searchString));

                return guids;
            }
        }

        public static string[] FindAssetPaths(string searchString = null)
        {
            using (_PRF_FindAssetPaths.Auto())
            {
                InitializeAssetPathData();

                return FindAssetPaths(null, searchString);
            }
        }

        public static string[] FindAssetPaths<T>(string searchString = null)
        {
            using (_PRF_FindAssetPaths.Auto())
            {
                InitializeAssetPathData();

                return FindAssetPaths(typeof(T), searchString);
            }
        }

        public static string[] FindAssetPaths(Type t, string searchString = null)
        {
            using (_PRF_FindAssetPaths.Auto())
            {
                InitializeAssetPathData();

                var guids = FindAssetGuids(t, searchString);
                var paths = new string[guids.Length];

                for (var index = 0; index < guids.Length; index++)
                {
                    var guid = guids[index];
                    var path = GUIDToAssetPath(guid).CleanFullPath();

                    paths[index] = path;
                }

                return paths;
            }
        }

        private static string CleanExtension(this string extension)
        {
            using (_PRF_CleanExtension.Auto())
            {
                if (_extensionCleaner == null)
                {
                    _extensionCleaner = new StringCleanerWithContext<char[]>(
                        new[] {'.', ' ', '\t', ','},
                        (cleaner, value) =>
                        {
                            var result = value.ToLowerInvariant().Trim(cleaner.context1);
                            return result;
                        }
                    );
                }

                return _extensionCleaner.Clean(extension);
            }
        }

        private static string FormatSearchString(Type t, string searchString)
        {
            using (_PRF_FormatSearchString.Auto())
            {
                if (t == null)
                {
                    return searchString;
                }

                var typename = t.Name;
                return $"t:{typename} {searchString ?? string.Empty}";
            }
        }

        private static void InitializeAssetPathData()
        {
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
    }
}
