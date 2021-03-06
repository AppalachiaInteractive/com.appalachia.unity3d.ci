using System;
using System.Collections.Generic;
using Appalachia.CI.Integration.Extensions;
using Appalachia.Utility.Extensions;
using Appalachia.Utility.Strings;
using Unity.Profiling;
using UnityEngine;

namespace Appalachia.CI.Integration.FileSystem
{
    [Serializable]
    public sealed class AssetPath : IEquatable<AssetPath>, IComparable<AssetPath>, IComparable
    {
        #region Constants and Static Readonly

        private const string ASSETS = "Assets";

        private const string EXCLUSION_SUBSTRING_1 = "~/";
        private const string EXCLUSION_SUBSTRING_2 = "~\\";

        private const string EXTENSION_CHECK_FORMAT_STRING = ".{0}";

        #endregion

        public AssetPath(string relativePath, string absolutePath)
        {
            using (_PRF_AssetPath.Auto())
            {
                _relativePath = relativePath;
                _absolutePath = absolutePath;
                _initialPath = _relativePath ?? _absolutePath;
            }
        }

        #region Fields and Autoproperties

        [SerializeField] private string _absolutePath;

        private string _directoryName;
        private string _extension;
        private string _fileName;
        private string _fileNameWithoutExtensions;

        [SerializeField] private string _initialPath;

        [SerializeField] private string _relativePath;

        private Utf8PreparedFormat<string> _extensionCheckFormat;

        #endregion

        public bool DirectoryDoesNotExist
        {
            get
            {
                using (_PRF_DirectoryDoesNotExist.Auto())
                {
                    return !DirectoryExists;
                }
            }
        }

        public bool DirectoryExists
        {
            get
            {
                using (_PRF_DirectoryExists.Auto())
                {
                    return AppaDirectory.Exists(EitherPath);
                }
            }
        }

        public bool FileDoesNotExist
        {
            get
            {
                using (_PRF_FileDoesNotExist.Auto())
                {
                    return !FileExists;
                }
            }
        }

        public bool FileExists
        {
            get
            {
                using (_PRF_FileExists.Auto())
                {
                    return AppaFile.Exists(EitherPath);
                }
            }
        }

        public bool IsAsset
        {
            get
            {
                using (_PRF_IsAsset.Auto())
                {
                    return RelativePath.StartsWith(ASSETS);
                }
            }
        }

        public bool IsExcluded
        {
            get
            {
                using (_PRF_IsExcluded.Auto())
                {
                    return EitherPath.Contains(EXCLUSION_SUBSTRING_1) ||
                           EitherPath.Contains(EXCLUSION_SUBSTRING_2);
                }
            }
        }

        public bool ParentDirectoryExists
        {
            get
            {
                using (_PRF_ParentDirectoryExists.Auto())
                {
                    var dirInfo = new AppaDirectoryInfo(DirectoryName);

                    return dirInfo.Exists;
                }
            }
        }

        public string AbsolutePath
        {
            get
            {
                using (_PRF_AbsolutePath.Auto())
                {
                    if ((_absolutePath == null) && (_relativePath != null))
                    {
                        _absolutePath = _relativePath.ToAbsolutePath().CleanFullPath();
                    }

                    return _absolutePath;
                }
            }
        }

        public string DirectoryName
        {
            get
            {
                using (_PRF_DirectoryName.Auto())
                {
                    if (_directoryName == null)
                    {
                        _directoryName = AppaPath.GetDirectoryName(EitherPath);
                    }

                    return _directoryName;
                }
            }
        }

        public string EitherPath => _absolutePath ?? _relativePath;

        public string Extension
        {
            get
            {
                using (_PRF_Extension.Auto())
                {
                    if (_extension == null)
                    {
                        _extension = AppaPath.GetExtension(EitherPath).CleanExtension();
                    }

                    return _extension;
                }
            }
        }

        public string FileName
        {
            get
            {
                using (_PRF_FileName.Auto())
                {
                    if (_fileName == null)
                    {
                        _fileName = AppaPath.GetFileName(EitherPath);
                    }

                    return _fileName;
                }
            }
        }

