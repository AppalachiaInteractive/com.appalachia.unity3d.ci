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
            AssetDatabaseManager.ThrowIfInvalidState();
            _doEdit = doEdit;
            _refresh = refresh;
            _saveAssets = saveAssets;

            if (_doEdit)
            {
                AssetDatabaseManager.StartAssetEditing();
            }
        }

        #region Fields and Autoproperties

        private readonly bool _doEdit;
        private readonly bool _refresh;
        private readonly bool _saveAssets;
        private bool _ignored;

        #endregion

        public void Ignore()
        {
            _ignored = true;
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            AssetDatabaseManager.ThrowIfInvalidState();

            if (_doEdit)
            {
                AssetDatabaseManager.StopAssetEditing();

                if (_ignored)
                {
                    return;
                }

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

        #endregion
    }
}

#endif
