using System.Diagnostics;
using Appalachia.CI.Integration.Attributes;

namespace Appalachia.CI.Integration.Packages.NpmModel
{
    [DoNotReorderFields]

    /// <summary>
    /// Allows packages within a directory to depend on one another using direct linking of local
    /// files. Additionally, dependencies within a workspace are hoisted to the workspace root
    /// when possible to reduce duplication. Note: It's also a good idea to set "private" to true
    /// when using this feature.
    /// </summary>
    public struct NpmPackageWorkspaces
    {
        public FluffyWorkspaces FluffyWorkspaces;
        public string[] StringArray;

        [DebuggerStepThrough] public static implicit operator NpmPackageWorkspaces(FluffyWorkspaces FluffyWorkspaces)
        {
            return new() {FluffyWorkspaces = FluffyWorkspaces};
        }

        [DebuggerStepThrough] public static implicit operator NpmPackageWorkspaces(string[] StringArray)
        {
            return new() {StringArray = StringArray};
        }
    }
}
