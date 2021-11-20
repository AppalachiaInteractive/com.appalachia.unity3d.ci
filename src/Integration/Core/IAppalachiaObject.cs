using UnityEngine;

namespace Appalachia.CI.Integration.Core
{
    public interface IAppalachiaObject
    {
#if UNITY_EDITOR
        public void SetDirtyAndSave();
        
        public string NiceName { get; set; }
        public string AssetPath { get; }
        public string DirectoryPath { get; }

        public void Ping();
        public void Select();
        public void Duplicate();
        public bool HasAssetPath(out string path);
        public bool HasSubAssets(out Object[] subAssets);
        public bool UpdateNameAndMove(string newName);
        public void OnCreate();
        
        public void Rename(string newName);
#endif
    }
}
