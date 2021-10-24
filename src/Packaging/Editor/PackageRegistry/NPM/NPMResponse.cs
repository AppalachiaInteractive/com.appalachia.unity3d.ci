using System;

namespace Appalachia.CI.Packaging.PackageRegistry.NPM
{
    [Serializable]
    public class NPMResponse
    {
        public bool success;
        public string error;
        public string ok;

        public string reason;
        public string token;
    }
}
