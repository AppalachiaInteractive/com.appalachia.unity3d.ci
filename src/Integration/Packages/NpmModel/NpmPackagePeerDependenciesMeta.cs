using Appalachia.CI.Integration.Attributes;
using Newtonsoft.Json;

namespace Appalachia.CI.Integration.Packages.NpmModel
{
    [DoNotReorderFields]
    public class NpmPackagePeerDependenciesMeta
    {
        /// <summary>
        ///     Specifies that this peer dependency is optional and should not be installed automatically.
        /// </summary>
        [JsonProperty("optional", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Optional { get; set; }
    }
}
