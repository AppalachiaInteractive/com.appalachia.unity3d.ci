using System;
using System.Collections.Generic;
using Appalachia.CI.Integration.Assets;
using Appalachia.CI.Integration.FileSystem;
using Appalachia.Utility.Extensions;
using Unity.Profiling;

namespace Appalachia.CI.Integration.Extensions
{
    public static class IntegrationStringExtensions
    {
        #region Constants and Static Readonly

        private static readonly (char find, char replace)[] CharReplacements =
        {
            ('\\', '/'),
            ('\0', default),
            ('\a', default),
            ('\b', default),
            ('\f', default),
            ('\n', default),
            ('\r', default),
            ('\t', default),
            ('\v', default),
        };

        private static readonly (string find, string replace)[] StringReplacements = { ("//", "/") };

        private const char AT = '@';
        private const char SLASH = '/';
        private static readonly char[] Trims = { ' ', '.' };

        private const string LIBRARY_PACKAGE_CACHE = "Library/PackageCache";
        private const string LIBRARY_PACKAGE_CACHE_BACK = "Library\\PackageCache";
        private const string PACKAGES = "Packages";

        #endregion

        public static void ClearCachedData()
        {
            _toRelativePathLookup.Clear();
            _cleanPathLookup.Clear();
            _toAbsolutePathLookup.Clear();
        }

        #region Static Fields and Autoproperties

        [NonSerialized] private static Dictionary<string, string> _toRelativePathLookup;
        [NonSerialized] private static Dictionary<string, string> _cleanPathLookup;
        [NonSerialized] private static Dictionary<string, string> _toAbsolutePathLookup;

        #endregion

        public static string CleanFullPath(this string path)
        {
            using (_PRF_CleanFullPath.Auto())
            {
                if (path.IsNullOrWhiteSpace())
                {
                    return string.Empty;
                }

                using var set = new StringCleaningSet();

                set.Load(path);

                CleanFullPath(set);

                return set.IsFinished ? set.Result : set.Finish();
            }
        }

        public static void CleanFullPath(StringCleaningSet set)
        {
            using (_PRF_CleanFullPath.Auto())
            {
                CleanPath(set);
            }
        }

        public static string CleanRelativePath(this string path)
        {
            using (_PRF_CleanRelativePath.Auto())
            {
                if (path.IsNullOrWhiteSpace())
                {
                    return string.Empty;
                }

                using var set = new StringCleaningSet();

                set.Load(path);

                CleanRelativePath(set);

                return set.IsFinished ? set.Result : set.Finish();
            }
        }

        public static void CleanRelativePath(StringCleaningSet set)
        {
            using (_PRF_CleanRelativePath.Auto())
            {
                if (set.input.IsPackagePath())
                {
                    CleanPackagePath(set);
                }

                while ((set[0] == '/') || (set[0] == '\\'))
                {
                    set.Remove(0, 1);
                }

                CleanPath(set);
            }
        }

        public static string ToAbsolutePath(this string relativePath)
        {
            using (_PRF_ToAbsolutePath.Auto())
            {
                if (relativePath.IsNullOrWhiteSpace())
                {
                    return string.Empty;
                }

                using var set = new StringCleaningSet();

                set.Load(relativePath);

                CleanRelativePath(set);

                return set.IsFinished ? set.Result : set.Finish();
            }
        }

        public static void ToAbsolutePath(this StringCleaningSet set)
        {
            using (_PRF_ToAbsolutePath.Auto())
            {
                if (HasToAbsolutePathResult(set))
                {
                    return;
                }

                CleanFullPath(set);

                var result = set.Peek();

                var absolutePath = AppaPath.GetFullPath(result);

                set.SetResult(absolutePath);

                _toAbsolutePathLookup.Add(set.input, set.Result);
            }
        }

        public static void ToRelativePath(StringCleaningSet set)
        {
            using (_PRF_ToRelativePath.Auto())
            {
                if (HasToRelativePathResult(set))
                {
                    return;
                }

                CleanFullPath(set);

                var basePath = ProjectLocations.GetProjectDirectoryPath();

                set.Replace(basePath, string.Empty);

                CleanRelativePath(set);

                var relativePath = set.Peek();

                _toRelativePathLookup.Add(set.input, relativePath);
            }
        }

