using System;
using System.Collections.Generic;
using Appalachia.CI.Integration.Extensions;
using Appalachia.CI.Integration.FileSystem;
using Object = UnityEngine.Object;

namespace Appalachia.CI.Integration.Assets
{
    public static partial class AssetDatabaseManager
    {
        private static Dictionary<string, List<string>> _assetPathsByExtension;
        private static string[] _allAssetPaths;

        public static string[] FindAssetGuids<T>(string searchString = null)
            where T : Object
        {
            InitializeAssetPathData();

            return FindAssetGuids(typeof(T), searchString);
        }

        public static string[] FindAssetGuids(Type t, string searchString = null)
        {
            InitializeAssetPathData();

            var guids = FindAssets(FormatSearchString(t, searchString));

            return guids;
        }

        public static string[] FindAssetPaths(string searchString = null)
        {
            InitializeAssetPathData();

            return FindAssetPaths(null, searchString);
        }

        public static string[] FindAssetPaths<T>(string searchString = null)
        {
            InitializeAssetPathData();

            return FindAssetPaths(typeof(T), searchString);
        }

        public static string[] FindAssetPaths(Type t, string searchString = null)
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

        public static List<string> FindAssetPathsByExtension(string extension)
        {
            InitializeAssetPathData();

            var cleanExtension = extension.CleanExtension();

            if (_assetPathsByExtension.ContainsKey(cleanExtension))
            {
                return _assetPathsByExtension[cleanExtension];
            }

            return null;
        }

        public static List<string> FindAssetPathsByFileName(string name)
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

        public static List<string> FindAssetPathsBySubstring(string substring)
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

        public static List<Object> FindAssets(Type t, string searchString = null)
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

        public static List<T> FindAssets<T>(string searchString = null)
            where T : Object
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

        private static string CleanExtension(this string extension)
        {
            return extension?.ToLower().Trim().Trim('.');
        }

        private static string FormatSearchString(Type t, string searchString)
        {
            if (t == null)
            {
                return searchString;
            }

            var typename = t.Name;
            return $"t:{typename} {searchString ?? string.Empty}";
        }

        private static void InitializeAssetPathData()
        {
            if ((_allAssetPaths == null) || (_allAssetPaths.Length == 0))
            {
                _allAssetPaths = GetAllAssetPaths();

                _assetPathsByExtension = null;
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
