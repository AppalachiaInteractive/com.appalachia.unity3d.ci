using System.Collections.Generic;
using System.IO;
using Appalachia.CI.Integration.Cleaning;
using Appalachia.CI.Integration.Extensions;
using Appalachia.Utility.Extensions;
using Appalachia.Utility.Logging;
using Unity.Profiling;

namespace Appalachia.CI.Integration.FileSystem
{
    public static class AppaPath
    {
        #region Profiling And Tracing Markers

        private const string _PRF_PFX = nameof(AppaPath) + ".";

        private static readonly ProfilerMarker _PRF_Combine = new(_PRF_PFX + nameof(Combine));

        private static readonly ProfilerMarker _PRF_GetDirectoryName =
            new(_PRF_PFX + nameof(GetDirectoryName));

        private static readonly ProfilerMarker _PRF_GetExtension = new(_PRF_PFX + nameof(GetExtension));

        private static readonly ProfilerMarker _PRF_GetFileName = new(_PRF_PFX + nameof(GetFileName));

        private static readonly ProfilerMarker _PRF_GetFileNameWithoutExtension =
            new(_PRF_PFX + nameof(GetFileNameWithoutExtension));

        private static readonly ProfilerMarker _PRF_GetFullPath = new(_PRF_PFX + nameof(GetFullPath));

        #endregion

        private static StringCombiner _pathCombiner;
        public static char DirectorySeparatorChar => Path.DirectorySeparatorChar;

        public static string Combine(List<string> paths)
        {
            using (_PRF_Combine.Auto())
            {
                var internalArray = paths.ToArray();
                return Path.Combine(internalArray);
            }
        }
        
        public static string Combine(params string[] paths)
        {
            using (_PRF_Combine.Auto())
            {
                return Path.Combine(paths);
            }
        }

        public static string GetDirectoryName(string path, bool cleanPath = true)
        {
            using (_PRF_GetDirectoryName.Auto())
            {
                var result = Path.GetDirectoryName(path);

                if (cleanPath)
                {
                    result = result.CleanFullPath();
                }

                return result;
            }
        }

        public static string GetExtension(string path)
        {
            using (_PRF_GetExtension.Auto())
            {
                try
                {
                    return Path.GetExtension(path);
                }
                catch
                {
                    AppaLog.Error(nameof(GetExtension) + ": Error parsing extension! " + path);
                    throw;
                }
            }
        }

        public static string GetFileName(string path)
        {
            using (_PRF_GetFileName.Auto())
            {
                return Path.GetFileName(path);
            }
        }

        public static string GetFileNameWithoutExtension(string path)
        {
            using (_PRF_GetFileNameWithoutExtension.Auto())
            {
                return Path.GetFileNameWithoutExtension(path);
            }
        }

        public static string GetFullPath(string path, bool cleanPath = true)
        {
            using (_PRF_GetFullPath.Auto())
            {
                var result = Path.GetFullPath(path);

                if (cleanPath)
                {
                    result = result.CleanFullPath();
                }

                return result;
            }
        }
    }
}
