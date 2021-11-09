using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Appalachia.CI.Integration.Core;
using Appalachia.CI.Integration.FileSystem;
using Appalachia.CI.Integration.Repositories;
using Unity.Profiling;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

namespace Appalachia.CI.Integration.Packages
{
    [Serializable]
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public class PackageMetadata : IntegrationMetadata<PackageMetadata>
    {
        #region Profiling And Tracing Markers

        private const string _PRF_PFX = nameof(PackageMetadata) + ".";

        private static readonly ProfilerMarker _PRF_PackageMetadata = new(_PRF_PFX + nameof(PackageMetadata));

        private static readonly ProfilerMarker _PRF_ExecutePackageListRequest =
            new(_PRF_PFX + nameof(ExecutePackageListRequest));

        private static readonly ProfilerMarker _PRF_Packages = new(_PRF_PFX + nameof(Packages));
        private static readonly ProfilerMarker _PRF_FindAllInternal = new(_PRF_PFX + nameof(FindAllInternal));

        private static readonly ProfilerMarker _PRF_InitializeInternal =
            new(_PRF_PFX + nameof(InitializeInternal));

        private static readonly ProfilerMarker _PRF_FinalizeInternal =
            new(_PRF_PFX + nameof(FinalizeInternal));

        #endregion

        static PackageMetadata()
        {
            using (_PRF_PackageMetadata.Auto())
            {
                _request = ExecutePackageListRequest();

                IntegrationMetadataRegistry<PackageMetadata>.Register(
                    0,
                    FindAllInternal,
                    ProcessAll,
                    FinalizeInternal
                );
            }
        }

        private PackageMetadata()
        {
        }

        private static ListRequest _request;

        public HashSet<PackageMetadata> dependents;

        public PackageInfo packageInfo;
        public RepositoryMetadata repository;

        public static PackageCollection Packages
        {
            get
            {
                using (_PRF_Packages.Auto())
                {
                    if (_request == null)
                    {
                        _request = ExecutePackageListRequest();
                    }

                    if (!_request.IsCompleted)
                    {
                        return null;
                    }

                    return _request.Result;
                }
            }
        }

        public override string Id => packageInfo.name;
        public override string Name => packageInfo.name;
        public override string Path => packageInfo.assetPath;

        public bool IsOutOfDate => (packageInfo != null) && (Version != LatestVersion);
        public string LatestVersion => packageInfo?.versions.latest ?? string.Empty;

        public string NameAndVersion => $"{Name}@{Version}";

        public string Version => packageInfo?.version ?? string.Empty;

        [DebuggerStepThrough] public static bool operator ==(PackageMetadata left, PackageMetadata right)
        {
            return Equals(left, right);
        }

        [DebuggerStepThrough] public static bool operator >(PackageMetadata left, PackageMetadata right)
        {
            return Comparer<PackageMetadata>.Default.Compare(left, right) > 0;
        }

        [DebuggerStepThrough] public static bool operator >=(PackageMetadata left, PackageMetadata right)
        {
            return Comparer<PackageMetadata>.Default.Compare(left, right) >= 0;
        }

        [DebuggerStepThrough] public static bool operator !=(PackageMetadata left, PackageMetadata right)
        {
            return !Equals(left, right);
        }

        [DebuggerStepThrough] public static bool operator <(PackageMetadata left, PackageMetadata right)
        {
            return Comparer<PackageMetadata>.Default.Compare(left, right) < 0;
        }

        [DebuggerStepThrough] public static bool operator <=(PackageMetadata left, PackageMetadata right)
        {
            return Comparer<PackageMetadata>.Default.Compare(left, right) <= 0;
        }

        private static ListRequest ExecutePackageListRequest()
        {
            using (_PRF_ExecutePackageListRequest.Auto())
            {
                return Client.List(false, false);
            }
        }

        private static void FinalizeInternal()
        {
            using (_PRF_FinalizeInternal.Auto())
            {
                foreach (var instance in Instances)
                {
                    instance.repository = RepositoryMetadata.FindByName(instance.Name);
                }
            }
        }

        private static IEnumerable<PackageMetadata> FindAllInternal()
        {
            using (_PRF_FindAllInternal.Auto())
            {
                while (!_request.IsCompleted)
                {
                    Task.Delay(1);
                }

                foreach (var packageInfo in Packages)
                {
                    var newMetadata = new PackageMetadata();

                    newMetadata.InitializeInternal(packageInfo);

                    yield return newMetadata;
                }
            }
        }

        [DebuggerStepThrough] public override int CompareTo(PackageMetadata other)
        {
            if (ReferenceEquals(this, other))
            {
                return 0;
            }

            if (ReferenceEquals(null, other))
            {
                return 1;
            }

            return Comparer<string>.Default.Compare(NameAndVersion, other.NameAndVersion);
        }

        [DebuggerStepThrough] public override int CompareTo(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return 1;
            }

            if (ReferenceEquals(this, obj))
            {
                return 0;
            }

            return obj is PackageMetadata other
                ? CompareTo(other)
                : throw new ArgumentException($"Object must be of type {nameof(PackageMetadata)}");
        }

        [DebuggerStepThrough] public override bool Equals(PackageMetadata other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(packageInfo, other.packageInfo);
        }

        [DebuggerStepThrough] public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((PackageMetadata) obj);
        }

        [DebuggerStepThrough] public override int GetHashCode()
        {
            return packageInfo != null ? packageInfo.GetHashCode() : 0;
        }

        public override void InitializeForAnalysis()
        {
        }

        [DebuggerStepThrough] public override string ToString()
        {
            return NameAndVersion;
        }

        internal void PopulateDependents()
        {
            if (dependents != null)
            {
                return;
            }

            dependents = new HashSet<PackageMetadata>();

            var allPackages = FindAll();

            foreach (var packageToCheck in allPackages)
            {
                if (packageToCheck == this)
                {
                    continue;
                }
                
                var repositoryToCheck = packageToCheck.repository;
                
                if (repositoryToCheck != null)
                {
                    foreach (var dependency in packageToCheck.repository.dependencies)
                    {
                        if (dependency.name == repository.Name)
                        {
                            dependents.Add(packageToCheck);
                            
                            break;
                        }
                    }
                }
                else
                {
                    var packageInfoToCheck = packageToCheck.packageInfo;
                    var packageDependencies = packageInfoToCheck.dependencies;

                    foreach (var dependency in packageDependencies)
                    {
                        if (dependency.name == Name)
                        {
                            dependents.Add(packageToCheck);
                            
                            break;
                        }
                    }
                }
            }
        }

        protected override IEnumerable<string> GetIds()
        {
            yield return packageInfo.name;
            yield return NameAndVersion;
        }

        private void InitializeInternal(PackageInfo pkg)
        {
            using (_PRF_InitializeInternal.Auto())
            {
                directory = new AppaDirectoryInfo(pkg.assetPath);
                packageInfo = pkg;
                repository = RepositoryMetadata.FindByName(pkg.name);
            }
        }
    }
}
