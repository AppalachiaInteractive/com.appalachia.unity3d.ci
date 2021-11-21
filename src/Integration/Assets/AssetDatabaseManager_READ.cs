#if UNITY_EDITOR
using Unity.Profiling;
using UnityEditor;
using UnityEngine;

namespace Appalachia.CI.Integration.Assets
{
    public static partial class AssetDatabaseManager
    {

        private static readonly ProfilerMarker _PRF_GetPrefabAsset = new ProfilerMarker(_PRF_PFX + nameof(GetPrefabAsset));
        
        public static GameObject GetPrefabAsset(GameObject prefabInstance)
        {
            using (_PRF_GetPrefabAsset.Auto())
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
}

#endif