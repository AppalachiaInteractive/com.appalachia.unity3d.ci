using UnityEditor.Compilation;

namespace Appalachia.CI.Integration.Assets
{
    public static partial class AssetDatabaseManager
    {
        #region Menu Items

        [UnityEditor.MenuItem(PKG.Menu.Appalachia.RootTools.Base + "Force Recompile C# Project")]
        public static void ForceRecompile()
        {
            CompilationPipeline.RequestScriptCompilation(RequestScriptCompilationOptions.None);
        }

        [UnityEditor.MenuItem(
            PKG.Menu.Appalachia.RootTools.Base + "Clear Cached Asset Data",
            false,
            PKG.Menu.Appalachia.RootTools.Priority
        )]
        public static void InvalidateCache()
        {
            _allAssetPaths = null;
            _allMonoScripts = null;
            _allProjectPaths = null;
            _runtimeMonoScripts = null;
            _scriptTypeLookup = null;
            _typeScriptLookup = null;
            _assetPathsByExtension = null;
            _assetTypeFolderLookup = null;
            _guidsByTypeName = null;
            _pathsByTypeName = null;
            _projectPathsByExtension = null;
            _typesByTypeName = null;
        }

        #endregion
    }
}
