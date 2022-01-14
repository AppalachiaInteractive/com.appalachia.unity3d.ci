using System;
using Appalachia.CI.Constants;
using Appalachia.Utility.Execution;
using Appalachia.Utility.Strings;
using Unity.Profiling;

namespace Appalachia.CI.Integration.Assets
{
    public static partial class AssetDatabaseManager
    {
        #region Static Fields and Autoproperties

        [NonSerialized] private static AppaContext _context;

        #endregion

        private static AppaContext Context
        {
            get
            {
                if (_context == null)
                {
                    _context = new AppaContext(typeof(AssetDatabaseManager));
                }

                return _context;
            }
        }

        internal static void ThrowIfInvalidState()
        {
            using (_PRF_ThrowIfInvalidState.Auto())
            {
                if (AppalachiaApplication.IsPlaying)
                {
                    throw new NotSupportedException(
                        ZString.Format("Cannot use {0} during Play mode.", nameof(AssetDatabaseManager))
                    );
                }
            }
        }

        #region Profiling

        private const string _PRF_PFX = nameof(AssetDatabaseManager) + ".";

        private static readonly ProfilerMarker _PRF_ThrowIfInvalidState =
            new ProfilerMarker(_PRF_PFX + nameof(ThrowIfInvalidState));

        #endregion
    }
}
