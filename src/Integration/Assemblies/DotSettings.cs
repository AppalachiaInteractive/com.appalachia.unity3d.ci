using System;
using System.Collections.Generic;
using System.Text;
using Appalachia.CI.Integration.Extensions;

namespace Appalachia.CI.Integration.Assemblies
{
    public class DotSettings
    {
        public DotSettings(string[] lines)
        {
            if (lines == null)
            {
                return;
            }
            
            LoadXml(lines);
        }
        
        private const string FOLDER_SKIP_END = @"/@EntryIndexedValue"">True</s:Boolean>";

        private const string FOLDER_SKIP_START =
            @"<s:Boolean x:Key=""/Default/CodeInspection/NamespaceProvider/NamespaceFoldersToSkip/=";

        private const string FOOTER = @"</wpf:ResourceDictionary>";

        private const string HEADER =
            @"<wpf:ResourceDictionary xml:space=""preserve"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" xmlns:s=""clr-namespace:System;assembly=mscorlib"" xmlns:ss=""urn:shemas-jetbrains-com:settings-storage-xaml"" xmlns:wpf=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">";
        
        public bool missingNamspaceFolderExclusions;
        
        private HashSet<string> _foldersToSkip;
        private HashSet<string> _missingFolders;
        private List<string> _otherLines;

        public bool SkipsFolder(string path)
        {
            Initialize();

            var formatted = FormatFolder(path);

            return _foldersToSkip.Contains(formatted);
        }

        public void LoadXml(string[] lines)
        {
            Initialize();

            foreach (var line in lines)
            {
                if (line.Contains(FOLDER_SKIP_START))
                {
                    var substring = line.Replace(FOLDER_SKIP_START, string.Empty)
                                        .Replace(FOLDER_SKIP_END, string.Empty);

                    _foldersToSkip.Add(substring);
                }
                else if (!line.Contains(HEADER) && !line.Contains(FOOTER))
                {
                    _otherLines.Add(line);
                    throw new NotSupportedException(line);
                }
            }
        }

        public string ToXml()
        {
            Initialize();

            var builder = new StringBuilder();

            builder.AppendLine(HEADER);

            foreach (var line in _otherLines)
            {
                builder.AppendLine(line);
            }

            var indent = new string(' ', 4);
            
            foreach (var folderToSkip in _foldersToSkip)
            {
                builder.Append(indent);
                builder.Append(FOLDER_SKIP_START);

                builder.Append(folderToSkip);

                builder.Append(FOLDER_SKIP_END);
                builder.AppendLine();
            }

            builder.AppendLine(FOOTER);

            return builder.ToString();
        }

        public void SkipFolder(string path)
        {
            Initialize();

            var formatted = FormatFolder(path);

            _foldersToSkip.Add(formatted);
        }

        public void CheckIfExcludingFolder(string path)
        {
            Initialize();

            var formatted = FormatFolder(path);

            if (_foldersToSkip.Contains(formatted))
            {
                return;
            }

            _missingFolders.Add(formatted);
            missingNamspaceFolderExclusions = true;
        }

        public void AddMissingFolders()
        {
            Initialize();
            
            foreach (var folder in _missingFolders)
            {
                _foldersToSkip.Add(folder);
            }

            _missingFolders.Clear();
        }

        private string FormatFolder(string folder)
        {
            var clean = folder.Replace("/", "\\");
            var lower = clean.ToLowerInvariant();
            var encoded = lower.EncodeUnicodePathToASCII();

            return encoded;
        }

        private void Initialize()
        {
            if (_otherLines == null)
            {
                _otherLines = new List<string>();
            }
            
            if (_foldersToSkip == null)
            {
                _foldersToSkip = new HashSet<string>();
            }

            if (_missingFolders == null)
            {
                _missingFolders = new HashSet<string>();
            }
        }
    }
}
