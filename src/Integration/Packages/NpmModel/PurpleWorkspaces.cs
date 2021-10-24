using Appalachia.CI.Integration.Attributes;
using Newtonsoft.Json;

namespace Appalachia.CI.Integration.Packages.NpmModel
{
    [DoNotReorderFields]
    public class PurpleWorkspaces
    {
        /// <summary>
        ///     Packages to block from hoisting to the workspace root. Currently only supported in Yarn
        ///     only.
        /// </summary>
        [JsonProperty("nohoist", NullValueHandling = NullValueHandling.Ignore)]
        public string[] Nohoist { get; set; }

        /// <summary>
        ///     Workspace package paths. Glob patterns are supported.
        /// </summary>
        [JsonProperty("packages", NullValueHandling = NullValueHandling.Ignore)]
        public string[] Packages { get; set; }
    }
}
