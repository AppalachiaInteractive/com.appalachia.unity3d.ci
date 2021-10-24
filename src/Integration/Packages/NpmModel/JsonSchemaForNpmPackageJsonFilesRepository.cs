using Appalachia.CI.Integration.Attributes;

namespace Appalachia.CI.Integration.Packages.NpmModel
{
    [DoNotReorderFields]

    /// <summary>
    /// Specify the place where your code lives. This is helpful for people who want to
    /// contribute.
    /// </summary>
    public struct JsonSchemaForNpmPackageJsonFilesRepository
    {
        public PurpleRepository PurpleRepository;
        public string String;

        public static implicit operator JsonSchemaForNpmPackageJsonFilesRepository(
            PurpleRepository PurpleRepository)
        {
            return new() {PurpleRepository = PurpleRepository};
        }

        public static implicit operator JsonSchemaForNpmPackageJsonFilesRepository(string String)
        {
            return new() {String = String};
        }
    }
}
