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

        #region Constants and Static Readonly

        public static readonly IReadOnlyDictionary<BuildType, SemVerAutoBuild> Instances =
            new Dictionary<BuildType, SemVerAutoBuild>
            {
                { BuildType.Manual, new ManualBuild() },
                { BuildType.CloudBuildNumber, new CloudBuildNumberBuild() }
            };

        #endregion

        #region Fields and Autoproperties

        [NonSerialized] private AppaContext _context;

        #endregion

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

        internal abstract string Get(string build);

        internal abstract string Set(string build);

        #region Nested type: CloudBuildNumberBuild

        private class CloudBuildNumberBuild : ReadOnly
        {
            /// <inheritdoc />
            internal override string Get(string build)
            {
                return CloudBuildManifest.Instance.IsLoaded
                    ? CloudBuildManifest.Instance.BuildNumber.ToString()
                    : string.Empty;
            }
        }

        #endregion

        #region Nested type: ManualBuild

        private class ManualBuild : SemVerAutoBuild
        {
            /// <inheritdoc />
            internal override string Get(string build)
            {
                return build;
            }

            /// <inheritdoc />
            internal override string Set(string build)
            {
                return build;
            }
        }

        #endregion

        #region Nested type: ReadOnly

        public abstract class ReadOnly : SemVerAutoBuild
        {
            /// <inheritdoc />
            internal sealed override string Set(string build)
            {
                Context.Log.Warn("The build metadata is read-only");
                return build;
            }
        }

        #endregion
    }
}