        public string FileNameWithoutExtension
        {
            get
            {
                using (_PRF_FileNameWithoutExtension.Auto())
                {
                    if (_fileNameWithoutExtensions == null)
                    {
                        _fileNameWithoutExtensions = AppaPath.GetFileNameWithoutExtension(EitherPath);
                    }

                    return _fileNameWithoutExtensions;
                }
            }
        }

        public string RelativePath
        {
            get
            {
                using (_PRF_RelativePath.Auto())
                {
                    if ((_relativePath == null) && (_absolutePath != null))
                    {
                        _relativePath = _absolutePath.ToRelativePath().CleanRelativePath();
                    }

                    return _relativePath;
                }
            }
        }

        public static AssetPath FromAbsolutePath(string absolutePath)
        {
            using (_PRF_FromAbsolutePath.Auto())
            {
                return new AssetPath(null, absolutePath);
            }
        }

        public static AssetPath FromRelativePath(string relativePath)
        {
            using (_PRF_FromRelativePath.Auto())
            {
                return new AssetPath(relativePath, null);
            }
        }

        public static bool operator ==(AssetPath left, AssetPath right)
        {
            using (_PRF_eq.Auto())
            {
                return Equals(left, right);
            }
        }

        public static bool operator >(AssetPath left, AssetPath right)
        {
            return Comparer<AssetPath>.Default.Compare(left, right) > 0;
        }

        public static bool operator >=(AssetPath left, AssetPath right)
        {
            return Comparer<AssetPath>.Default.Compare(left, right) >= 0;
        }

        public static bool operator !=(AssetPath left, AssetPath right)
        {
            using (_PRF_neq.Auto())
            {
                return !Equals(left, right);
            }
        }

        public static bool operator <(AssetPath left, AssetPath right)
        {
            return Comparer<AssetPath>.Default.Compare(left, right) < 0;
        }

        public static bool operator <=(AssetPath left, AssetPath right)
        {
            return Comparer<AssetPath>.Default.Compare(left, right) <= 0;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            using (_PRF_Equals.Auto())
            {
                return ReferenceEquals(this, obj) || (obj is AssetPath other && Equals(other));
            }
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            using (_PRF_GetHashCode.Auto())
            {
                return _initialPath != null ? _initialPath.GetHashCode() : 0;
            }
        }

        public bool HasExtension(string ext)
        {
            using (_PRF_HasExtension.Auto())
            {
                _extensionCheckFormat ??= new Utf8PreparedFormat<string>(EXTENSION_CHECK_FORMAT_STRING);

                return (Extension == ext) || (Extension == _extensionCheckFormat.Format(ext));
            }
        }

        #region IComparable Members

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

            return obj is AssetPath other
                ? CompareTo(other)
                : throw new ArgumentException($"Object must be of type {nameof(AssetPath)}");
        }

        #endregion

        #region IComparable<AssetPath> Members

        public int CompareTo(AssetPath other)
        {
            if (ReferenceEquals(this, other))
            {
                return 0;
            }

            if (ReferenceEquals(null, other))
            {
                return 1;
            }

            var relativePathComparison = string.Compare(
                _relativePath,
                other._relativePath,
                StringComparison.Ordinal
            );
            if (relativePathComparison != 0)
            {
                return relativePathComparison;
            }

            var absolutePathComparison = string.Compare(
                _absolutePath,
                other._absolutePath,
                StringComparison.Ordinal
            );
            if (absolutePathComparison != 0)
            {
                return absolutePathComparison;
            }

            var initialPathComparison = string.Compare(
                _initialPath,
                other._initialPath,
                StringComparison.Ordinal
            );
            if (initialPathComparison != 0)
            {
                return initialPathComparison;
            }

            var directoryNameComparison = string.Compare(
                _directoryName,
                other._directoryName,
                StringComparison.Ordinal
            );
            if (directoryNameComparison != 0)
            {
                return directoryNameComparison;
            }

            var extensionComparison = string.Compare(_extension, other._extension, StringComparison.Ordinal);
            if (extensionComparison != 0)
            {
                return extensionComparison;
            }

            var fileNameComparison = string.Compare(_fileName, other._fileName, StringComparison.Ordinal);
            if (fileNameComparison != 0)
            {
                return fileNameComparison;
            }

            return string.Compare(
                _fileNameWithoutExtensions,
                other._fileNameWithoutExtensions,
                StringComparison.Ordinal
            );
        }

