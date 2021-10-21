using System;
using System.Collections.Generic;
using Appalachia.CI.Integration.Analysis;
using Unity.Profiling;
using UnityEngine;

namespace Appalachia.CI.Integration.Assemblies
{
    [Serializable]
    public class AssemblyDefinitionReferenceMetadata : IComparable<AssemblyDefinitionReferenceMetadata>,
                                                       IComparable,
                                                       IAnalysisColorable,
                                                       IEquatable<AssemblyDefinitionReferenceMetadata>
    {
        private const string _PRF_PFX = nameof(AssemblyDefinitionReferenceMetadata) + ".";

        private static readonly ProfilerMarker _PRF_ToString = new(_PRF_PFX + nameof(ToString));

        public AssemblyDefinitionReferenceMetadata(string guid)
        {
            this.guid = guid;
        }

        public AssemblyDefinitionReferenceMetadata(AssemblyDefinitionMetadata assembly, string guid)
        {
            this.assembly = assembly;
            this.guid = guid;
        }

        public AssemblyDefinitionReferenceMetadata(AssemblyDefinitionMetadata assembly)
        {
            this.assembly = assembly;
            guid = assembly.guid;
        }
        
        public AssemblyDefinitionMetadata assembly;
        private bool _isDuplicate;
        private bool _isLevelIssue;
        private bool _outOfSorts;

        public bool HasIssue => isDuplicate || isLevelIssue || outOfSorts;
        
        public bool isDuplicate 
        {
            get     
            {
                return _isDuplicate;
            }
            set 
            {
                _isDuplicate = value;
            }
        }
        
        public bool isLevelIssue 
        {
            get     
            {
                return _isLevelIssue;
            }
            set 
            {
                _isLevelIssue = value;
            }
        }
        
        public bool outOfSorts 
        {
            get     
            {
                return _outOfSorts;
            }
            set 
            {
                _outOfSorts = value;
            }
        }
        
        public string guid;
        public Color IssueColor { get; set; }

        public bool IsGuidReference => guid.StartsWith(AssemblyDefinitionModel.GUID_PREFIX);

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

            return obj is AssemblyDefinitionReferenceMetadata other
                ? CompareTo(other)
                : throw new ArgumentException(
                    $"Object must be of type {nameof(AssemblyDefinitionReferenceMetadata)}"
                );
        }

        public int CompareTo(AssemblyDefinitionReferenceMetadata other)
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

        public bool Equals(AssemblyDefinitionReferenceMetadata other)
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

            return Equals((AssemblyDefinitionReferenceMetadata) obj);
        }

        public override int GetHashCode()
        {
            return guid != null ? guid.GetHashCode() : 0;
        }

        public override string ToString()
        {
            using (_PRF_ToString.Auto())
            {
                return assembly?.assembly_current ?? guid;
            }
        }

        public static bool operator ==(
            AssemblyDefinitionReferenceMetadata left,
            AssemblyDefinitionReferenceMetadata right)
        {
            return Equals(left, right);
        }

        public static bool operator >(
            AssemblyDefinitionReferenceMetadata left,
            AssemblyDefinitionReferenceMetadata right)
        {
            return Comparer<AssemblyDefinitionReferenceMetadata>.Default.Compare(left, right) > 0;
        }

        public static bool operator >=(
            AssemblyDefinitionReferenceMetadata left,
            AssemblyDefinitionReferenceMetadata right)
        {
            return Comparer<AssemblyDefinitionReferenceMetadata>.Default.Compare(left, right) >= 0;
        }

        public static bool operator !=(
            AssemblyDefinitionReferenceMetadata left,
            AssemblyDefinitionReferenceMetadata right)
        {
            return !Equals(left, right);
        }

        public static bool operator <(
            AssemblyDefinitionReferenceMetadata left,
            AssemblyDefinitionReferenceMetadata right)
        {
            return Comparer<AssemblyDefinitionReferenceMetadata>.Default.Compare(left, right) < 0;
        }

        public static bool operator <=(
            AssemblyDefinitionReferenceMetadata left,
            AssemblyDefinitionReferenceMetadata right)
        {
            return Comparer<AssemblyDefinitionReferenceMetadata>.Default.Compare(left, right) <= 0;
        }
    }
}
