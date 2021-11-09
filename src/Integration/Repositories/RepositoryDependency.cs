using System;
using Unity.Profiling;

namespace Appalachia.CI.Integration.Repositories
{
    [Serializable]
    public class RepositoryDependency
    {
        #region Profiling And Tracing Markers

        private const string _PRF_PFX = nameof(RepositoryDependency) + ".";
        private static readonly ProfilerMarker _PRF_ToString = new(_PRF_PFX + nameof(ToString));

        #endregion

        public RepositoryDependency(RepositoryMetadata repository)
        {
            this.repository = repository;
        }

        public RepositoryDependency(string name, string version)
        {
            this.name = name;
            this.version = version;
        }

        public RepositoryDependency(string name, string version, RepositoryMetadata repository)
        {
            this.repository = repository;
            this.name = name;
            this.version = version;
        }

        public RepositoryMetadata repository;

        public string name;
        public string version;
        private bool _isMissing;
        public bool IsMissing => _isMissing || !IsValid || (name != repository.PackageName);
        public bool IsOutOfDate => !IsValid || (version != repository.PackageVersion);

        public bool IsValid => repository != null;

        public string Name => repository?.PackageName ?? name;
        public string Version => version;

        public override string ToString()
        {
            using (_PRF_ToString.Auto())
            {
                return $"{Name} : {Version}";
            }
        }

        public void SetMissing()
        {
            _isMissing = true;
        }
    }
}
