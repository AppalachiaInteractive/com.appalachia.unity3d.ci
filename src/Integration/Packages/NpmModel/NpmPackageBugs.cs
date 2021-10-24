using Appalachia.CI.Integration.Attributes;

namespace Appalachia.CI.Integration.Packages.NpmModel
{
    [DoNotReorderFields]

    /// <summary>
    /// The url to your project's issue tracker and / or the email address to which issues should
    /// be reported. These are helpful for people who encounter issues with your package.
    /// </summary>
    public struct NpmPackageBugs
    {
        public PurpleBugs PurpleBugs;
        public string String;

        public static implicit operator NpmPackageBugs(PurpleBugs PurpleBugs)
        {
            return new() {PurpleBugs = PurpleBugs};
        }

        public static implicit operator NpmPackageBugs(string String)
        {
            return new() {String = String};
        }
    }
}
