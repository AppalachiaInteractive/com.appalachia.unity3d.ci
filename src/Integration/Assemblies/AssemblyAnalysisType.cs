namespace Appalachia.CI.Integration.Assemblies
{
    public enum AssemblyAnalysisType
    {
        All = 00000,
        DependencyLevel = 00010,
        DependencyOpportunity = 00020,
        DependencyPresence = 00030,
        DependencyVersions = 00040,
        NameAssembly = 00050,
        NameFile = 00060,
        Namespace = 00070,
        ReferenceStyle = 00080,
        ReferenceValidity = 00090,
        Sorting = 00100,
        ReferenceDuplicates = 00110,
        DependencyValidity = 00120,
        NamespaceFoldersExclusions = 00130,
        NamespaceFoldersEncoding = 00140,
    }
}
