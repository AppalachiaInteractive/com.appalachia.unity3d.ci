using System;
using Appalachia.CI.Integration.Attributes;
using Newtonsoft.Json;

namespace Appalachia.CI.Integration.Packages.NpmModel
{
    [DoNotReorderFields]
    public class JsonSchemaForNpmPackageJsonFilesPublishConfig
    {
        [JsonProperty("access", NullValueHandling = NullValueHandling.Ignore)]
        public Access? Access { get; set; }

        [JsonProperty("registry", NullValueHandling = NullValueHandling.Ignore)]
        public Uri Registry { get; set; }

        [JsonProperty("tag", NullValueHandling = NullValueHandling.Ignore)]
        public string Tag { get; set; }
    }
}
