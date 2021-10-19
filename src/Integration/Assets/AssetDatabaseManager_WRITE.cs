using System;
using Appalachia.CI.Integration.Extensions;
using Appalachia.CI.Integration.FileSystem;
using UnityEditor;
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

                var savePathMetadata = GetSaveLocationForOwnedAsset<T, Texture2D>("x.png");

                var targetSavePath = AppaPath.Combine(savePathMetadata.relativePath, $"{fileName}.png");

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

                var tImporter = AssetImporter.GetAtPath(targetSavePath) as TextureImporter;
                if (tImporter != null)
                {
                    tImporter.textureType = TextureImporterType.Default;

                    tImporter.wrapMode = TextureWrapMode.Clamp;
                    tImporter.sRGBTexture = false;
                    tImporter.alphaSource = TextureImporterAlphaSource.None;
                    tImporter.SaveAndReimport();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }

            return texture;
        }
    }
}
