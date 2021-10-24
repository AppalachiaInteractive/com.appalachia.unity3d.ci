using Appalachia.CI.Integration.Attributes;
using Newtonsoft.Json;

namespace Appalachia.CI.Integration.Packages.NpmModel
{
    [DoNotReorderFields]

    /// <summary>
    /// Used to specify conditional exports, note that Conditional exports are unsupported in
    /// older environments, so it's recommended to use the fallback array option if support for
    /// those environments is a concern.
    ///
    /// The module path that is resolved when the module specifier matches "name", shadows the
    /// "main" field.
    /// </summary>
    public class PurplePackageExportsEntryObject
    {
        /// <summary>
        ///     The module path that is resolved when the module specifier matches "name", shadows the
        ///     "main" field.
        /// </summary>
        [JsonProperty(".")]
        public PackageExportsEntryOrFallback? Empty { get; set; }

        /// <summary>
        ///     The module path prefix that is resolved when the module specifier starts with "name/",
        ///     set to "./" to allow external modules to import any subpath.
        /// </summary>
        [JsonProperty("./")]
        public PackageExportsEntryOrFallback? PackageExportsEntryObject { get; set; }

        /// <summary>
        ///     The module path that is resolved when no other export type matches.
        /// </summary>
        [JsonProperty("default")]
        public PackageExportsEntryOrFallback? Default { get; set; }

        /// <summary>
        ///     The module path that is resolved when this specifier is imported as an ECMAScript module
        ///     using an `import` declaration or the dynamic `import(...)` function.
        /// </summary>
        [JsonProperty("import")]
        public PackageExportsEntryOrFallback? Import { get; set; }

        /// <summary>
        ///     The module path that is resolved when this environment is Node.js.
        /// </summary>
        [JsonProperty("node")]
        public PackageExportsEntryOrFallback? Node { get; set; }

        /// <summary>
        ///     The module path that is resolved when this specifier is imported as a CommonJS module
        ///     using the `require(...)` function.
        /// </summary>
        [JsonProperty("require")]
        public PackageExportsEntryOrFallback? Require { get; set; }
    }
}
