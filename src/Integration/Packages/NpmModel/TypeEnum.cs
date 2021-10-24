using Appalachia.CI.Integration.Attributes;

namespace Appalachia.CI.Integration.Packages.NpmModel
{
    [DoNotReorderFields]

    /// <summary>
    /// When set to "module", the type field allows a package to specify all .js files within are
    /// ES modules. If the "type" field is omitted or set to "commonjs", all .js files are
    /// treated as CommonJS.
    /// </summary>
    public enum TypeEnum
    {
        Commonjs,
        Module,
        Library
    }
}
