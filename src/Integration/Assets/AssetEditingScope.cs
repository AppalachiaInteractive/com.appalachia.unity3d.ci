#if UNITY_EDITOR

#region

using System;

#endregion

namespace Appalachia.CI.Integration.Assets
{
    public class AssetEditingScope : IDisposable
    {
        public AssetEditingScope(bool doEdit = true, bool refresh = true, bool saveAssets = true)
        {
            _doEdit = doEdit;
            _refresh = refresh;
            _saveAssets = saveAssets;

            if (_doEdit)
            {
                AssetDatabaseManager.StartAssetEditing();
            }
        }

        private readonly bool _doEdit;
        private readonly bool _refresh;
        private readonly bool _saveAssets;

        void IDisposable.Dispose()
        {
            if (_doEdit)
            {
                AssetDatabaseManager.StopAssetEditing();
                
                if (_refresh)
                {
                    AssetDatabaseManager.Refresh();
                }
                
                if (_saveAssets)
                {
                    AssetDatabaseManager.SaveAssets();
                }
            }
        }
    }
}

#endif