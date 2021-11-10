using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Appalachia.CI.Integration.FileSystem;
using Unity.Profiling;

namespace Appalachia.CI.Integration.Development
{
    public class ProjectCache
    {
        #region Profiling And Tracing Markers

        private const string _PRF_PFX = nameof(ProjectCache) + ".";

        private static readonly ProfilerMarker _PRF_ClearPackageCache =
            new ProfilerMarker(_PRF_PFX + nameof(ClearPackageCache));

        private static readonly ProfilerMarker _PRF_GetPackageCacheDirectory =
            new ProfilerMarker(_PRF_PFX + nameof(GetPackageCacheDirectory));

        private static readonly ProfilerMarker _PRF_GetProjectCache =
            new ProfilerMarker(_PRF_PFX + nameof(GetProjectCache));

        private static readonly ProfilerMarker _PRF_GetSpecificPackageCacheDirectory =
            new ProfilerMarker(_PRF_PFX + nameof(GetSpecificPackageCacheDirectory));

        #endregion

        private ProjectCache()
        {
            _packages = new List<Package>();
        }

        private static AppaDirectoryInfo _packageCacheDirectory;
        private static ProjectCache _instance;

        private readonly List<Package> _packages;

        public IReadOnlyList<Package> packages => _packages;

        public static void ResetInstance()
        {
            _instance = null;
        }

        public static void ClearPackageCache()
        {
            using (_PRF_ClearPackageCache.Auto())
            {
                var packageCacheDirectory = GetPackageCacheDirectory();

                if (!packageCacheDirectory.Exists)
                {
                    return;
                }
                
                foreach (var child in packageCacheDirectory.GetDirectories())
                {
                    child.DeleteRecursively();
                }
                
                ResetInstance();
            }
        }

        public static AppaDirectoryInfo GetPackageCacheDirectory()
        {
            using (_PRF_GetPackageCacheDirectory.Auto())
            {
                if (_packageCacheDirectory != null)
                {
                    return _packageCacheDirectory;
                }

                var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var registryUrl = "35.211.123.13:4873";
                var registrySubpath = registryUrl.Replace(":", "_");

                var packageCachePath = AppaPath.Combine(
                    appDataPath,
                    "Unity",
                    "cache",
                    "packages",
                    registrySubpath
                );

                _packageCacheDirectory = new AppaDirectoryInfo(packageCachePath);

                return _packageCacheDirectory;
            }
        }

        public static ProjectCache GetProjectCache()
        {
            using (_PRF_GetProjectCache.Auto())
            {
                if (_instance != null)
                {
                    return _instance;
                }

                _instance = new ProjectCache();

                Package currentPackage = null;

                //var fields = typeof(Package).GetFields().Select(f => f.Name).ToArray();

                using (var stream = AppaFile.OpenRead("Library/PackageManager/ProjectCache"))
                using (var streamReader = new StreamReader(stream))
                {
                    while (!streamReader.EndOfStream)
                    {
                        var line = streamReader.ReadLine();

                        if (line.StartsWith("- packageId:"))
                        {
                            if (currentPackage != null)
                            {
                                _instance._packages.Add(currentPackage);
                            }

                            currentPackage = new Package();
                        }

                        if (currentPackage == null)
                        {
                            continue;
                        }

                        currentPackage.Parse(line);
                    }

                    if (currentPackage != null)
                    {
                        _instance._packages.Add(currentPackage);
                    }
                }

                return _instance;
            }
        }

        public static AppaDirectoryInfo GetSpecificPackageCacheDirectory(string package, string version)
        {
            using (_PRF_GetSpecificPackageCacheDirectory.Auto())
            {
                var dir = GetPackageCacheDirectory();

                var subpath = $"{package}@{version}";

                var packagePath = AppaPath.Combine(dir.FullPath, subpath);
                return new AppaDirectoryInfo(packagePath);
            }
        }

        #region Nested Types

        public class Package
        {
            #region Profiling And Tracing Markers

            private const string _PRF_PFX = nameof(Package) + ".";

            private static readonly ProfilerMarker _PRF_Parse = new ProfilerMarker(_PRF_PFX + nameof(Parse));

            #endregion

            private static char[] _trimCharacters = {'-', ' ', ':'};
            public int? hasRegistry;         // 1
            public int? hasRepository;       // 1
            public int? hideInEditor;        // 1
            public int? isAssetStorePackage; // 0
            public int? isDirectDependency;  // 1
            public int? source;              // 1
            public int? testable;            // 0
            public long? datePublishedTicks; // 637697229110000000
            public string assetPath;         // Packages/com.unity.burst
            public string category;          //
            public string displayName;       // Burst
            public string documentationUrl;  //
            public string name;              // com.unity.burst
            public string packageId;         // com.unity.burst@1.6.1

            public string
                resolvedPath; // C:\Users\Chris\com.appalachia\unity3d\appa\Library\PackageCache\com.unity.burst@1.6.1

            public string type;    //
            public string version; // 1.6.1

            private DateTime? _datePublished;

