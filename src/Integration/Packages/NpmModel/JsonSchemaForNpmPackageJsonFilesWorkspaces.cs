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
    public struct JsonSchemaForNpmPackageJsonFilesWorkspaces
    {
        public PurpleWorkspaces PurpleWorkspaces;
        public string[] StringArray;

        public static implicit operator JsonSchemaForNpmPackageJsonFilesWorkspaces(
            PurpleWorkspaces PurpleWorkspaces)
        {
            return new() {PurpleWorkspaces = PurpleWorkspaces};
        }

        public static implicit operator JsonSchemaForNpmPackageJsonFilesWorkspaces(string[] StringArray)
        {
            return new() {StringArray = StringArray};
        }
    }
}
