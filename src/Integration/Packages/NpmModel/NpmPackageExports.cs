using Appalachia.CI.Integration.Attributes;

namespace Appalachia.CI.Integration.Packages.NpmModel
{
    [DoNotReorderFields]

    /// <summary>
    /// The "exports" field is used to restrict external access to non-exported module files,
    /// also enables a module to import itself using "name".
    /// </summary>
    public struct NpmPackageExports
    {
        public PackageExportsEntry[] AnythingArray;
        public PurplePackageExportsEntryObject PurplePackageExportsEntryObject;
        public string String;

        public bool IsNull =>
            (AnythingArray == null) && (PurplePackageExportsEntryObject == null) && (String == null);

        public static implicit operator NpmPackageExports(PackageExportsEntry[] AnythingArray)
        {
            return new() {AnythingArray = AnythingArray};
        }

        public static implicit operator NpmPackageExports(
            PurplePackageExportsEntryObject PurplePackageExportsEntryObject)
        {
            return new() {PurplePackageExportsEntryObject = PurplePackageExportsEntryObject};
        }

        public static implicit operator NpmPackageExports(string String)
        {
            return new() {String = String};
        }
    }
}
