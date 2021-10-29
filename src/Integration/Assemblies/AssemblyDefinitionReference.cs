using System;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

namespace Appalachia.CI.Integration.Assemblies
{
    [Serializable]
    public class AssemblyDefinitionReference : IComparable<AssemblyDefinitionReference>,
                                               IComparable,
                                               IEquatable<AssemblyDefinitionReference>
    {
#region Profiling And Tracing Markers

        private const string _PRF_PFX = nameof(AssemblyDefinitionReference) + ".";

        private static readonly ProfilerMarker _PRF_ToString = new(_PRF_PFX + nameof(ToString));

#endregion

        public AssemblyDefinitionReference(string guid)
        {
            this.guid = guid;
        }

        public AssemblyDefinitionReference(AssemblyDefinitionMetadata assembly, string guid)
        {
            this.assembly = assembly;
            this.guid = guid;
        }

        public AssemblyDefinitionReference(AssemblyDefinitionMetadata assembly)
        {
            this.assembly = assembly;
            guid = assembly.Id;
        }

        public AssemblyDefinitionMetadata assembly;

        public string guid;
        private bool _isDuplicated;
        private bool _hasReferenceLevelIssue;
        private bool _hasSortingIssue;

        public string DisplayName => assembly?.Name ?? guid;
        public Color IssueColor { get; set; }

        public bool HasIssue => IsDuplicated || HasReferenceLevelIssue || HasSortingIssue;

        public bool IsGuidReference => guid.StartsWith(AssemblyDefinitionModel.GUID_PREFIX);

        public bool IsDuplicated
        {
            get => _isDuplicated;
            set => _isDuplicated = value;
        }

        public bool HasReferenceLevelIssue
        {
            get => _hasReferenceLevelIssue;
            set => _hasReferenceLevelIssue = value;
        }

        public bool HasSortingIssue
        {
            get => _hasSortingIssue;
            set => _hasSortingIssue = value;
        }

        public bool IsValid
        {
            get => assembly != null;
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

            return obj is AssemblyDefinitionReference other
                ? CompareTo(other)
                : throw new ArgumentException(
                    $"Object must be of type {nameof(AssemblyDefinitionReference)}"
                );
        }

        public int CompareTo(AssemblyDefinitionReference other)
        {
            if (ReferenceEquals(this, other))
            {
                return 0;
            }

            if (ReferenceEquals(null, other))
            {
                return 1;
            }

            return Comparer<AssemblyDefinitionMetadata>.Default.Compare(assembly, other.assembly);
        }

        public bool Equals(AssemblyDefinitionReference other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return guid == other.guid;
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

            return Equals((AssemblyDefinitionReference) obj);
        }

        public override int GetHashCode()
        {
            return guid != null ? guid.GetHashCode() : 0;
        }

        public override string ToString()
        {
            using (_PRF_ToString.Auto())
            {
                return assembly?.AssemblyCurrent ?? guid;
            }
        }

        public static bool operator ==(AssemblyDefinitionReference left, AssemblyDefinitionReference right)
        {
            return Equals(left, right);
        }

        public static bool operator >(AssemblyDefinitionReference left, AssemblyDefinitionReference right)
        {
            return Comparer<AssemblyDefinitionReference>.Default.Compare(left, right) > 0;
        }

        public static bool operator >=(AssemblyDefinitionReference left, AssemblyDefinitionReference right)
        {
            return Comparer<AssemblyDefinitionReference>.Default.Compare(left, right) >= 0;
        }

        public static bool operator !=(AssemblyDefinitionReference left, AssemblyDefinitionReference right)
        {
            return !Equals(left, right);
        }

        public static bool operator <(AssemblyDefinitionReference left, AssemblyDefinitionReference right)
        {
            return Comparer<AssemblyDefinitionReference>.Default.Compare(left, right) < 0;
        }

        public static bool operator <=(AssemblyDefinitionReference left, AssemblyDefinitionReference right)
        {
            return Comparer<AssemblyDefinitionReference>.Default.Compare(left, right) <= 0;
        }
    }
}
