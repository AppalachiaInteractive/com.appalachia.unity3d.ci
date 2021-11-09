using System.Diagnostics;
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

        [DebuggerStepThrough] public static implicit operator Person(PersonClass PersonClass)
        {
            return new() {PersonClass = PersonClass};
        }

        [DebuggerStepThrough] public static implicit operator Person(string String)
        {
            return new() {String = String};
        }
    }
}
