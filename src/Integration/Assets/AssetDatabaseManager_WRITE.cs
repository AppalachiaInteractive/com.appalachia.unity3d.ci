#if UNITY_EDITOR
using System;
using Appalachia.CI.Integration.Extensions;
using Appalachia.CI.Integration.FileSystem;
using Appalachia.Utility.Logging;
using UnityEngine;

namespace Appalachia.CI.Integration.Assets
{
    public static partial class AssetDatabaseManager
    {
        public static Texture2D SaveTextureAssetToFile<T>(T owner, Texture2D texture)
            where T : MonoBehaviour
        {
            try
            {
                var fileName = texture.name;

                if (fileName.EndsWith(".png"))
                {
                    fileName = fileName.Replace(".png", string.Empty);
                    texture.name = fileName;
                }

                var savePathMetadata = GetSaveDirectoryForOwnedAsset<T, Texture2D>("x.png");

                var targetSavePath = AppaPath.Combine(savePathMetadata.ToRelativePath(), $"{fileName}.png");

                var absolutePath = targetSavePath;

                if (absolutePath.StartsWith("Assets"))
                {
                    absolutePath = targetSavePath.ToAbsolutePath();
                }

                var directoryName = AppaPath.GetDirectoryName(absolutePath);
                AppaDirectory.CreateDirectory(directoryName);

                var bytes = texture.EncodeToPNG();
                AppaFile.WriteAllBytes(absolutePath, bytes);

                ImportAsset(targetSavePath);

                texture = LoadAssetAtPath<Texture2D>(targetSavePath);

                var tImporter = UnityEditor.AssetImporter.GetAtPath(targetSavePath) as UnityEditor.TextureImporter;
                if (tImporter != null)
                {
                    tImporter.textureType = UnityEditor.TextureImporterType.Default;

                    tImporter.wrapMode = TextureWrapMode.Clamp;
                    tImporter.sRGBTexture = false;
                    tImporter.alphaSource = UnityEditor.TextureImporterAlphaSource.None;
                    tImporter.SaveAndReimport();
                }
            }
            catch (Exception ex)
            {
                AppaLog.Error(ex);
            }

            return texture;
        }
    }
}

#endif