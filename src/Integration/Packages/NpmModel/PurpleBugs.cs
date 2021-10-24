using System;
using Appalachia.CI.Integration.Attributes;
using Newtonsoft.Json;

namespace Appalachia.CI.Integration.Packages.NpmModel
{
    [DoNotReorderFields]
    public class PurpleBugs
    {
        /// <summary>
        ///     The email address to which issues should be reported.
        /// </summary>
        [JsonProperty("email", NullValueHandling = NullValueHandling.Ignore)]
        public string Email { get; set; }

        /// <summary>
        ///     The url to your project's issue tracker.
        /// </summary>
        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
        public Uri Url { get; set; }
    }
}
