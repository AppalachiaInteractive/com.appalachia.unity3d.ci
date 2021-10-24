using System;
using Appalachia.CI.Integration.Assets;
using Appalachia.CI.Integration.Extensions;
using Appalachia.CI.Integration.FileSystem;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Appalachia.CI.Integration.Paths
{
    [Serializable]
    public class AssetPath
    {
        public AssetPath(string rp, bool isDirectory, AssetPathType pathType = AssetPathType.None)
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

            if (pathType == AssetPathType.None)
            {
                if (relativePath.StartsWith("Packages"))
                {
                    this.pathType = AssetPathType.PackageResource;
                }
                else if (relativePath.StartsWith("Library"))
                {
                    this.pathType = AssetPathType.LibraryResource;
                }
            }
            else
            {
                this.pathType = pathType;
            }
        }

        public AppaDirectoryInfo parentAppaDirectory;
        public AppaFileSystemInfo assetInfo;
        public AssetPathType pathType;
        public bool doesExist;
        public bool isDirectory;
        public Object asset;
        public Object parentDirectory;
        public string absolutePath;
        public string name;
        public string parentDirectoryRelative;
        public string relativePath;

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

        public static AssetPath ForScript(MonoScript script)
        {
            if (script == null)
            {
                return null;
            }

            var path = AssetDatabaseManager.GetAssetPath(script);

            var result = new AssetPath(path, false, AssetPathType.Script);
            return result;
        }
    }
}
