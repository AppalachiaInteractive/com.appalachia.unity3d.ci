using System.Collections.ObjectModel;

namespace Appalachia.CI.SemVer
{
    /// <summary>
    ///     Information returned about a checked semantic version.
    ///     <seealso cref="SemVer.Validate" />
    /// </summary>
    public class SemVerValidationResult
    {
        internal SemVerValidationResult(ReadOnlyCollection<string> errors, SemVer corrected)
        {
            Errors = errors;
            Corrected = corrected;
        }

        #region Fields and Autoproperties

        /// <summary>
        ///     Error messages. This collection is empty if the version is valid.
        /// </summary>
        public readonly ReadOnlyCollection<string> Errors;

        /// <summary>
        ///     Automatically corrected semantic version.
        /// </summary>
        public readonly SemVer Corrected;

        #endregion

        /// <summary>
        ///     Does the version meet the <a href="https://semver.org/">Semantic Versioning 2.0.0</a> specification?
        /// </summary>
        public bool IsValid => Errors.Count == 0;
    }
}
