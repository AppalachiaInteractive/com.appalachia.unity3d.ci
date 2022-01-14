using System;
using System.Collections.Generic;
using System.IO;
using Appalachia.CI.Constants;
using Appalachia.CI.Integration.Extensions;
using Appalachia.Utility.Strings;
using Unity.Profiling;

namespace Appalachia.CI.Integration.FileSystem
{
    public static class AppaPath
    {
        #region Constants and Static Readonly

        private const char SLASH = '/';

        #endregion

        #region Static Fields and Autoproperties

        [NonSerialized] private static AppaContext _context;

        #endregion

        public static char DirectorySeparatorChar => Path.DirectorySeparatorChar;

        private static AppaContext Context
        {
            get
            {
                if (_context == null)
                {
                    _context = new AppaContext(typeof(AppaPath));
                }

                return _context;
            }
        }

        public static string Combine(List<string> paths)
        {
            using (_PRF_Combine.Auto())
            {
                return ZString.Join(SLASH, paths);
            }
        }

        public static string Combine(params string[] paths)
        {
            using (_PRF_Combine.Auto())
            {
                return ZString.Join(SLASH, paths);
            }
        }

        public static string Combine(string path1, string path2)
        {
            using (_PRF_Combine_Manual_2.Auto())
            {
                return ZString.Concat(path1, SLASH, path2);
            }
        }

        public static string Combine(string path1, string path2, string path3)
        {
            using (_PRF_Combine_Manual_3.Auto())
            {
                return ZString.Concat(path1, SLASH, path2, SLASH, path3);
            }
        }

        public static string Combine(string path1, string path2, string path3, string path4)
        {
            using (_PRF_Combine_Manual_4.Auto())
            {
                return ZString.Concat(path1, SLASH, path2, SLASH, path3, SLASH, path4);
            }
        }

        public static string Combine(string path1, string path2, string path3, string path4, string path5)
        {
            using (_PRF_Combine_Manual_5.Auto())
            {
                return ZString.Concat(path1, SLASH, path2, SLASH, path3, SLASH, path4, SLASH, path5);
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
                    Context.Log.Error($"{nameof(GetExtension)}: Error parsing extension! {path}");
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

        public static bool MatchesExtension(string path, StringComparison comparison, params string[] args)
        {
            using (_PRF_MatchesExtension.Auto())
            {
                var extension = GetExtension(path);

                var extensionStartsWithPeriod = extension.StartsWith('.');

                foreach (var arg in args)
                {
                    var matchStartsWithPeriod = arg.StartsWith('.');

                    if (extensionStartsWithPeriod && !matchStartsWithPeriod)
                    {
                        if (extension.Equals(ZString.Format(".{0}", arg), comparison))
                        {
                            return true;
                        }
                    }
                    else if (!extensionStartsWithPeriod && matchStartsWithPeriod)
                    {
                        if (arg.Equals(ZString.Format(".{0}", extension), comparison))
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (arg.Equals(extension, comparison))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        public static bool MatchesExtension(string path, params string[] args)
        {
            using (_PRF_MatchesExtension.Auto())
            {
                return MatchesExtension(path, StringComparison.Ordinal, args);
            }
        }

        #region Nested type: ExtensionSets

        public static class ExtensionSets
        {
            #region Static Fields and Autoproperties

            private static string[] _image;
            private static string[] _shader;
            private static string[] _shaderInclude;

            #endregion

            public static string[] Image
            {
                get
                {
                    if (_image == null)
                    {
                        _image = new[] { "png", "jpg", "jpeg", "tif", "tiff", "tga", "bmp", "psd" };
                    }

                    return _image;
                }
            }

            public static string[] Shader
            {
                get
                {
                    if (_shader == null)
                    {
                        _shader = new[] { "shader", "compute" };
                    }

                    return _shader;
                }
            }

            public static string[] ShaderInclude
            {
                get
                {
                    if (_shaderInclude == null)
                    {
                        _shaderInclude = new[] { "cginc", "hlsl", "template" };
                    }

                    return _shaderInclude;
                }
            }
        }

        #endregion

        #region Profiling

        private const string _PRF_PFX = nameof(AppaPath) + ".";

        private static readonly ProfilerMarker _PRF_Combine = new(_PRF_PFX + nameof(Combine));

        private static readonly ProfilerMarker _PRF_GetDirectoryName =
            new(_PRF_PFX + nameof(GetDirectoryName));

        private static readonly ProfilerMarker _PRF_GetExtension = new(_PRF_PFX + nameof(GetExtension));
        private static readonly ProfilerMarker _PRF_GetFileName = new(_PRF_PFX + nameof(GetFileName));

        private static readonly ProfilerMarker _PRF_GetFileNameWithoutExtension =
            new(_PRF_PFX + nameof(GetFileNameWithoutExtension));

        private static readonly ProfilerMarker _PRF_GetFullPath = new(_PRF_PFX + nameof(GetFullPath));

        private static readonly ProfilerMarker _PRF_MatchesExtension =
            new ProfilerMarker(_PRF_PFX + nameof(MatchesExtension));

        private static readonly ProfilerMarker _PRF_Combine_Manual_2 =
            new ProfilerMarker($"{_PRF_PFX}{nameof(Combine)}.Manual.2");

        private static readonly ProfilerMarker _PRF_Combine_Manual_3 =
            new ProfilerMarker($"{_PRF_PFX}{nameof(Combine)}.Manual.3");

        private static readonly ProfilerMarker _PRF_Combine_Manual_4 =
            new ProfilerMarker($"{_PRF_PFX}{nameof(Combine)}.Manual.4");

        private static readonly ProfilerMarker _PRF_Combine_Manual_5 =
            new ProfilerMarker($"{_PRF_PFX}{nameof(Combine)}.Manual.5");

        #endregion
    }
}
