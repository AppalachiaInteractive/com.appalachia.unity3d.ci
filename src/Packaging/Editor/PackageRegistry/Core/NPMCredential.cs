using System;

namespace Appalachia.CI.Packaging.PackageRegistry.Core
{
    [Serializable]
    public class NPMCredential
    {
        public string url;
        public string token;
        public bool alwaysAuth;
    }
}