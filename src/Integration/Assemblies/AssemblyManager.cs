using System.Collections.Generic;
using Appalachia.CI.Integration.Assets;
using Appalachia.CI.Integration.FileSystem;
using Appalachia.CI.Integration.Repositories;

namespace Appalachia.CI.Integration.Assemblies
{
    public static class AssemblyManager
    {
        public static Dictionary<string, AssemblyDefinitionMetadata> assemblyLookup;
        public static List<AssemblyDefinitionMetadata> assetAssemblies;
        public static List<RepositoryDirectoryMetadata> repositories;

        //[InitializeOnLoadMethod]
        public static void InitializeAssemblies()
        {
            assetAssemblies = new List<AssemblyDefinitionMetadata>();
            assemblyLookup = new Dictionary<string, AssemblyDefinitionMetadata>();
            repositories = new List<RepositoryDirectoryMetadata>();

            var assemblyDefinitionFileIds = AssetDatabaseManager.FindAssets("t:AssemblyDefinitionAsset");

            foreach (var assemblyDefinitionFileId in assemblyDefinitionFileIds)
            {
                var assemblyDefinitionFilePath =
                    AssetDatabaseManager.GUIDToAssetPath(assemblyDefinitionFileId);

                var wrapper = AssemblyDefinitionMetadata.CreateNew(assemblyDefinitionFilePath);

                if (wrapper.IsAsset)
                {
                    assetAssemblies.Add(wrapper);
                }

                assemblyLookup.Add(assemblyDefinitionFileId, wrapper);
            }

            var directoryInfo = new AppaDirectoryInfo(ProjectLocations.GetAssetsDirectoryPath());

            var children = directoryInfo.GetDirectories();

            RecursiveGitRepositorySearch(children, repositories);
        }

        private static void RecursiveGitRepositorySearch(
            AppaDirectoryInfo[] children,
            List<RepositoryDirectoryMetadata> repositories)
        {
            foreach (var child in children)
            {
                var subchildren = child.GetDirectories();

                foreach (var subchild in subchildren)
                {
                    if (subchild.Name == ".git")
                    {
                        var repo = RepositoryDirectoryMetadata.FromRoot(child);

                        repositories.Add(repo);
                        break;
                    }
                }

                RecursiveGitRepositorySearch(subchildren, repositories);
            }
        }
    }
}
