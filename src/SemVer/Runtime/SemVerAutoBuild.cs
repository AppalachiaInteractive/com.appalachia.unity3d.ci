using System;
using System.Collections.Generic;
using Appalachia.CI.Constants;

namespace Appalachia.CI.SemVer
{
    /// <summary>
    ///     Sets the <see cref="SemVer.Build">build</see> metadata automatically
    /// </summary>
    /// <seealso cref="SemVer.Build" />
    public abstract class SemVerAutoBuild
    {
        [NonSerialized] private AppaContext _context;

        protected AppaContext Context
        {
            get
            {
                if (_context == null)
                {
                    _context = new AppaContext(this);
                }

                return _context;
            }
        }
        
        /// <summary>
        ///     <see cref="SemVerAutoBuild" /> implementations
        /// </summary>
        public enum BuildType
        {
            /// <summary>
            ///     Disables automatic <see cref="SemVer.Build">build</see> metadata
            /// </summary>
            Manual,

            /// <summary>
            ///     Sets the <see cref="SemVer.Build">build</see> metadata to the
            ///     <a href="https://docs.unity3d.com/Manual/UnityCloudBuildManifest.html">Unity Cloud Build</a>
            ///     <see cref="CloudBuildManifest.BuildNumber">“build number”</see>
            /// </summary>
            /// <seealso cref="CloudBuildManifest" />
            CloudBuildNumber
        }

        public static readonly IReadOnlyDictionary<BuildType, SemVerAutoBuild> Instances =
            new Dictionary<BuildType, SemVerAutoBuild>
            {
                {BuildType.Manual, new ManualBuild()}, {BuildType.CloudBuildNumber, new CloudBuildNumberBuild()}
            };

        internal abstract string Get(string build);

        internal abstract string Set(string build);

        public abstract class ReadOnly : SemVerAutoBuild
        {
            internal sealed override string Set(string build)
            {
                Context.Log.Warn("The build metadata is read-only");
                return build;
            }
        }

        private class CloudBuildNumberBuild : ReadOnly
        {
            internal override string Get(string build)
            {
                return CloudBuildManifest.Instance.IsLoaded
                    ? CloudBuildManifest.Instance.BuildNumber.ToString()
                    : string.Empty;
            }
        }

        private class ManualBuild : SemVerAutoBuild
        {
            internal override string Get(string build)
            {
                return build;
            }

            internal override string Set(string build)
            {
                return build;
            }
        }
    }
}
