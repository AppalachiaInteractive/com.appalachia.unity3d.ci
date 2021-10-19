using UnityEditor;
using UnityEngine;

namespace Appalachia.CI.Integration.Assets
{
    public static partial class AssetDatabaseManager
    {
        public static GameObject GetPrefabAsset(GameObject prefabInstance)
        {
            var path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(prefabInstance);

            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }

            return LoadAssetAtPath<GameObject>(path);
        }
    }
}
