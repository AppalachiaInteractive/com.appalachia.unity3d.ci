using System.Diagnostics;
using Appalachia.CI.Integration.Attributes;

namespace Appalachia.CI.Integration.Packages.NpmModel
{
    [DoNotReorderFields]

    /// <summary>
    /// Used to allow fallbacks in case this environment doesn't support the preceding entries.
    /// </summary>
    public struct PackageExportsEntry
    {
        public PackageExportsEntryPackageExportsEntryObject PackageExportsEntryPackageExportsEntryObject;
        public string String;
        public bool IsNull => (PackageExportsEntryPackageExportsEntryObject == null) && (String == null);

        [DebuggerStepThrough] public static implicit operator PackageExportsEntry(
            PackageExportsEntryPackageExportsEntryObject PackageExportsEntryPackageExportsEntryObject)
        {
            return new()
            {
                PackageExportsEntryPackageExportsEntryObject =
                    PackageExportsEntryPackageExportsEntryObject
            };
        }

        [DebuggerStepThrough] public static implicit operator PackageExportsEntry(string String)
        {
            return new() {String = String};
        }
    }
}
