using System;
using Appalachia.CI.Integration.Assets;
using Appalachia.CI.Integration.Extensions;
using Appalachia.CI.Integration.FileSystem;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Appalachia.CI.Integration.Paths
{
    [Serializable]
    public class AssetPathMetadata
    {
        public string absolutePath;
        public Object asset;
        public bool doesExist;
        public bool isDirectory;
        public string name;
        public Object parentDirectory;
        public string parentDirectoryRelative;
        public AssetPathMetadataType pathType;
        public string relativePath;
        public AppaFileSystemInfo assetInfo;
        public AppaDirectoryInfo parentAppaDirectory;

        public AssetPathMetadata(
            string rp,
            bool isDirectory,
            AssetPathMetadataType pathType = AssetPathMetadataType.None)
        {
            this.isDirectory = isDirectory;

            if (this.isDirectory)
            {
                assetInfo = new AppaDirectoryInfo(rp);
            }
            else
            {
                assetInfo = new AppaFileInfo(rp);
            }

            parentAppaDirectory = assetInfo.Parent;
            absolutePath = assetInfo.FullPath;
            name = assetInfo.Name;
            doesExist = assetInfo.Exists;

            var parentFullPath = parentAppaDirectory.FullPath;
            var cleanParentFullPath = parentFullPath.CleanFullPath();

            var cleanDataPath = ProjectLocations.GetAssetsDirectoryPath().CleanFullPath();
            var cleanProjectPath = cleanDataPath.Replace("/Assets", string.Empty);

            parentDirectoryRelative = cleanParentFullPath.Replace(cleanProjectPath, string.Empty);

            parentDirectoryRelative = parentDirectoryRelative.CleanFullPath();

            if (AssetDatabaseManager.IsValidFolder(parentDirectoryRelative))
            {
                var type = AssetDatabaseManager.GetMainAssetTypeAtPath(parentDirectoryRelative);
                parentDirectory = AssetDatabaseManager.LoadAssetAtPath(parentDirectoryRelative, type);
            }

            relativePath = absolutePath.CleanFullPath();

            if (relativePath.Contains(cleanProjectPath))
            {
                relativePath = relativePath.Replace(cleanProjectPath, string.Empty);
            }

            relativePath = relativePath.CleanFullPath();

            if (parentDirectory != null)
            {
                var type = AssetDatabaseManager.GetMainAssetTypeAtPath(relativePath);
                asset = AssetDatabaseManager.LoadAssetAtPath(relativePath, type);
            }

            if (pathType == AssetPathMetadataType.None)
            {
                if (relativePath.StartsWith("Packages"))
                {
                    this.pathType = AssetPathMetadataType.PackageResource;
                }
                else if (relativePath.StartsWith("Library"))
                {
                    this.pathType = AssetPathMetadataType.LibraryResource;
                }
            }
            else
            {
                this.pathType = pathType;
            }
        }

        public void CreateDirectoryStructure()
        {
            AssetDatabaseManager.CreateFolder(relativePath);
            doesExist = true;
        }

        public string[] GetFileLines()
        {
            return AppaFile.ReadAllLines(absolutePath);
        }

        public void WriteFileLines(string[] lines)
        {
            AppaFile.WriteAllLines(absolutePath, lines);
        }

        public static AssetPathMetadata ForScript(MonoScript script)
        {
            if (script == null)
            {
                return null;
            }

            var path = AssetDatabaseManager.GetAssetPath(script);

            var result = new AssetPathMetadata(path, false, AssetPathMetadataType.Script);
            return result;
        }
    }
}
