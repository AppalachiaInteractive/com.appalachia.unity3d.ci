#if UNITY_EDITOR
using Unity.Profiling;
using UnityEditor;
using UnityEngine;

namespace Appalachia.CI.Integration.Assets
{
    public static partial class AssetDatabaseManager
    {
        public static GameObject GetPrefabAsset(GameObject prefabInstance)
        {
            ThrowIfInvalidState();
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

        #region Profiling

        private static readonly ProfilerMarker _PRF_GetPrefabAsset =
            new ProfilerMarker(_PRF_PFX + nameof(GetPrefabAsset));

        #endregion
    }
}

#endif
