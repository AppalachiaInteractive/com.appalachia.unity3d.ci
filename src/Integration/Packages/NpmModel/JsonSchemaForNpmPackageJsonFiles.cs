using System.Collections.Generic;
using Appalachia.CI.Integration.Attributes;
using Newtonsoft.Json;

namespace Appalachia.CI.Integration.Packages.NpmModel
{
    [DoNotReorderFields]
    public class JsonSchemaForNpmPackageJsonFiles
    {
        [JsonProperty("author", NullValueHandling = NullValueHandling.Ignore)]
        public Person? Author { get; set; }

        [JsonProperty("bin", NullValueHandling = NullValueHandling.Ignore)]
        public Bin? Bin { get; set; }

        /// <summary>
        ///     The url to your project's issue tracker and / or the email address to which issues should
        ///     be reported. These are helpful for people who encounter issues with your package.
        /// </summary>
        [JsonProperty("bugs", NullValueHandling = NullValueHandling.Ignore)]
        public JsonSchemaForNpmPackageJsonFilesBugs? Bugs { get; set; }

        /// <summary>
        ///     Array of package names that will be bundled when publishing the package.
        /// </summary>
        [JsonProperty("bundledDependencies", NullValueHandling = NullValueHandling.Ignore)]
        public BundleDependencies? BundledDependencies { get; set; }

        /// <summary>
        ///     DEPRECATED: This field is honored, but "bundledDependencies" is the correct field name.
        /// </summary>
        [JsonProperty("bundleDependencies", NullValueHandling = NullValueHandling.Ignore)]
        public BundleDependencies? BundleDependencies { get; set; }

