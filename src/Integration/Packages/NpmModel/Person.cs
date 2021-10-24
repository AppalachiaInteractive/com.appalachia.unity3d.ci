using Appalachia.CI.Integration.Attributes;

namespace Appalachia.CI.Integration.Packages.NpmModel
{
    [DoNotReorderFields]

    /// <summary>
    /// A person who has been involved in creating or maintaining this package.
    /// </summary>
    public struct Person
    {
        public PersonClass PersonClass;
        public string String;

        public static implicit operator Person(PersonClass PersonClass)
        {
            return new() {PersonClass = PersonClass};
        }

        public static implicit operator Person(string String)
        {
            return new() {String = String};
        }
    }
}
