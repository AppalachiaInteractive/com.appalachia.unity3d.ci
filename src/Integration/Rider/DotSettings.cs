using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Appalachia.CI.Integration.Assemblies;
using Appalachia.CI.Integration.Extensions;

namespace Appalachia.CI.Integration.Rider
{
    public class DotSettings
    {
        private const string FOLDER_SKIP_END = @"/@EntryIndexedValue"">True</s:Boolean>";

        private const string FOLDER_SKIP_START =
            @"<s:Boolean x:Key=""/Default/CodeInspection/NamespaceProvider/NamespaceFoldersToSkip/=";

        private const string FOOTER = @"</wpf:ResourceDictionary>";

        private const string HEADER =
            @"<wpf:ResourceDictionary xml:space=""preserve"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" xmlns:s=""clr-namespace:System;assembly=mscorlib"" xmlns:ss=""urn:shemas-jetbrains-com:settings-storage-xaml"" xmlns:wpf=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">";

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

        private List<DotSettingsNamespaceFolder> _folders;
        private Dictionary<string, DotSettingsNamespaceFolder> _foldersLookup;

        public IEnumerable<DotSettingsNamespaceFolder> AllFolders
        {
            get
            {
                Initialize();
                
                return _folders;
            }
        }

        public IEnumerable<DotSettingsNamespaceFolder> ExcludedFolders => AllFolders.Where(f => f.excluded);
        public IEnumerable<DotSettingsNamespaceFolder> MissingFolders => AllFolders.Where(f => !f.excluded);
        public IEnumerable<DotSettingsNamespaceFolder> EncodingIssues => AllFolders.Where(f => f.encodingIssue);

        private List<string> _otherLines;
        
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

                builder.Append(FOLDER_SKIP_END);
                builder.AppendLine();
            }

            builder.AppendLine(FOOTER);

            return builder.ToString();
        }

        public void AddMissingFolders()
        {
            Initialize();

            foreach (var folder in MissingFolders)
            {
                folder.excluded = true;
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

                    folder.excluded = false;
                }
            }
        }

        public void LoadXml(string[] lines)
        {
            Initialize();

            foreach (var line in lines)
            {
                if (line.Contains(FOLDER_SKIP_START))
                {
                    var encoded = line.Replace(FOLDER_SKIP_START, string.Empty)
                                      .Replace(FOLDER_SKIP_END, string.Empty)
                                      .Trim();

                    var folder = Create(null, encoded);
                    folder.excluded = true;

                    if (encoded.Contains(FOOTER))
                    {
                        folder.encodingIssue = true;
                    }
                }
                else if (!line.Contains(HEADER) && !line.Contains(FOOTER))
                {
                    _otherLines.Add(line);
                    throw new NotSupportedException(line);
                }
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
