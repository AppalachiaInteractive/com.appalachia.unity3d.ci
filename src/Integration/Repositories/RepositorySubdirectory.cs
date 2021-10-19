using System;
using System.Collections.Generic;
using Appalachia.CI.Integration.Assets;
using Appalachia.CI.Integration.Extensions;
using Appalachia.CI.Integration.FileSystem;
using Appalachia.CI.Integration.Paths;
using Unity.Profiling;
using Object = UnityEngine.Object;

namespace Appalachia.CI.Integration.Repositories
{
    [Serializable]
    public class RepositorySubdirectory
    {
        private const string _PRF_PFX = nameof(RepositorySubdirectory) + ".";
        public AssetPathMetadata correctionPath;
        public Object directory;
        public List<Object> instances;
        public bool isConventional;
        public string relativePath;
        public RepositoryDirectoryMetadata repository;

        public bool showInstances;
        private static readonly ProfilerMarker _PRF_ToString = new(_PRF_PFX + nameof(ToString));
        public AppaDirectoryInfo directoryInfo;
        public HashSet<Object> lookup;

        public RepositorySubdirectory(RepositoryDirectoryMetadata repository, string relativePath)
        {
            this.relativePath = relativePath.CleanFullPath();
            var type = AssetDatabaseManager.GetMainAssetTypeAtPath(this.relativePath);
            directory = AssetDatabaseManager.LoadAssetAtPath(this.relativePath, type);
            directoryInfo = new AppaDirectoryInfo(this.relativePath);
            this.repository = repository;
        }

        public override string ToString()
        {
            using (_PRF_ToString.Auto())
            {
                return directoryInfo.ToString();
            }
        }
    }
}
