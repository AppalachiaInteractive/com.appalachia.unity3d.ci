using System;

namespace Appalachia.CI.Packaging.PackageRegistry.NPM
{
    [Serializable]
    internal class NPMLoginRequest
    {
        public string name;
        public string password;
    }
}