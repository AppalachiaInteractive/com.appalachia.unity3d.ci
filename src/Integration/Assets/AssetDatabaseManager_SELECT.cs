using System.Reflection.Emit;
using Appalachia.CI.Integration.Extensions;
using UnityEditor;
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
            EditorGUIUtility.PingObject(o);
            EditorUtility.FocusProjectWindow();
        }
        
        public static void PingAsset(string assetPath)
        {
            var relativePath = assetPath.ToRelativePath();
            var assetType = GetMainAssetTypeAtPath(relativePath);
            var asset = LoadAssetAtPath(relativePath, assetType);

            PingAsset(asset);
        }

        public static void SetSelection(string path)
        {
            var relativePath = path.ToRelativePath();
            var assetType = GetMainAssetTypeAtPath(relativePath);
            var asset = LoadAssetAtPath(relativePath, assetType);

            SetSelection(asset);
        }

        public static void SetSelection(Object o)
        {
            Selection.activeObject = o;
            PingAsset(o);
        }
    }
}
