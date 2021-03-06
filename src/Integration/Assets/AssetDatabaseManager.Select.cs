#if UNITY_EDITOR
using Appalachia.CI.Integration.Extensions;
using UnityEngine;

namespace Appalachia.CI.Integration.Assets
{
    public static partial class AssetDatabaseManager
    {
        /*
        public void x()
        {
            Selection.count;
            Selection.assetGUIDs;
            Selection.activeGameObject;
            Selection.activeObject;
            Selection.gameObjects;
            Selection.objects;
        }
        */

        public static void PingAsset(Object o)
        {
            ThrowIfInvalidState();
            UnityEditor.EditorGUIUtility.PingObject(o);
            UnityEditor.EditorUtility.FocusProjectWindow();
        }

        public static void PingAsset(string assetPath)
        {
            ThrowIfInvalidState();
            var relativePath = assetPath.ToRelativePath();
            var assetType = GetMainAssetTypeAtPath(relativePath);
            var asset = LoadAssetAtPath(relativePath, assetType);

            PingAsset(asset);
        }

        public static void SetSelection(string path)
        {
            ThrowIfInvalidState();
            var relativePath = path.ToRelativePath();
            var assetType = GetMainAssetTypeAtPath(relativePath);
            var asset = LoadAssetAtPath(relativePath, assetType);

            SetSelection(asset);
        }

        public static void SetSelection(Object o)
        {
            ThrowIfInvalidState();
            UnityEditor.Selection.activeObject = o;
            PingAsset(o);
        }
    }
}

#endif
