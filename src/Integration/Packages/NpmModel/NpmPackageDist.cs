using Appalachia.CI.Integration.Attributes;
using Newtonsoft.Json;

namespace Appalachia.CI.Integration.Packages.NpmModel
{
    [DoNotReorderFields]
    public class NpmPackageDist
    {
        [JsonProperty("shasum", NullValueHandling = NullValueHandling.Ignore)]
        public string Shasum { get; set; }

        [JsonProperty("tarball", NullValueHandling = NullValueHandling.Ignore)]
        public string Tarball { get; set; }
    }
}
