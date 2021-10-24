using Appalachia.CI.Integration.Attributes;

namespace Appalachia.CI.Integration.Packages.NpmModel
{
    [DoNotReorderFields]

    /// <summary>
    /// DEPRECATED: This field is honored, but "bundledDependencies" is the correct field name.
    ///
    /// Array of package names that will be bundled when publishing the package.
    /// </summary>
    public struct BundleDependencies
    {
        public bool? Bool;
        public string[] StringArray;

        public static implicit operator BundleDependencies(bool Bool)
        {
            return new() {Bool = Bool};
        }

        public static implicit operator BundleDependencies(string[] StringArray)
        {
            return new() {StringArray = StringArray};
        }
    }
}
