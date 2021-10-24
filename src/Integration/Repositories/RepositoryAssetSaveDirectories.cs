using System;
using System.Collections.Generic;

namespace Appalachia.CI.Integration.Repositories
{
    [Serializable]
    public class RepositoryAssetSaveDirectories
    {
        public List<RepositorySubdirectory> locations;
        public RepositoryMetadata repository;
    }
}