        #endregion

        #region IEquatable<AssetPath> Members

        public bool Equals(AssetPath other)
        {
            using (_PRF_Equals.Auto())
            {
                if (ReferenceEquals(null, other))
                {
                    return false;
                }

                if (ReferenceEquals(this, other))
                {
                    return true;
                }

                return _initialPath == other._initialPath;
            }
        }

        #endregion

        #region Profiling

        private const string _PRF_PFX = nameof(AssetPath) + ".";

        private static readonly ProfilerMarker _PRF_eq = new ProfilerMarker(_PRF_PFX + "==");
        private static readonly ProfilerMarker _PRF_neq = new ProfilerMarker(_PRF_PFX + "!=");

        private static readonly ProfilerMarker _PRF_GetHashCode =
            new ProfilerMarker(_PRF_PFX + nameof(GetHashCode));

        private static readonly ProfilerMarker _PRF_Equals = new ProfilerMarker(_PRF_PFX + nameof(Equals));

        private static readonly ProfilerMarker _PRF_DirectoryName =
            new ProfilerMarker(_PRF_PFX + nameof(DirectoryName));

        private static readonly ProfilerMarker _PRF_IsExcluded =
            new ProfilerMarker(_PRF_PFX + nameof(IsExcluded));

        private static readonly ProfilerMarker _PRF_IsAsset = new ProfilerMarker(_PRF_PFX + nameof(IsAsset));

        private static readonly ProfilerMarker _PRF_Extension =
            new ProfilerMarker(_PRF_PFX + nameof(Extension));

        private static readonly ProfilerMarker _PRF_FileName =
            new ProfilerMarker(_PRF_PFX + nameof(FileName));

        private static readonly ProfilerMarker _PRF_FileNameWithoutExtension =
            new ProfilerMarker(_PRF_PFX + nameof(FileNameWithoutExtension));

        private static readonly ProfilerMarker _PRF_DirectoryDoesNotExist =
            new ProfilerMarker(_PRF_PFX + nameof(DirectoryDoesNotExist));

        private static readonly ProfilerMarker _PRF_DirectoryExists =
            new ProfilerMarker(_PRF_PFX + nameof(DirectoryExists));

        private static readonly ProfilerMarker _PRF_FileDoesNotExist =
            new ProfilerMarker(_PRF_PFX + nameof(FileDoesNotExist));

        private static readonly ProfilerMarker _PRF_FileExists =
            new ProfilerMarker(_PRF_PFX + nameof(FileExists));

        private static readonly ProfilerMarker _PRF_ParentDirectoryExists =
            new ProfilerMarker(_PRF_PFX + nameof(ParentDirectoryExists));

        private static readonly ProfilerMarker _PRF_HasExtension =
            new ProfilerMarker(_PRF_PFX + nameof(HasExtension));

        private static readonly ProfilerMarker _PRF_FromRelativePath =
            new ProfilerMarker(_PRF_PFX + nameof(FromRelativePath));

        private static readonly ProfilerMarker _PRF_FromAbsolutePath =
            new ProfilerMarker(_PRF_PFX + nameof(FromAbsolutePath));

        private static readonly ProfilerMarker _PRF_RelativePath =
            new ProfilerMarker(_PRF_PFX + nameof(RelativePath));

        private static readonly ProfilerMarker _PRF_AbsolutePath =
            new ProfilerMarker(_PRF_PFX + nameof(AbsolutePath));

        private static readonly ProfilerMarker _PRF_AssetPath =
            new ProfilerMarker(_PRF_PFX + nameof(AssetPath));

        #endregion
    }
}
