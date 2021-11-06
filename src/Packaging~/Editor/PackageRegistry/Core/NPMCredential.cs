using System;

namespace Appalachia.CI.Packaging.PackageRegistry.Core
{
    [Serializable]
    public class NPMCredential
    {
        public bool alwaysAuth;
        public string token;
        public string url;
    }
}
