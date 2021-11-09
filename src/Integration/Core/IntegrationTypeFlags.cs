using System;

namespace Appalachia.CI.Integration.Core
{
    [Flags]
    public enum IntegrationTypeFlags
    {
        /*00000000*/
        None = 0,

        /*00000001*/
        IsAsset = 1 << 0,

        /*00000010*/
        IsLibrary = 1 << 1,

        /*00000100*/
        IsPackage = 1 << 2,

        /*00001000*/
        IsAppalachia = 1 << 3,

        /*00010000*/
        IsBuiltinUnity = 1 << 4,

        /*00100000*/
        IsCustomUnity = 1 << 5,

        /*01000000*/
        IsThirdParty = 1 << 6,

        /*00110000*/
        IsUnity = IsBuiltinUnity | IsCustomUnity,

        /*00111000*/
        IsAppalachiaManaged = IsAppalachia | IsThirdParty | IsCustomUnity,
    }
}
