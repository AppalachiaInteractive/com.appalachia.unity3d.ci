using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Appalachia.CI.Integration.Assemblies;
using Appalachia.CI.Integration.Extensions;
using Appalachia.Core.Extensions;

namespace Appalachia.CI.Integration.Rider
{
    public class DotSettings
    {
        private const string FALSE = "False";
        private const string FOLDER_SKIP_BOOLEND = @"</s:Boolean>";
        private const string FOLDER_SKIP_END_FALSE = FOLDER_SKIP_ENTRYINDEX + FALSE + FOLDER_SKIP_BOOLEND;

        private const string FOLDER_SKIP_END_TRUE = FOLDER_SKIP_ENTRYINDEX + TRUE + FOLDER_SKIP_BOOLEND;
        private const string ENTRYINDEXVALUE = "EntryIndexedValue";
        private const string FOLDER_SKIP_ENTRYINDEX = @"/@EntryIndexedValue"">";

        private const string FOLDER_SKIP_START =
            @"<s:Boolean x:Key=""/Default/CodeInspection/NamespaceProvider/NamespaceFoldersToSkip/=";

        private const string FOOTER = @"</wpf:ResourceDictionary>";

        private const string HEADER =
            @"<wpf:ResourceDictionary xml:space=""preserve"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" xmlns:s=""clr-namespace:System;assembly=mscorlib"" xmlns:ss=""urn:shemas-jetbrains-com:settings-storage-xaml"" xmlns:wpf=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">";

        private const string MALFORMED1 = @"/@EntryIndexedValue""&gt;True";
        private const string MALFORMED2 = @"/@EntryIndexedValue""&gt;False";
        private const string MALFORMED3 = @"&gt";
        private const string MALFORMED4 = @"&lt>";
        private const string TRUE = "True";

        private static readonly string[] MALFORMATIONS = {MALFORMED1, MALFORMED2, MALFORMED3, MALFORMED4};

        public DotSettings(string[] lines)
        {
            if (lines == null)
            {
                return;
            }

            LoadXml(lines);
        }

        private static HashSet<string> _nameExclusions;

        private static string[] _pathExclusions;
        private Dictionary<string, DotSettingsNamespaceFolder> _foldersLookup;

        private List<DotSettingsNamespaceFolder> _folders;

        private List<string> _otherLines;

        public IEnumerable<DotSettingsNamespaceFolder> AllFolders
        {
            get
            {
                Initialize();

                return _folders;
            }
        }

        public IEnumerable<DotSettingsNamespaceFolder> EncodingIssues =>
            AllFolders.Where(f => f.encodingIssue);

        public IEnumerable<DotSettingsNamespaceFolder> ExcludedFolders => AllFolders.Where(f => f.excluded);
        public IEnumerable<DotSettingsNamespaceFolder> MissingFolders => AllFolders.Where(f => !f.excluded);

        public void AddMissingFolders()
        {
            Initialize();

            foreach (var folder in MissingFolders)
            {
                folder.shouldExclude = true;
                folder.excluded = true;
            }
        }

        public void CheckNamespaceFolderIssues(AssemblyDefinitionMetadata assembly)
        {
            Initialize();

            var nameExclusions = GetDirectoryNameExclusions();
            var pathExclusions = GetDirectoryPathExclusions();

            var dir = assembly.directory;
            var first = true;

            var exclusions = new List<string>();

            do
            {
                if (dir.Parent == null)
                {
                    throw new NotSupportedException(assembly.ToString());
                }

                if (!first)
                {
                    dir = dir.Parent;
                }

                first = false;

                var lowerName = dir.Name.ToLowerInvariant();
                var relativePath = dir.RelativePath;
                var relativePathLower = lowerName.ToLowerInvariant();

                if (nameExclusions.Contains(lowerName))
                {
                    exclusions.Add(relativePath);
                    continue;
                }

                foreach (var pathExclusion in pathExclusions)
                {
                    if (relativePathLower.EndsWith(pathExclusion))
                    {
                        exclusions.Add(relativePath);
                        break;
                    }
                }
            } while ((dir != null) &&
                     (dir.Name != "Assets") &&
                     (dir.Name != "Packages") &&
                     (dir.Name != "Library"));

            foreach (var exclusion in exclusions)
            {
                if (!assembly.dotSettings.IsExcludingFolder(exclusion, out var encoded))
                {
                    var folder = Create(exclusion, encoded);

                    folder.shouldExclude = true;
                    folder.excluded = false;
                }
            }
        }

        public void FixEncodingIssues()
        {
            Initialize();

            foreach (var folder in EncodingIssues)
            {
                folder.encoded = folder.encoded.Replace(FOOTER, string.Empty);
                folder.encoded = folder.encoded.Replace(HEADER, string.Empty);
                folder.encodingIssue = false;
                folder.excluded = true;
            }
        }

