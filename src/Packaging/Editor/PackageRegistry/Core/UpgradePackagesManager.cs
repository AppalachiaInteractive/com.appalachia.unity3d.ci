using System;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace Appalachia.CI.Packaging.PackageRegistry.Core
{
    public class UpgradePackagesManager
    {
        public UpgradePackagesManager()
        {
#if UNITY_2019_1_OR_NEWER
            request = Client.List(false, false);
#else
            request = Client.List();
#endif
        }

        public bool packagesLoaded;

        public List<PackageInfo> UpgradeablePackages = new();
        private readonly ListRequest request;

        public string GetLatestVersion(PackageInfo info)
        {
            if (info.source == PackageSource.Git)
            {
                return info.packageId;
            }

            string latest;

            if (string.IsNullOrEmpty(info.versions.verified))
            {
                latest = info.versions.latestCompatible;
            }
            else
            {
                latest = info.versions.verified;
            }

            return latest;
        }

        public void Update()
        {
            if (!packagesLoaded && request.IsCompleted)
            {
                if (request.Status == StatusCode.Success)
                {
                    var collection = request.Result;
                    foreach (var info in collection)
                    {
                        if (info.source == PackageSource.Git)
                        {
                            UpgradeablePackages.Add(info);
                        }
                        else if (info.source == PackageSource.Registry)
                        {
                            AddRegistryPackage(info);
                        }
                    }
                }
                else
                {
                    Debug.LogError("Cannot query package manager for packages");
                }

                packagesLoaded = true;
            }
        }

        public bool UpgradePackage(PackageInfo info, ref string error)
        {
            string latestVersion;
            if (info.source == PackageSource.Git)
            {
                latestVersion = GetLatestVersion(info);
            }
            else if (info.source == PackageSource.Registry)
            {
                latestVersion = info.name + "@" + GetLatestVersion(info);
            }
            else
            {
                error = "Invalid source";
                return false;
            }

            var rqst = Client.Add(latestVersion);

            while (!rqst.IsCompleted)
            {
                Thread.Sleep(100);
            }

            if (rqst.Status == StatusCode.Success)
            {
                return true;
            }

            error = rqst.Error.message;
            return false;
        }

        private void AddRegistryPackage(PackageInfo info)
        {
            try
            {
                var latestVersion = SemVer.SemVer.Parse(GetLatestVersion(info));
                var currentVersion = SemVer.SemVer.Parse(info.version);

                if (currentVersion < latestVersion)
                {
                    UpgradeablePackages.Add(info);
                }
            }
            catch (Exception)
            {
                Debug.LogError(
                    "Invalid version for package " +
                    info.displayName +
                    ". Current: " +
                    info.version +
                    ", Latest: " +
                    GetLatestVersion(info)
                );
            }
        }
    }
}
