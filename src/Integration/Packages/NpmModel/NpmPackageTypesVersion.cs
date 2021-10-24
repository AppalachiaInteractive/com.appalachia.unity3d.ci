using Appalachia.CI.Integration.Attributes;
using Newtonsoft.Json;

namespace Appalachia.CI.Integration.Packages.NpmModel
{
    [DoNotReorderFields]

    /// <summary>
    /// Contains overrides for the TypeScript version that matches the version range matching the
    /// property key.
    /// </summary>
    public class NpmPackageTypesVersion
    {
        /// <summary>
        ///     Maps all file paths to the file paths specified in the array.
        /// </summary>
        [JsonProperty("*", NullValueHandling = NullValueHandling.Ignore)]
        public string[] Empty { get; set; }
    }
}
