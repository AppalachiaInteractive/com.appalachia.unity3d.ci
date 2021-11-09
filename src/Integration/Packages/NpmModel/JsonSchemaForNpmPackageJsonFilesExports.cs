using System.Diagnostics;
using Appalachia.CI.Integration.Attributes;

namespace Appalachia.CI.Integration.Packages.NpmModel
{
    [DoNotReorderFields]

    /// <summary>
    /// The "exports" field is used to restrict external access to non-exported module files,
    /// also enables a module to import itself using "name".
    /// </summary>
    public struct JsonSchemaForNpmPackageJsonFilesExports
    {
        public PackageExportsEntry[] AnythingArray;
        public FluffyPackageExportsEntryObject FluffyPackageExportsEntryObject;
        public string String;

        public bool IsNull =>
            (AnythingArray == null) && (FluffyPackageExportsEntryObject == null) && (String == null);

        [DebuggerStepThrough] public static implicit operator JsonSchemaForNpmPackageJsonFilesExports(
            PackageExportsEntry[] AnythingArray)
        {
            return new() {AnythingArray = AnythingArray};
        }

        [DebuggerStepThrough] public static implicit operator JsonSchemaForNpmPackageJsonFilesExports(
            FluffyPackageExportsEntryObject FluffyPackageExportsEntryObject)
        {
            return new() {FluffyPackageExportsEntryObject = FluffyPackageExportsEntryObject};
        }

        [DebuggerStepThrough] public static implicit operator JsonSchemaForNpmPackageJsonFilesExports(string String)
        {
            return new() {String = String};
        }
    }
}
