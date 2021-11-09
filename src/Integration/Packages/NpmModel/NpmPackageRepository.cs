using System.Diagnostics;
using Appalachia.CI.Integration.Attributes;

namespace Appalachia.CI.Integration.Packages.NpmModel
{
    [DoNotReorderFields]

    /// <summary>
    /// Specify the place where your code lives. This is helpful for people who want to
    /// contribute.
    /// </summary>
    public struct NpmPackageRepository
    {
        public FluffyRepository FluffyRepository;
        public string String;

        [DebuggerStepThrough] public static implicit operator NpmPackageRepository(FluffyRepository FluffyRepository)
        {
            return new() {FluffyRepository = FluffyRepository};
        }

        [DebuggerStepThrough] public static implicit operator NpmPackageRepository(string String)
        {
            return new() {String = String};
        }
    }
}
