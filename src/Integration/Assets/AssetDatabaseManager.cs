using UnityEditor.Compilation;

namespace Appalachia.CI.Integration.Assets
{
    public static partial class AssetDatabaseManager
    {
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.RootTools.Base + "Force Recompile C# Project")]
        public static void ForceRecompile()
        {
            CompilationPipeline.RequestScriptCompilation(RequestScriptCompilationOptions.None);
        }
    }
}
