using System;

namespace Appalachia.CI.Integration.Core
{
    [Flags]
    public enum IntegrationTypeFlags
    {
        None = 0,
        IsAsset = 1 << 0,
        IsLibrary = 1 << 1,
        IsPackage = 1 << 2,
        IsAppalachia = 1 << 3,
        IsBuiltinUnity = 1 << 4,
        IsCustomUnity = 1 << 5,
    }
}
