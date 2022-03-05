#if UNITY_EDITOR
using UnityEngine;

namespace Appalachia.CI.Integration.Assets
{
    public static partial class AssetDatabaseManager
    {
        public static void TestAssetType<T>()
            where T : Object
        {
        }
    }
}

#endif
