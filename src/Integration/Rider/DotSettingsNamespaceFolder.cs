using System;
using System.Collections.Generic;

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

        public static bool operator ==(DotSettingsNamespaceFolder left, DotSettingsNamespaceFolder right)
        {
            return Equals(left, right);
        }

        public static bool operator >(DotSettingsNamespaceFolder left, DotSettingsNamespaceFolder right)
        {
            return Comparer<DotSettingsNamespaceFolder>.Default.Compare(left, right) > 0;
        }

        public static bool operator >=(DotSettingsNamespaceFolder left, DotSettingsNamespaceFolder right)
        {
            return Comparer<DotSettingsNamespaceFolder>.Default.Compare(left, right) >= 0;
        }

        public static bool operator !=(DotSettingsNamespaceFolder left, DotSettingsNamespaceFolder right)
        {
            return !Equals(left, right);
        }

        public static bool operator <(DotSettingsNamespaceFolder left, DotSettingsNamespaceFolder right)
        {
            return Comparer<DotSettingsNamespaceFolder>.Default.Compare(left, right) < 0;
        }

        public static bool operator <=(DotSettingsNamespaceFolder left, DotSettingsNamespaceFolder right)
        {
            return Comparer<DotSettingsNamespaceFolder>.Default.Compare(left, right) <= 0;
        }

        public override bool Equals(object obj)
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

        public override int GetHashCode()
        {
            return encoded != null ? encoded.GetHashCode() : 0;
        }

        public int CompareTo(object obj)
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

        public int CompareTo(DotSettingsNamespaceFolder other)
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

        public bool Equals(DotSettingsNamespaceFolder other)
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
