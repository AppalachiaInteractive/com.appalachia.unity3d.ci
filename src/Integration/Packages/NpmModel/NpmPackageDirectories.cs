using Appalachia.CI.Integration.Attributes;
using Newtonsoft.Json;

namespace Appalachia.CI.Integration.Packages.NpmModel
{
    [DoNotReorderFields]
    public class NpmPackageDirectories
    {
        /// <summary>
        ///     If you specify a 'bin' directory, then all the files in that folder will be used as the
        ///     'bin' hash.
        /// </summary>
        [JsonProperty("bin", NullValueHandling = NullValueHandling.Ignore)]
        public string Bin { get; set; }

        /// <summary>
        ///     Put markdown files in here. Eventually, these will be displayed nicely, maybe, someday.
        /// </summary>
        [JsonProperty("doc", NullValueHandling = NullValueHandling.Ignore)]
        public string Doc { get; set; }

        /// <summary>
        ///     Put example scripts in here. Someday, it might be exposed in some clever way.
        /// </summary>
        [JsonProperty("example", NullValueHandling = NullValueHandling.Ignore)]
        public string Example { get; set; }

        /// <summary>
        ///     Tell people where the bulk of your library is. Nothing special is done with the lib
        ///     folder in any way, but it's useful meta info.
        /// </summary>
        [JsonProperty("lib", NullValueHandling = NullValueHandling.Ignore)]
        public string Lib { get; set; }

        /// <summary>
        ///     A folder that is full of man pages. Sugar to generate a 'man' array by walking the folder.
        /// </summary>
        [JsonProperty("man", NullValueHandling = NullValueHandling.Ignore)]
        public string Man { get; set; }

        [JsonProperty("test", NullValueHandling = NullValueHandling.Ignore)]
        public string Test { get; set; }
    }
}
