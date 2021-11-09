using System;

namespace Appalachia.CI.Integration.Development
{
    [Flags]
    public enum PackageSwapOptions
    {
        None = 0,
        DryRun = 1 << 0,
        RefreshAssetsAtStart = 1 << 1,
        RefreshAssetsAfterMove = 1 << 2,
        RefreshAssetsAtEnd = 1 << 3,
        ExecutePackageClient = 1 << 4,

        SingleExecution = ExecutePackageClient | RefreshAssetsAtStart | RefreshAssetsAfterMove | RefreshAssetsAtEnd,
    }
}
