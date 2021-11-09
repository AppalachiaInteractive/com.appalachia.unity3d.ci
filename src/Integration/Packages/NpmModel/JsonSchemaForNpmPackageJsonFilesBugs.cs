using System.Diagnostics;
using Appalachia.CI.Integration.Attributes;

namespace Appalachia.CI.Integration.Packages.NpmModel
{
    [DoNotReorderFields]

    /// <summary>
    /// The url to your project's issue tracker and / or the email address to which issues should
    /// be reported. These are helpful for people who encounter issues with your package.
    /// </summary>
    public struct JsonSchemaForNpmPackageJsonFilesBugs
    {
        public FluffyBugs FluffyBugs;
        public string String;

        [DebuggerStepThrough] public static implicit operator JsonSchemaForNpmPackageJsonFilesBugs(FluffyBugs FluffyBugs)
        {
            return new() {FluffyBugs = FluffyBugs};
        }

        [DebuggerStepThrough] public static implicit operator JsonSchemaForNpmPackageJsonFilesBugs(string String)
        {
            return new() {String = String};
        }
    }
}
