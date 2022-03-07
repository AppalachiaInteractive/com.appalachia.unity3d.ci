using System.Collections.Generic;
using Appalachia.Utility.Strings;
using Unity.Profiling;

namespace Appalachia.CI.Constants
{
    public static partial class APPASTR
    {
        #region Static Fields and Autoproperties

        private static Dictionary<string, string> _nicifyCache;
        private static Utf16ValueStringBuilder _nicifyBuilder;
        private static bool _hasNicifyBuilder;

        #endregion

        /// <summary>
        ///     Make a displayable name for a variable.
        ///     This function will insert spaces before capital letters and remove optional m_, _ or k followed by uppercase letter in front of the name.
        /// </summary>
        /// <param name="variableName">The variable name.</param>
        /// <returns>The displayable name.</returns>
        public static string NicifyVariableName(string variableName)
        {
            using (_PRF_NicifyVariableName.Auto())
            {
                _nicifyCache ??= new();

                if (!_hasNicifyBuilder)
                {
                    _nicifyBuilder = new Utf16ValueStringBuilder(false);
                    _hasNicifyBuilder = true;
                }

                string result;

                if (_nicifyCache.TryGetValue(variableName, out result))
                {
                    return result;
                }

                var length = variableName.Length;

                if (length <= 2)
                {
                    return variableName;
                }

                _nicifyBuilder.Clear();
                var modified = false;

                for (var i = 0; i < length; i++)
                {
                    var currentCharacter = variableName[i];

                    if (i == 0)
                    {
                        if (currentCharacter == 'm')
                        {
                            var nextCharacter = variableName[1];

                            if (nextCharacter == '_')
                            {
                                i = 2;
                                modified = true;
                                continue;
                            }
                        }
                        else if (currentCharacter == 'k')
                        {
                            if (char.IsUpper(variableName, 1))
                            {
                                i = 1;
                                modified = true;
                                continue;
                            }
                        }
                        else if (currentCharacter == '_')
                        {
                            i = 1;
                            modified = true;
                            continue;
                        }

                        var upperCharacter = char.ToUpperInvariant(currentCharacter);
                        _nicifyBuilder.Append(upperCharacter);
                    }
                    else
                    {
                        var previousCharacter = variableName[i - 1];

                        var currentCharacterIsLower = char.IsLower(currentCharacter);
                        var currentCharacterIsUpper = char.IsUpper(currentCharacter);
                        var previousCharacterIsLower = char.IsLower(previousCharacter);
                        var previousCharacterIsUpper = char.IsUpper(previousCharacter);
                        
                        if (currentCharacter == '_')
                        {
                            if (previousCharacter != '_')
                            {
                                _nicifyBuilder.Append(' ');
                                modified = true;
                            }

                            continue;
                        }

                        if ((previousCharacter == '_') && currentCharacterIsLower)
                        {
                            var upperCharacter = char.ToUpperInvariant(currentCharacter);
                            _nicifyBuilder.Append(upperCharacter);
                            modified = true;
                            continue;
                        }

                        if (previousCharacterIsLower && currentCharacterIsUpper)
                        {
                            _nicifyBuilder.Append(' ');
                            _nicifyBuilder.Append(currentCharacter);
                            modified = true;
                            continue;
                        }

                        _nicifyBuilder.Append(currentCharacter);
                    }
                }

                if (!modified)
                {
                    _nicifyCache.Add(variableName, variableName);
                    return variableName;
                }

                result = _nicifyBuilder.ToString();
                _nicifyCache.Add(variableName, result);

                return result;
            }
        }

        #region Profiling

        private const string _PRF_PFX = nameof(APPASTR) + ".";

        private static readonly ProfilerMarker _PRF_NicifyVariableName =
            new ProfilerMarker(_PRF_PFX + nameof(NicifyVariableName));

        #endregion
    }
}
