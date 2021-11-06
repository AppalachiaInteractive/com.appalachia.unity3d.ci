using Appalachia.CI.Packaging.PackageRegistry.Core;
using UnityEngine;

namespace Appalachia.CI.Packaging.PackageRegistry.UI
{
    internal class TokenMethod : GUIContent
    {
        public TokenMethod(string name, string usernameName, string passwordName, GetToken action) : base(
            name
        )
        {
            this.usernameName = usernameName;
            this.passwordName = passwordName;
            this.action = action;
        }

        internal GetToken action;
        internal string passwordName;
        internal string usernameName;

        internal delegate bool GetToken(ScopedRegistry registry, string username, string password);
    }
}
