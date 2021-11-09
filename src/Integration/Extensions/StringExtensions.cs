using System.Collections.Generic;
using Appalachia.CI.Integration.FileSystem;
using Appalachia.CI.Integration.SourceControl;
using Unity.Profiling;

namespace Appalachia.CI.Integration.Extensions
{
    public static class IntegrationStringExtensions
    {
        #region Profiling And Tracing Markers

        private const string _PRF_PFX = nameof(IntegrationStringExtensions) + ".";
        private static readonly ProfilerMarker _PRF_CleanFullPath = new(_PRF_PFX + nameof(CleanFullPath));

        private static readonly ProfilerMarker _PRF_ToAbsolutePath = new(_PRF_PFX + nameof(ToAbsolutePath));
        private static readonly ProfilerMarker _PRF_ToRelativePath = new(_PRF_PFX + nameof(ToRelativePaths));

        private static readonly ProfilerMarker _PRF_CleanPackagePath =
            new(_PRF_PFX + nameof(CleanPackagePath));

        private static readonly ProfilerMarker _PRF_InitializePathLookups =
            new(_PRF_PFX + nameof(InitializePathLookups));

        private static readonly ProfilerMarker _PRF_CleanRelativePath =
            new(_PRF_PFX + nameof(CleanRelativePath));

        private static readonly ProfilerMarker _PRF_ToRelativePaths = new(_PRF_PFX + nameof(ToRelativePaths));

        #endregion

        private static char[] _trims = {' ', '.'};

        private static Dictionary<string, string> _absoluteToRelativePathLookup;
        private static Dictionary<string, string> _relativeToAbsolutePathLookup;

        private static string[] _keys = {"\\", "//", "\0", "\a", "\b", "\f", "\n", "\r", "\t", "\v"};

        private static string[] _values = {"/", "/", "", "", "", "", "", "", "", ""};

        public static string CleanFullPath(this string path)
        {
            using (_PRF_CleanFullPath.Auto())
            {
                for (var i = 0; i < _keys.Length; i++)
                {
                    var key = _keys[i];

                    if (path.Contains(key))
                    {
                        var value = _values[i];
                        path = path.Replace(key, value);
                    }
                }

                path = path.Trim(_trims);

                if (path.StartsWith('/'))
                {
                    path = path.Substring(1);
                }

                return path;
            }
        }

        public static string CleanRelativePath(this string path)
        {
            using (_PRF_CleanRelativePath.Auto())
            {
                if (path.Contains("Library/PackageCache") || path.Contains("Library\\PackageCache"))
                {
                    path = CleanPackagePath(path);
                }

                for (var i = 0; i < _keys.Length; i++)
                {
                    var key = _keys[i];
                    var value = _values[i];

                    path = path.Replace(key, value);
                }

                path = path.Trim(_trims);

                if (path.StartsWith('/'))
                {
                    path = path.Substring(1);
                }

                return path;
            }
        }

        public static string ToAbsolutePath(this string relativePath)
        {
            using (_PRF_ToAbsolutePath.Auto())
            {
                InitializePathLookups();

                if (_relativeToAbsolutePathLookup.ContainsKey(relativePath))
                {
                    return _relativeToAbsolutePathLookup[relativePath];
                }

                var cleanRelativePath = relativePath.CleanFullPath();

                var firstSubfolder = cleanRelativePath.IndexOf('/');
                var relativePathSubstring = cleanRelativePath.Substring(firstSubfolder + 1);

                /*var basePath = ProjectLocations.GetProjectDirectoryPath();
                var absolutePath = AppaPath.Combine(basePath, relativePathSubstring);*/
                var absolutePath = AppaPath.GetFullPath(relativePathSubstring);

                _relativeToAbsolutePathLookup.Add(relativePath, absolutePath);

                if (!_absoluteToRelativePathLookup.ContainsKey(absolutePath))
                {
                    _absoluteToRelativePathLookup.Add(absolutePath, relativePath);
                }

                return absolutePath;
            }
        }

        public static string ToRelativePath(this string absolutePath)
        {
            using (_PRF_ToRelativePath.Auto())
            {
                InitializePathLookups();

                if (_absoluteToRelativePathLookup.ContainsKey(absolutePath))
                {
                    return _absoluteToRelativePathLookup[absolutePath];
                }

                var cleanAbsolutePath = absolutePath.CleanFullPath();

                var basePath = ProjectLocations.GetProjectDirectoryPath();

                var trimmedPath = cleanAbsolutePath.Replace(basePath, string.Empty);
                var relativePath = trimmedPath.CleanRelativePath();

                _absoluteToRelativePathLookup.Add(absolutePath, relativePath);

                if (!_relativeToAbsolutePathLookup.ContainsKey(relativePath))
                {
                    _relativeToAbsolutePathLookup.Add(relativePath, absolutePath);
                }

                return relativePath;
            }
        }

        public static string[] ToRelativePaths(this string[] paths)
        {
            using (_PRF_ToRelativePaths.Auto())
            {
                var results = new string[paths.Length];

                for (var index = 0; index < paths.Length; index++)
                {
                    var path = paths[index];

                    results[index] = path.ToRelativePath();
                }

                return results;
            }
        }

        internal static bool IsPathIgnored(this string path, IgnoreFile ignoreFile)
        {
            if (ignoreFile == null)
            {
                return false;
            }

            return ignoreFile.IsIgnored(path);
        }

        private static string CleanPackagePath(string path)
        {
            using (_PRF_CleanPackagePath.Auto())
            {
                path = path.Replace("Library/PackageCache", "Packages");

                var indexOfAt = path.IndexOf('@');

                if (indexOfAt <= 0)
                {
                    return path;
                }

                var start = path.Substring(0, indexOfAt);
                var end = path.Substring(indexOfAt + 1);

                var nextFolderStart = end.IndexOf('/');

                if (nextFolderStart == -1)
                {
                    return start;
                }

                end = end.Substring(nextFolderStart + 1);

                var final = AppaPath.Combine(start, end);

                return final;
            }
        }

        private static void InitializePathLookups()
        {
            using (_PRF_InitializePathLookups.Auto())
            {
                if (_relativeToAbsolutePathLookup == null)
                {
                    _relativeToAbsolutePathLookup = new Dictionary<string, string>();
                }

                if (_absoluteToRelativePathLookup == null)
                {
                    _absoluteToRelativePathLookup = new Dictionary<string, string>();
                }
            }
        }
    }
}