        public void LoadXml(string[] lines)
        {
            Initialize();

            foreach (var originalLine in lines)
            {
                if (string.IsNullOrWhiteSpace(originalLine))
                {
                    continue;
                }

                if (originalLine.Contains(FOLDER_SKIP_START))
                {
                    var cleanLine = originalLine.Replace(FOLDER_SKIP_START, string.Empty)
                                                .Replace(HEADER, string.Empty)
                                                .Replace(FOOTER, string.Empty)
                                                .Trim();

                    var excluded = cleanLine.Contains(TRUE) && !cleanLine.Contains(FALSE);

                    var encoded = cleanLine.Replace(FOLDER_SKIP_END_TRUE, string.Empty)
                                           .Replace(FOLDER_SKIP_END_FALSE, string.Empty)
                                           .Trim();

                    var entryValuesfirst = originalLine.IndexOf(ENTRYINDEXVALUE);
                    var entryValuesLast = originalLine.LastIndexOf(ENTRYINDEXVALUE);

                    var encodingIssue = false;

                    foreach (var malformed in MALFORMATIONS)
                    {
                        if (originalLine.Contains(malformed))
                        {
                            encodingIssue = true;
                        }

                        encoded = encoded.Replace(malformed, string.Empty);
                    }

                    encodingIssue = encodingIssue || (entryValuesfirst != entryValuesLast);
                                    
                    var folder = Create(null, encoded);

                    folder.excluded = excluded;
                    folder.encodingIssue = encodingIssue;
                }
                else if (!originalLine.Contains(HEADER) && !originalLine.Contains(FOOTER))
                {
                    _otherLines.Add(originalLine);
                    throw new NotSupportedException(originalLine);
                }
            }
        }

        public string ToXml()
        {
            Initialize();

            var builder = new StringBuilder();
            var alreadyAdded = new HashSet<string>();

            builder.AppendLine(HEADER);

            foreach (var line in _otherLines)
            {
                builder.AppendLine(line);
            }

            var indent = new string(' ', 4);

            foreach (var excludedFolder in ExcludedFolders)
            {
                if (alreadyAdded.Contains(excludedFolder.encoded))
                {
                    continue;
                }

                alreadyAdded.Add(excludedFolder.encoded);

                builder.Append(indent);
                builder.Append(FOLDER_SKIP_START);

                builder.Append(excludedFolder.encoded);

                builder.Append(FOLDER_SKIP_END_TRUE);
                builder.AppendLine();
            }

            builder.AppendLine(FOOTER);

            return builder.ToString();
        }

        private DotSettingsNamespaceFolder Create(string path, string encoded)
        {
            Initialize();

            if (path != null)
            {
                encoded = GetEncodedPath(path);
            }

            if (_foldersLookup.ContainsKey(encoded))
            {
                return _foldersLookup[encoded];
            }

            var newFolder = new DotSettingsNamespaceFolder {path = path, encoded = encoded};

            if (encoded.Contains(FOOTER) || encoded.Contains(HEADER))
            {
                newFolder.encodingIssue = true;
            }

            _foldersLookup.Add(encoded, newFolder);
            _folders.Add(newFolder);

            _folders.Sort();

            return newFolder;
        }

        private string GetEncodedPath(string folder)
        {
            Initialize();

            if (_foldersLookup.ContainsKey(folder))
            {
                return _foldersLookup[folder].encoded;
            }

            var clean = folder.Replace("/", "\\");
            var lower = clean.ToLowerInvariant();
            var encoded = lower.EncodeUnicodePathToASCII(StringExtensions.EncodingPrefix.Underscore);

            return encoded;
        }

        private void Initialize()
        {
            if (_otherLines == null)
            {
                _otherLines = new List<string>();
            }

            if (_foldersLookup == null)
            {
                _foldersLookup = new Dictionary<string, DotSettingsNamespaceFolder>();
            }

            if (_folders == null)
            {
                _folders = new List<DotSettingsNamespaceFolder>();
            }
        }

        private bool IsExcludingFolder(string path, out string encoded)
        {
            Initialize();

            encoded = GetEncodedPath(path);

            if (!_foldersLookup.ContainsKey(encoded))
            {
                return false;
            }

            var folder = _foldersLookup[encoded];

            return folder.excluded;
        }

        private static HashSet<string> GetDirectoryNameExclusions()
        {
            if (_nameExclusions == null)
            {
                _nameExclusions = new HashSet<string>
                {
                    "runtime",
                    "editor",
                    "test",
                    "tests",
                    "src"
                };
            }

            return _nameExclusions;
        }

        private static string[] GetDirectoryPathExclusions()
        {
            if (_pathExclusions == null)
            {
                _pathExclusions = new[]
                {
                    "assets/",
                    "assets/thirdparty/",
                    "assets/thirdparty/assemblies/",
                    "assets/third-party/",
                    "assets/third-party/assemblies/"
                };
            }

            return _pathExclusions;
        }
    }
}
