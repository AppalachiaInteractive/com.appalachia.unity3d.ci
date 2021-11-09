using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Appalachia.CI.Integration.FileSystem;
using Appalachia.Utility.Extensions;
using Appalachia.Utility.Logging;
using UnityEngine;

namespace Appalachia.CI.Integration.SourceControl
{
    public class IgnoreFile
    {
        public IgnoreFile(string path)
        {
            _ignored = new HashSet<string>();
            _notIgnored = new HashSet<string>();

            _patterns = AppaFile.ReadAllLines(path)
                                .Where(line => !string.IsNullOrWhiteSpace(line))
                                .Where(line => !line.StartsWith("#"))
                                .Select(line => new IgnorePattern(line))
                                .ForEach(p => p.pattern.TrimEnd())
                                .ForEach(p => p.Replace("\\",   "\\\\"))
                                .ForEach(p => p.Replace("/",    "\\/"))
                                .ForEach(p => p.Replace(".",    "\\."))
                                .ForEach(p => p.Replace("(",    "\\("))
                                .ForEach(p => p.Replace(")",    "\\)"))
                                .ForEach(p => p.Replace("**",   "(.````)"))
                                .ForEach(p => p.Replace("*",    "([^/]*)"))
                                .ForEach(p => p.Replace("````", "*"))
                                .ForEach(p => p.Replace("?",    "([^/])"))
                                .ForEach(
                                     p => p.pattern = p.pattern.StartsWith("!")
                                         ? $"(?!{p.pattern.Substring(1)})"
                                         : p.pattern
                                 )
                                .ForEach(p => p.Compile())
                                .ToList();
        }

        private readonly HashSet<string> _ignored;
        private readonly HashSet<string> _notIgnored;

        private readonly List<IgnorePattern> _patterns;

        public bool IsIgnored(string path)
        {
            if (_ignored.Contains(path))
            {
                return true;
            }

            if (_notIgnored.Contains(path))
            {
                return false;
            }

            for (var i = 0; i < _patterns.Count; i++)
            {
                var pattern = _patterns[i];

                if (pattern.regex.IsMatch(path))
                {
                    _ignored.Add(path);
                    return true;
                }
            }

            _notIgnored.Add(path);
            return false;
        }

        #region Nested Types

        private class IgnorePattern
        {
            public IgnorePattern(string original)
            {
                this.original = original;
                pattern = original;
            }

            public Regex regex;

            public string original;
            public string pattern;

            public override string ToString()
            {
                return pattern;
            }

            public void Compile()
            {
                try
                {
                    pattern = $"^{pattern}$";
                    regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);
                }
                catch (ArgumentException)
                {
                    AppaLog.Error($"Failed to compile regex [{pattern}]");
                    throw;
                }
            }

            public void Replace(string find, string replace)
            {
                pattern = pattern.Replace(find, replace);
            }
        }

        #endregion
    }
}
