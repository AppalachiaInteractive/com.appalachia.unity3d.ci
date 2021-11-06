using System;
using System.Collections.Generic;
using UnityEngine;

namespace Appalachia.CI.Packaging.PackageRegistry.Core
{
    [Serializable]
    public class ScopedRegistry
    {
        public bool auth;
        public List<string> scopes = new();
        public string name;

        public string token;
        public string url;

        public bool isValid()
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            if (scopes.Count < 1)
            {
                return false;
            }

            scopes.RemoveAll(string.IsNullOrEmpty);

            foreach (var scope in scopes)
            {
                if (Uri.CheckHostName(scope) != UriHostNameType.Dns)
                {
                    Debug.LogWarning("Invalid scope " + scope);
                    return false;
                }
            }

            return isValidCredential();
        }

        public bool isValidCredential()
        {
            if (string.IsNullOrEmpty(url) || !Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                return false;
            }

            if (auth)
            {
                if (string.IsNullOrEmpty(token))
                {
                    return false;
                }
            }

            return true;
        }

        public override string ToString()
        {
            return JsonUtility.ToJson(this, true);
        }
    }
}