        public static string ToRelativePath(this string absolutePath)
        {
            using (_PRF_ToRelativePath.Auto())
            {
                if (absolutePath.IsNullOrWhiteSpace())
                {
                    return string.Empty;
                }

                using var set = new StringCleaningSet();

                set.Load(absolutePath);

                ToRelativePath(set);

                return set.IsFinished ? set.Result : set.Finish();
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

        private static void CleanPackagePath(StringCleaningSet set)
        {
            using (_PRF_CleanPackagePath.Auto())
            {
                set.Replace(LIBRARY_PACKAGE_CACHE, PACKAGES);

                var trimStart = -1;
                var trimEnd = -1;

                for (var i = 0; i < set.Length; i++)
                {
                    var character = set[i];

                    if ((trimStart == -1) && (character == AT))
                    {
                        trimStart = i;
                    }
                    else if ((trimStart != -1) && (character == SLASH))
                    {
                        trimEnd = i;
                        break;
                    }
                }

                if ((trimStart == -1) || (trimEnd == -1))
                {
                    return;
                }

                var length = trimEnd - trimStart - 1;

                set.Remove(trimStart, length);
            }
        }

        private static void CleanPath(StringCleaningSet set)
        {
            using (_PRF_CleanPath.Auto())
            {
                if (HasCleanPathResult(set))
                {
                    return;
                }

                var startValue = set.Peek();

                for (var replacementIndex = 0; replacementIndex < CharReplacements.Length; replacementIndex++)
                {
                    var charReplacement = CharReplacements[replacementIndex];

                    if (charReplacement.replace == default)
                    {
                        for (var charIndex = set.Length - 1; charIndex >= 0; charIndex--)
                        {
                            var current = set[charIndex];

                            if (current == charReplacement.replace)
                            {
                                set.Remove(charIndex, 1);
                            }
                        }
                    }
                    else
                    {
                        set.Replace(charReplacement.find, charReplacement.replace);
                    }
                }

                for (var i = 0; i < StringReplacements.Length; i++)
                {
                    var stringReplacement = StringReplacements[i];

                    set.Replace(stringReplacement.find, stringReplacement.replace);
                }

                for (var i = 0; i < Trims.Length; i++)
                {
                    var trim = Trims[i];

                    if (set[0] == trim)
                    {
                        set.Remove(0, 1);
                    }

                    if (set[set.Length - 1] == trim)
                    {
                        set.Remove(set.Length - 2, 1);
                    }
                }

                if (set[0] == SLASH)
                {
                    set.Remove(0, 1);
                }

                var result = set.Peek();

                _cleanPathLookup.Add(startValue, result);
            }
        }

        private static bool HasToRelativePathResult(StringCleaningSet set)
        {
            using (_PRF_HasPreviouslyCachedAbsolutePath.Auto())
            {
                _toRelativePathLookup ??= new Dictionary<string, string>();

                if (!_toRelativePathLookup.ContainsKey(set.input))
                {
                    return false;
                }

                var cachedResult = _toRelativePathLookup[set.input];
                set.SetResult(cachedResult);

                return true;
            }
        }

        private static bool HasCleanPathResult(StringCleaningSet set)
        {
            using (_PRF_HasPreviouslyCachedCleanPath.Auto())
            {
                var setValue = set.Peek();

                _cleanPathLookup ??= new Dictionary<string, string>();

                if (!_cleanPathLookup.ContainsKey(setValue))
                {
                    return false;
                }

                var cachedResult = _cleanPathLookup[setValue];
                set.SetResult(cachedResult);

                return true;
            }
        }

        private static readonly ProfilerMarker _PRF_HasRelativeToAbsolutePathResult =
            new ProfilerMarker(_PRF_PFX + nameof(HasToAbsolutePathResult));

        private static bool HasToAbsolutePathResult(StringCleaningSet set)
        {
            using (_PRF_HasRelativeToAbsolutePathResult.Auto())
            {
                _toAbsolutePathLookup ??= new Dictionary<string, string>();

                if (!_toAbsolutePathLookup.ContainsKey(set.input))
                {
                    return false;
                }

                var cachedResult = _toAbsolutePathLookup[set.input];
                set.SetResult(cachedResult);

                return true;
            }
        }

        #region Profiling

        private const string _PRF_PFX = nameof(IntegrationStringExtensions) + ".";

        private static readonly ProfilerMarker _PRF_HasPreviouslyCachedAbsolutePath =
            new ProfilerMarker(_PRF_PFX + nameof(HasToRelativePathResult));

        private static readonly ProfilerMarker _PRF_HasPreviouslyCachedRelativePath =
            new ProfilerMarker(_PRF_PFX + nameof(HasToAbsolutePathResult));

        private static readonly ProfilerMarker _PRF_HasPreviouslyCachedCleanPath =
            new ProfilerMarker(_PRF_PFX + nameof(HasCleanPathResult));

        private static readonly ProfilerMarker _PRF_CleanPath =
            new ProfilerMarker(_PRF_PFX + nameof(CleanPath));

        private static readonly ProfilerMarker _PRF_CleanFullPath = new(_PRF_PFX + nameof(CleanFullPath));

        private static readonly ProfilerMarker _PRF_ToAbsolutePath = new(_PRF_PFX + nameof(ToAbsolutePath));
        private static readonly ProfilerMarker _PRF_ToRelativePath = new(_PRF_PFX + nameof(ToRelativePaths));
        private static readonly ProfilerMarker _PRF_ToRelativePaths = new(_PRF_PFX + nameof(ToRelativePaths));

        private static readonly ProfilerMarker _PRF_CleanPackagePath =
            new(_PRF_PFX + nameof(CleanPackagePath));

        private static readonly ProfilerMarker _PRF_CleanRelativePath =
            new(_PRF_PFX + nameof(CleanRelativePath));

        #endregion
    }
}
