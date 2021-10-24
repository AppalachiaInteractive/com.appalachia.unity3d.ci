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
#region Profiling And Tracing Markers

        private const string _PRF_PFX = nameof(RepositorySubdirectory) + ".";
        private static readonly ProfilerMarker _PRF_ToString = new(_PRF_PFX + nameof(ToString));

#endregion

        public RepositorySubdirectory(RepositoryMetadata repository, string relativePath)
        {
            this.relativePath = relativePath.CleanFullPath();
            var type = AssetDatabaseManager.GetMainAssetTypeAtPath(this.relativePath);
            directory = AssetDatabaseManager.LoadAssetAtPath(this.relativePath, type);
            directoryInfo = new AppaDirectoryInfo(this.relativePath);
            this.repository = repository;
        }

        public AppaDirectoryInfo directoryInfo;
        public AssetPath correctionPath;
        public bool isConventional;

        public bool showInstances;
        public HashSet<Object> lookup;
        public List<Object> instances;
        public Object directory;
        public RepositoryMetadata repository;
        public string relativePath;

        public override string ToString()
        {
            using (_PRF_ToString.Auto())
            {
                return directoryInfo.ToString();
            }
        }
    }
}
