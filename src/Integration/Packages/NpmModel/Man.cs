using Appalachia.CI.Integration.Attributes;

namespace Appalachia.CI.Integration.Packages.NpmModel
{
    [DoNotReorderFields]

    /// <summary>
    /// Specify either a single file or an array of filenames to put in place for the man program
    /// to find.
    /// </summary>
    public struct Man
    {
        public string String;
        public string[] StringArray;

        public static implicit operator Man(string String)
        {
            return new() {String = String};
        }

        public static implicit operator Man(string[] StringArray)
        {
            return new() {StringArray = StringArray};
        }
    }
}