        /// <summary>
        ///     A 'config' hash can be used to set configuration parameters used in package scripts that
        ///     persist across upgrades.
        /// </summary>
        [JsonProperty("config", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Config { get; set; }

        /// <summary>
        ///     A list of people who contributed to this package.
        /// </summary>
        [JsonProperty("contributors", NullValueHandling = NullValueHandling.Ignore)]
        public Person[] Contributors { get; set; }

        /// <summary>
        ///     Specify that your code only runs on certain cpu architectures.
        /// </summary>
        [JsonProperty("cpu", NullValueHandling = NullValueHandling.Ignore)]
        public string[] Cpu { get; set; }

        [JsonProperty("dependencies", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> Dependencies { get; set; }

        /// <summary>
        ///     This helps people discover your package, as it's listed in 'npm search'.
        /// </summary>
        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        [JsonProperty("devDependencies", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> DevDependencies { get; set; }

        [JsonProperty("directories", NullValueHandling = NullValueHandling.Ignore)]
        public JsonSchemaForNpmPackageJsonFilesDirectories Directories { get; set; }

        [JsonProperty("dist", NullValueHandling = NullValueHandling.Ignore)]
        public JsonSchemaForNpmPackageJsonFilesDist Dist { get; set; }

        [JsonProperty("engines", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> Engines { get; set; }

        [JsonProperty("engineStrict", NullValueHandling = NullValueHandling.Ignore)]
        public bool? EngineStrict { get; set; }

        /// <summary>
        ///     A module ID with untranspiled code that is the primary entry point to your program.
        /// </summary>
        [JsonProperty("esnext", NullValueHandling = NullValueHandling.Ignore)]
        public Bin? Esnext { get; set; }

        /// <summary>
        ///     The "exports" field is used to restrict external access to non-exported module files,
        ///     also enables a module to import itself using "name".
        /// </summary>
        [JsonProperty("exports")]
        public JsonSchemaForNpmPackageJsonFilesExports? Exports { get; set; }

        /// <summary>
        ///     The 'files' field is an array of files to include in your project. If you name a folder
        ///     in the array, then it will also include the files inside that folder.
        /// </summary>
        [JsonProperty("files", NullValueHandling = NullValueHandling.Ignore)]
        public string[] Files { get; set; }

        /// <summary>
        ///     The url to the project homepage.
        /// </summary>
        [JsonProperty("homepage", NullValueHandling = NullValueHandling.Ignore)]
        public string Homepage { get; set; }

        [JsonProperty("jspm", NullValueHandling = NullValueHandling.Ignore)]
        public JsonSchemaForNpmPackageJsonFiles Jspm { get; set; }

        /// <summary>
        ///     This helps people discover your package as it's listed in 'npm search'.
        /// </summary>
        [JsonProperty("keywords", NullValueHandling = NullValueHandling.Ignore)]
        public string[] Keywords { get; set; }

        /// <summary>
        ///     You should specify a license for your package so that people know how they are permitted
        ///     to use it, and any restrictions you're placing on it.
        /// </summary>
        [JsonProperty("license", NullValueHandling = NullValueHandling.Ignore)]
        public string License { get; set; }

        /// <summary>
        ///     DEPRECATED: Instead, use SPDX expressions, like this: { "license": "ISC" } or {
        ///     "license": "(MIT OR Apache-2.0)" } see:
        ///     'https://docs.npmjs.com/files/package.json#license'.
        /// </summary>
        [JsonProperty("licenses", NullValueHandling = NullValueHandling.Ignore)]
        public JsonSchemaForNpmPackageJsonFilesLicense[] Licenses { get; set; }

        /// <summary>
        ///     The main field is a module ID that is the primary entry point to your program.
        /// </summary>
        [JsonProperty("main", NullValueHandling = NullValueHandling.Ignore)]
        public string Main { get; set; }

        /// <summary>
        ///     A list of people who maintains this package.
        /// </summary>
        [JsonProperty("maintainers", NullValueHandling = NullValueHandling.Ignore)]
        public Person[] Maintainers { get; set; }

        /// <summary>
        ///     Specify either a single file or an array of filenames to put in place for the man program
        ///     to find.
        /// </summary>
        [JsonProperty("man", NullValueHandling = NullValueHandling.Ignore)]
        public Man? Man { get; set; }

        /// <summary>
        ///     An ECMAScript module ID that is the primary entry point to your program.
        /// </summary>
        [JsonProperty("module", NullValueHandling = NullValueHandling.Ignore)]
        public string Module { get; set; }

        /// <summary>
        ///     The name of the package.
        /// </summary>
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(MinMaxLengthCheckConverter))]
        public string Name { get; set; }

        [JsonProperty("optionalDependencies", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> OptionalDependencies { get; set; }

        /// <summary>
        ///     Specify which operating systems your module will run on.
        /// </summary>
        [JsonProperty("os", NullValueHandling = NullValueHandling.Ignore)]
        public string[] Os { get; set; }

        [JsonProperty("peerDependencies", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> PeerDependencies { get; set; }

        /// <summary>
        ///     When a user installs your package, warnings are emitted if packages specified in
        ///     "peerDependencies" are not already installed. The "peerDependenciesMeta" field serves to
        ///     provide more information on how your peer dependencies are utilized. Most commonly, it
        ///     allows peer dependencies to be marked as optional. Metadata for this field is specified
        ///     with a simple hash of the package name to a metadata object.
        /// </summary>
        [JsonProperty("peerDependenciesMeta", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, JsonSchemaForNpmPackageJsonFilesPeerDependenciesMeta> PeerDependenciesMeta
        {
            get;
            set;
        }

        /// <summary>
        ///     DEPRECATED: This option used to trigger an npm warning, but it will no longer warn. It is
        ///     purely there for informational purposes. It is now recommended that you install any
        ///     binaries as local devDependencies wherever possible.
        /// </summary>
        [JsonProperty("preferGlobal", NullValueHandling = NullValueHandling.Ignore)]
        public bool? PreferGlobal { get; set; }

        /// <summary>
        ///     If set to true, then npm will refuse to publish it.
        /// </summary>
        [JsonProperty("private", NullValueHandling = NullValueHandling.Ignore)]
        public PrivateUnion? Private { get; set; }

        [JsonProperty("publishConfig", NullValueHandling = NullValueHandling.Ignore)]
        public JsonSchemaForNpmPackageJsonFilesPublishConfig PublishConfig { get; set; }

        [JsonProperty("readme", NullValueHandling = NullValueHandling.Ignore)]
        public string Readme { get; set; }

        /// <summary>
        ///     Specify the place where your code lives. This is helpful for people who want to
        ///     contribute.
        /// </summary>
        [JsonProperty("repository", NullValueHandling = NullValueHandling.Ignore)]
        public JsonSchemaForNpmPackageJsonFilesRepository? Repository { get; set; }

        /// <summary>
        ///     Resolutions is used to support selective version resolutions, which lets you define
        ///     custom package versions or ranges inside your dependencies. See:
        ///     https://classic.yarnpkg.com/en/docs/selective-version-resolutions
        /// </summary>
        [JsonProperty("resolutions", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Resolutions { get; set; }

        /// <summary>
        ///     The 'scripts' member is an object hash of script commands that are run at various times
        ///     in the lifecycle of your package. The key is the lifecycle event, and the value is the
        ///     command to run at that point.
        /// </summary>
        [JsonProperty("scripts", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> Scripts { get; set; }

        /// <summary>
        ///     When set to "module", the type field allows a package to specify all .js files within are
        ///     ES modules. If the "type" field is omitted or set to "commonjs", all .js files are
        ///     treated as CommonJS.
        /// </summary>
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public TypeEnum? Type { get; set; }

        /// <summary>
        ///     Set the types property to point to your bundled declaration file.
        /// </summary>
        [JsonProperty("types", NullValueHandling = NullValueHandling.Ignore)]
        public string Types { get; set; }

        /// <summary>
        ///     The "typesVersions" field is used since TypeScript 3.1 to support features that were only
        ///     made available in newer TypeScript versions.
        /// </summary>
        [JsonProperty("typesVersions", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, JsonSchemaForNpmPackageJsonFilesTypesVersion> TypesVersions { get; set; }

        /// <summary>
        ///     Note that the "typings" field is synonymous with "types", and could be used as well.
        /// </summary>
        [JsonProperty("typings", NullValueHandling = NullValueHandling.Ignore)]
        public string Typings { get; set; }

        /// <summary>
        ///     Version must be parseable by node-semver, which is bundled with npm as a dependency.
        /// </summary>
        [JsonProperty("version", NullValueHandling = NullValueHandling.Ignore)]
        public string Version { get; set; }

        /// <summary>
        ///     Allows packages within a directory to depend on one another using direct linking of local
        ///     files. Additionally, dependencies within a workspace are hoisted to the workspace root
        ///     when possible to reduce duplication. Note: It's also a good idea to set "private" to true
        ///     when using this feature.
        /// </summary>
        [JsonProperty("workspaces", NullValueHandling = NullValueHandling.Ignore)]
        public JsonSchemaForNpmPackageJsonFilesWorkspaces? Workspaces { get; set; }
    }
}
