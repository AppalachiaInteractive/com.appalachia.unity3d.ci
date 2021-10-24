using System.Collections.Generic;
using Appalachia.CI.Integration.Attributes;

namespace Appalachia.CI.Integration.Packages.NpmModel
{
    [DoNotReorderFields]

    /// <summary>
    /// A module ID with untranspiled code that is the primary entry point to your program.
    /// </summary>
    public struct Bin
    {
        public string String;
        public Dictionary<string, string> StringMap;

        public static implicit operator Bin(string String)
        {
            return new() {String = String};
        }

        public static implicit operator Bin(Dictionary<string, string> StringMap)
        {
            return new() {StringMap = StringMap};
        }
    }
}
