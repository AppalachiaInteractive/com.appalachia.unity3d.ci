using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Appalachia.CI.Integration.Rider
{
    public class DotSettingsNamespaceFolder : IEquatable<DotSettingsNamespaceFolder>,
                                              IComparable<DotSettingsNamespaceFolder>,
                                              IComparable
    {
        public bool encodingIssue;
        public bool excluded;
        public bool shouldExclude;
        public string encoded;
        public string path;
        public bool HasEncodingIssue => encodingIssue;
        public bool HasExclusionIssue => shouldExclude && !excluded;

        public bool HasIssue => HasExclusionIssue || HasExclusionIssue;

        [DebuggerStepThrough] public static bool operator ==(DotSettingsNamespaceFolder left, DotSettingsNamespaceFolder right)
        {
            return Equals(left, right);
        }

        [DebuggerStepThrough] public static bool operator >(DotSettingsNamespaceFolder left, DotSettingsNamespaceFolder right)
        {
            return Comparer<DotSettingsNamespaceFolder>.Default.Compare(left, right) > 0;
        }

        [DebuggerStepThrough] public static bool operator >=(DotSettingsNamespaceFolder left, DotSettingsNamespaceFolder right)
        {
            return Comparer<DotSettingsNamespaceFolder>.Default.Compare(left, right) >= 0;
        }

        [DebuggerStepThrough] public static bool operator !=(DotSettingsNamespaceFolder left, DotSettingsNamespaceFolder right)
        {
            return !Equals(left, right);
        }

        [DebuggerStepThrough] public static bool operator <(DotSettingsNamespaceFolder left, DotSettingsNamespaceFolder right)
        {
            return Comparer<DotSettingsNamespaceFolder>.Default.Compare(left, right) < 0;
        }

        [DebuggerStepThrough] public static bool operator <=(DotSettingsNamespaceFolder left, DotSettingsNamespaceFolder right)
        {
            return Comparer<DotSettingsNamespaceFolder>.Default.Compare(left, right) <= 0;
        }

        [DebuggerStepThrough] public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((DotSettingsNamespaceFolder) obj);
        }

        [DebuggerStepThrough] public override int GetHashCode()
        {
            return encoded != null ? encoded.GetHashCode() : 0;
        }

        [DebuggerStepThrough] public int CompareTo(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return 1;
            }

            if (ReferenceEquals(this, obj))
            {
                return 0;
            }

            return obj is DotSettingsNamespaceFolder other
                ? CompareTo(other)
                : throw new ArgumentException($"Object must be of type {nameof(DotSettingsNamespaceFolder)}");
        }

        [DebuggerStepThrough] public int CompareTo(DotSettingsNamespaceFolder other)
        {
            if (ReferenceEquals(this, other))
            {
                return 0;
            }

            if (ReferenceEquals(null, other))
            {
                return 1;
            }

            return string.Compare(encoded, other.encoded, StringComparison.Ordinal);
        }

        [DebuggerStepThrough] public bool Equals(DotSettingsNamespaceFolder other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return encoded == other.encoded;
        }
    }
}
