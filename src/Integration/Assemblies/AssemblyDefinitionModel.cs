using System;
using Appalachia.CI.Integration.Attributes;
using Unity.Profiling;

namespace Appalachia.CI.Integration.Assemblies
{
    [Serializable]
    [DoNotReorderFields]
    public class AssemblyDefinitionModel
    {
#region Profiling And Tracing Markers

        private const string _PRF_PFX = nameof(AssemblyDefinitionModel) + ".";
        private static readonly ProfilerMarker _PRF_ToString = new(_PRF_PFX + nameof(ToString));

#endregion

        public const string GUID_PREFIX = "GUID:";

        public string name;
        public bool allowUnsafeCode;
        public bool autoReferenced;
        public bool noEngineReferences;
        public bool overrideReferences;
        public string rootNamespace;
        public string[] defineConstraints;
        public string[] references;
        public string[] optionalUnityReferences;
        public string[] precompiledReferences;
        public string[] excludePlatforms;
        public string[] includePlatforms;
        public AssemblyVersionDefineModel[] versionDefines;

        public void CheckBeforeWrite()
        {
            name = string.IsNullOrWhiteSpace(name) ? string.Empty : name;
            rootNamespace = string.IsNullOrWhiteSpace(rootNamespace) ? string.Empty : rootNamespace;

            references ??= new string[0];
            includePlatforms ??= new string[0];
            excludePlatforms ??= new string[0];
            precompiledReferences ??= new string[0];
            defineConstraints ??= new string[0];
            optionalUnityReferences ??= new string[0];

            versionDefines ??= new AssemblyVersionDefineModel[0];
        }

        public override string ToString()
        {
            using (_PRF_ToString.Auto())
            {
                return name;
            }
        }
    }
}
