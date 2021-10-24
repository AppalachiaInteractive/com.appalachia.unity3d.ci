using System.Collections.Generic;
using System.Text;
using Appalachia.CI.Integration.FileSystem;
using Unity.Profiling;

namespace Appalachia.CI.Integration.Extensions
{
    public static class StringExtensions
    {
        public enum EncodingPrefix
        {
            UnicodeDefault,
            Underscore
        }

#region Profiling And Tracing Markers

        private const string _PRF_PFX = nameof(StringExtensions) + ".";
        private static Dictionary<string, string> _absoluteToRelativePathLookup;
        private static Dictionary<string, string> _relativeToAbsolutePathLookup;
        private static readonly ProfilerMarker _PRF_CleanFullPath = new(_PRF_PFX + nameof(CleanFullPath));

        private static readonly ProfilerMarker _PRF_ToAbsolutePath = new(_PRF_PFX + nameof(ToAbsolutePath));
        private static readonly ProfilerMarker _PRF_ToRelativePath = new(_PRF_PFX + nameof(ToRelativePath));
        private static Dictionary<char, string> _encodingReplacements;
        private static Dictionary<EncodingPrefix, string> _encodingPrefixes;

        private static readonly ProfilerMarker _PRF_SetupEncodingReplacements =
            new(_PRF_PFX + nameof(SetupEncodingReplacements));

        private static readonly ProfilerMarker _PRF_EncodeUnicodePathToASCII =
            new(_PRF_PFX + nameof(EncodeUnicodePathToASCII));

#endregion

        public static string CleanFullPath(this string path)
        {
            using (_PRF_CleanFullPath.Auto())
            {
                return path.Replace("\\", "/")
                           .Replace("//", "/")
                           .Replace("//", "/")
                           .Trim('/')
                           .Trim(' ')
                           .Trim('.')
                           .Trim();
            }
        }

        public static string EncodeUnicodePathToASCII(this string path, EncodingPrefix prefix)
        {
            using (_PRF_EncodeUnicodePathToASCII.Auto())
            {
                SetupEncodingReplacements();

                var prefixString = _encodingPrefixes[prefix];

                var builder = new StringBuilder();

                for (var i = 0; i < path.Length; i++)
                {
                    var character = path[i];

                    if (_encodingReplacements.ContainsKey(character))
                    {
                        var replacementSuffix = _encodingReplacements[character];
                        var replacement = $"{prefixString}{replacementSuffix}";

                        builder.Append(replacement);
                    }
                    else
                    {
                        builder.Append(character);
                    }
                }

                return builder.ToString();
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

                var basePath = ProjectLocations.GetProjectDirectoryPath();

                var firstSubfolder = cleanRelativePath.IndexOf('/');
                var relativePathSubstring = cleanRelativePath.Substring(firstSubfolder + 1);

                var absolutePath =
                    AppaPath.Combine(basePath, relativePathSubstring).Trim('.', '/', '\\', ' ');

                _absoluteToRelativePathLookup.Add(absolutePath, relativePath);
                _relativeToAbsolutePathLookup.Add(relativePath, absolutePath);

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

                var relativePath =
                    cleanAbsolutePath.Replace(basePath, string.Empty).Trim('.', '/', '\\', ' ');

                _absoluteToRelativePathLookup.Add(absolutePath, relativePath);
                _relativeToAbsolutePathLookup.Add(relativePath, absolutePath);

                return relativePath;
            }
        }

        public static string[] ToRelativePath(this string[] paths)
        {
            var results = new string[paths.Length];

            for (var index = 0; index < paths.Length; index++)
            {
                var path = paths[index];

                results[index] = path.ToRelativePath();
            }

            return results;
        }

        private static void InitializePathLookups()
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

        private static void SetupEncodingReplacements()
        {
            using (_PRF_SetupEncodingReplacements.Auto())
            {
                if ((_encodingReplacements != null) && (_encodingReplacements.Count > 0))
                {
                    return;
                }

                _encodingPrefixes = new Dictionary<EncodingPrefix, string>();

                _encodingPrefixes.Add(EncodingPrefix.Underscore,     "_");
                _encodingPrefixes.Add(EncodingPrefix.UnicodeDefault, "U+");

                _encodingReplacements = new Dictionary<char, string>();

                _encodingReplacements.Add(' ',  "0020"); // Space
                _encodingReplacements.Add('!',  "0021"); // Exclamation mark
                _encodingReplacements.Add('"',  "0022"); // Quotation mark
                _encodingReplacements.Add('#',  "0023"); // Number sign, Hash, Octothorpe, Sharp
                _encodingReplacements.Add('$',  "0024"); // Dollar sign
                _encodingReplacements.Add('%',  "0025"); // Percent sign
                _encodingReplacements.Add('&',  "0026"); // Ampersand
                _encodingReplacements.Add('\'', "0027"); // Apostrophe
                _encodingReplacements.Add('(',  "0028"); // Left parenthesis
                _encodingReplacements.Add(')',  "0029"); // Right parenthesis
                _encodingReplacements.Add('*',  "002A"); // Asterisk
                _encodingReplacements.Add('+',  "002B"); // Plus sign
                _encodingReplacements.Add(',',  "002C"); // Comma
                _encodingReplacements.Add('-',  "002D"); // Hyphen-minus
                _encodingReplacements.Add('.',  "002E"); // Full stop
                _encodingReplacements.Add('/',  "002F"); // Slash (Solidus)
                _encodingReplacements.Add(':',  "003A"); // Colon
                _encodingReplacements.Add(';',  "003B"); // Semicolon
                _encodingReplacements.Add('<',  "003C"); // Less-than sign
                _encodingReplacements.Add('=',  "003D"); // Equal sign
                _encodingReplacements.Add('>',  "003E"); // Greater-than sign
                _encodingReplacements.Add('?',  "003F"); // Question mark
                _encodingReplacements.Add('@',  "0040"); // At sign
                _encodingReplacements.Add('[',  "005B"); // Left Square Bracket
                _encodingReplacements.Add('\\', "005C"); // Backslash
                _encodingReplacements.Add(']',  "005D"); // Right Square Bracket
                _encodingReplacements.Add('^',  "005E"); // Circumflex accent
                _encodingReplacements.Add('_',  "005F"); // Low line
                _encodingReplacements.Add('`',  "0060"); // Grave accent
                _encodingReplacements.Add('{',  "007B"); // Left Curly Bracket
                _encodingReplacements.Add('|',  "007C"); // Vertical bar
                _encodingReplacements.Add('}',  "007D"); // Right Curly Bracket
                _encodingReplacements.Add('~',  "007E"); // Tilde
            }
        }
    }
}
