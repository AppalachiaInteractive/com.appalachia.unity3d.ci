#if UNITY_EDITOR

namespace Appalachia.CI.Integration.Assets
{
    public static partial class AssetDatabaseManager
    {
        public static UnityEditor.AssetImporter GetAssetImporterAtPath(string path)
        {
            ThrowIfInvalidState();
            var importer = UnityEditor.AssetImporter.GetAtPath(path);

            return importer;
        }
    }
}

#endif
