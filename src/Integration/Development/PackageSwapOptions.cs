using System;

namespace Appalachia.CI.Integration.Development
{
    [Flags]
    public enum PackageSwapOptions
    {
        None = 0,
        DryRun = 1 << 0,
        RefreshAssets = 1 << 1,
        ExecutePackageClient = 1 << 2,
        
        SingleExecution = ExecutePackageClient | RefreshAssets
    }
}
