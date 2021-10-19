using Appalachia.CI.Integration.Assets;
using Appalachia.CI.Integration.FileSystem;
using UnityEditor;

namespace Appalachia.CI.Integration
{
    public static class ShaderIncludesCentralizer
    {
        private const string INCLUDE_PATH = "Assets/Resources/CGIncludes";

        [MenuItem("Appalachia/Tools/Appalachia.CI.Integration/Centralize All Shader Includes")]
        [InitializeOnLoadMethod]
        public static void CentralizeAllShaderIncludes()
        {
            var shaderIncudePaths = AssetDatabaseManager.FindAssetPaths<ShaderInclude>();

            var baseDirectory = new AppaDirectoryInfo(INCLUDE_PATH);

            for (var index = 0; index < shaderIncudePaths.Length; index++)
            {
                var shaderIncludePath = shaderIncudePaths[index];

                var file = new AppaFileInfo(shaderIncludePath);

                if (baseDirectory.IsFileInAnySubdirectory(file))
                {
                    continue;
                }

                var newPath = AppaPath.Combine(INCLUDE_PATH, file.Name);

                if (AppaFile.Exists(newPath))
                {
                    continue;
                }

                AppaFile.Copy(shaderIncludePath, newPath);
            }
        }
    }
}
