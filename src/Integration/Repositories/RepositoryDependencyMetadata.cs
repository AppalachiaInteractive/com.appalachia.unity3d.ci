using System;
using Appalachia.CI.Integration.Analysis;
using Unity.Profiling;
using UnityEngine;

namespace Appalachia.CI.Integration.Repositories
{
    [Serializable]
    public class RepositoryDependencyMetadata : IAnalysisColorable
    {
        private const string _PRF_PFX = nameof(RepositoryDependencyMetadata) + ".";

        public string name;
        public RepositoryDirectoryMetadata repository;
        public string version;
        private static readonly ProfilerMarker _PRF_ToString = new(_PRF_PFX + nameof(ToString));

        public RepositoryDependencyMetadata(RepositoryDirectoryMetadata repository)
        {
            this.repository = repository;
        }

        public RepositoryDependencyMetadata(string name, string version)
        {
            this.name = name;
            this.version = version;
        }

        public RepositoryDependencyMetadata(
            string name,
            string version,
            RepositoryDirectoryMetadata repository)
        {
            this.repository = repository;
            this.name = name;
            this.version = version;
        }

        public bool IsValid => repository != null;
        public bool IsMissing => !IsValid || (name != repository.PackageName);
        public bool IsOutOfDate => !IsValid || (version != repository.PackageVersion);

        public bool HasIssue => !IsValid || IsMissing || IsOutOfDate;

        public string Name => repository?.PackageName ?? name;
        public string Version => version;
        public Color IssueColor { get; set; }

        public override string ToString()
        {
            using (_PRF_ToString.Auto())
            {
                return $"{Name} : {Version}";
            }
        }
    }
}
