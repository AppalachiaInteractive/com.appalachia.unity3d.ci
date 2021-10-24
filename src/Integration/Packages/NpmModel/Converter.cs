using System.Globalization;
using Appalachia.CI.Integration.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Appalachia.CI.Integration.Packages.NpmModel
{
    [DoNotReorderFields]
    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new()
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Formatting = Formatting.Indented,
            Converters =
            {
                PersonConverter.Singleton,
                BinConverter.Singleton,
                NpmPackageBugsConverter.Singleton,
                BundleDependenciesConverter.Singleton,
                NpmPackageExportsConverter.Singleton,
                PackageExportsEntryConverter.Singleton,
                PackageExportsEntryOrFallbackConverter.Singleton,
                JsonSchemaForNpmPackageJsonFilesBugsConverter.Singleton,
                JsonSchemaForNpmPackageJsonFilesExportsConverter.Singleton,
                ManConverter.Singleton,
                PrivateUnionConverter.Singleton,
                PrivateEnumConverter.Singleton,
                AccessConverter.Singleton,
                JsonSchemaForNpmPackageJsonFilesRepositoryConverter.Singleton,
                TypeEnumConverter.Singleton,
                JsonSchemaForNpmPackageJsonFilesWorkspacesConverter.Singleton,
                NpmPackageRepositoryConverter.Singleton,
                NpmPackageWorkspacesConverter.Singleton,
                new IsoDateTimeConverter {DateTimeStyles = DateTimeStyles.AssumeUniversal}
            }
        };
    }
}