            public DateTime? datePublished
            {
                get
                {
                    if (_datePublished.HasValue)
                    {
                        return _datePublished;
                    }

                    if (datePublishedTicks.HasValue)
                    {
                        _datePublished = new DateTime(datePublishedTicks.Value, DateTimeKind.Utc);
                    }

                    return _datePublished;
                }
            }

            [DebuggerStepThrough]
            public override string ToString()
            {
                return packageId;
            }

            public void Parse(string line)
            {
                using (_PRF_Parse.Auto())
                {
                    var delimiterIndex = line.IndexOf(':');

                    if (delimiterIndex < 0)
                    {
                        return;
                    }

                    var firstCharacter = line[0];
                    if (char.IsLetterOrDigit(firstCharacter))
                    {
                        return;
                    }

                    var fourthCharacter = line[3];
                    if (char.IsWhiteSpace(fourthCharacter))
                    {
                        return;
                    }

                    var field = line[..delimiterIndex].Trim(_trimCharacters);
                    var value = line[(delimiterIndex + 1)..].Trim(_trimCharacters);

                    if (field == nameof(hasRegistry))
                    {
                        hasRegistry = ParseHasRegistry(value);
                    }
                    else if (field == nameof(hasRepository))
                    {
                        hasRepository = ParseHasRepository(value);
                    }
                    else if (field == nameof(hideInEditor))
                    {
                        hideInEditor = ParseHideInEditor(value);
                    }
                    else if (field == nameof(isAssetStorePackage))
                    {
                        isAssetStorePackage = ParseIsAssetStorePackage(value);
                    }
                    else if (field == nameof(isDirectDependency))
                    {
                        isDirectDependency = ParseIsDirectDependency(value);
                    }
                    else if (field == nameof(source))
                    {
                        source = ParseSource(value);
                    }
                    else if (field == nameof(testable))
                    {
                        testable = ParseTestable(value);
                    }
                    else if (field == nameof(assetPath))
                    {
                        assetPath = ParseAssetPath(value);
                    }
                    else if (field == nameof(category))
                    {
                        category = ParseCategory(value);
                    }
                    else if (field == nameof(datePublishedTicks))
                    {
                        datePublishedTicks = ParseDatePublishedTicks(value);
                    }
                    else if (field == nameof(displayName))
                    {
                        displayName = ParseDisplayName(value);
                    }
                    else if (field == nameof(documentationUrl))
                    {
                        documentationUrl = ParseDocumentationUrl(value);
                    }
                    else if (field == nameof(name))
                    {
                        name = ParseName(value);
                    }
                    else if (field == nameof(packageId))
                    {
                        packageId = ParsePackageId(value);
                    }
                    else if (field == nameof(resolvedPath))
                    {
                        resolvedPath = ParseResolvedPath(value);
                    }
                    else if (field == nameof(type))
                    {
                        type = ParseType(value);
                    }
                    else if (field == nameof(version))
                    {
                        version = ParseVersion(value);
                    }
                }
            }

            private string ParseAssetPath(string value)
            {
                return value;
            }

            private string ParseCategory(string value)
            {
                return value;
            }

            private long? ParseDatePublishedTicks(string value)
            {
                if (value == null)
                {
                    return null;
                }

                if (long.TryParse(value, out var parsedValue))
                {
                    return parsedValue;
                }

                return null;
            }

            private string ParseDisplayName(string value)
            {
                return value;
            }

            private string ParseDocumentationUrl(string value)
            {
                return value;
            }

            private int? ParseHasRegistry(string value)
            {
                if (value == null)
                {
                    return null;
                }

                if (int.TryParse(value, out var parsedValue))
                {
                    return parsedValue;
                }

                return null;
            }

            private int? ParseHasRepository(string value)
            {
                if (value == null)
                {
                    return null;
                }

                if (int.TryParse(value, out var parsedValue))
                {
                    return parsedValue;
                }

                return null;
            }

            private int? ParseHideInEditor(string value)
            {
                if (value == null)
                {
                    return null;
                }

                if (int.TryParse(value, out var parsedValue))
                {
                    return parsedValue;
                }

                return null;
            }

            private int? ParseIsAssetStorePackage(string value)
            {
                if (value == null)
                {
                    return null;
                }

                if (int.TryParse(value, out var parsedValue))
                {
                    return parsedValue;
                }

                return null;
            }

            private int? ParseIsDirectDependency(string value)
            {
                if (value == null)
                {
                    return null;
                }

                if (int.TryParse(value, out var parsedValue))
                {
                    return parsedValue;
                }

                return null;
            }

            private string ParseName(string value)
            {
                return value;
            }

            private string ParsePackageId(string value)
            {
                return value;
            }

            private string ParseResolvedPath(string value)
            {
                return value;
            }

            private int? ParseSource(string value)
            {
                if (value == null)
                {
                    return null;
                }

                if (int.TryParse(value, out var parsedValue))
                {
                    return parsedValue;
                }

                return null;
            }

            private int? ParseTestable(string value)
            {
                if (value == null)
                {
                    return null;
                }

                if (int.TryParse(value, out var parsedValue))
                {
                    return parsedValue;
                }

                return null;
            }

            private string ParseType(string value)
            {
                return value;
            }

            private string ParseVersion(string value)
            {
                return value;
            }
        }

        #endregion
    }
}
