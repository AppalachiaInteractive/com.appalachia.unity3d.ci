using System.Diagnostics;
using Appalachia.CI.Integration.Attributes;

namespace Appalachia.CI.Integration.Packages.NpmModel
{
    [DoNotReorderFields]

    /// <summary>
    /// The module path that is resolved when the module specifier matches "name", shadows the
    /// "main" field.
    ///
    /// The module path that is resolved when no other export type matches.
    ///
    /// The module path that is resolved when this specifier is imported as an ECMAScript module
    /// using an `import` declaration or the dynamic `import(...)` function.
    ///
    /// The module path that is resolved when this environment is Node.js.
    ///
    /// The module path that is resolved when this specifier is imported as a CommonJS module
    /// using the `require(...)` function.
    ///
    /// The module path prefix that is resolved when the module specifier starts with "name/",
    /// set to "./" to allow external modules to import any subpath.
    /// </summary>
    public struct PackageExportsEntryOrFallback
    {
        public PackageExportsEntry[] AnythingArray;
        public PackageExportsEntryPackageExportsEntryObject PackageExportsEntryPackageExportsEntryObject;
        public string String;

        public bool IsNull =>
            (AnythingArray == null) &&
            (PackageExportsEntryPackageExportsEntryObject == null) &&
            (String == null);

        [DebuggerStepThrough] public static implicit operator PackageExportsEntryOrFallback(PackageExportsEntry[] AnythingArray)
        {
            return new() {AnythingArray = AnythingArray};
        }

        [DebuggerStepThrough] public static implicit operator PackageExportsEntryOrFallback(
            PackageExportsEntryPackageExportsEntryObject PackageExportsEntryPackageExportsEntryObject)
        {
            return new()
            {
                PackageExportsEntryPackageExportsEntryObject =
                    PackageExportsEntryPackageExportsEntryObject
            };
        }

        [DebuggerStepThrough] public static implicit operator PackageExportsEntryOrFallback(string String)
        {
            return new() {String = String};
        }
    }
}
