using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Appalachia.CI.Integration.Extensions;
using Appalachia.Utility.Logging;
using Unity.Profiling;

namespace Appalachia.CI.Integration.FileSystem
{
    public sealed class AppaDirectoryInfo : AppaFileSystemInfo
    {
#region Profiling And Tracing Markers

        private const string _PRF_PFX = nameof(AppaDirectoryInfo) + ".";

        private static readonly ProfilerMarker _PRF_Create = new(_PRF_PFX + nameof(Create));

        private static readonly ProfilerMarker _PRF_CreateSubdirectory =
            new(_PRF_PFX + nameof(CreateSubdirectory));

        private static readonly ProfilerMarker _PRF_GetFiles = new(_PRF_PFX + nameof(GetFiles));
        private static readonly ProfilerMarker _PRF_GetDirectories = new(_PRF_PFX + nameof(GetDirectories));

        private static readonly ProfilerMarker _PRF_GetFileSystemInfos =
            new(_PRF_PFX + nameof(GetFileSystemInfos));

        private static readonly ProfilerMarker _PRF_EnumerateDirectories =
            new(_PRF_PFX + nameof(EnumerateDirectories));

        private static readonly ProfilerMarker _PRF_EnumerateFiles = new(_PRF_PFX + nameof(EnumerateFiles));

        private static readonly ProfilerMarker _PRF_EnumerateFileSystemInfos =
            new(_PRF_PFX + nameof(EnumerateFileSystemInfos));

        private static readonly ProfilerMarker _PRF_MoveTo = new(_PRF_PFX + nameof(MoveTo));
        private static readonly ProfilerMarker _PRF_Delete = new(_PRF_PFX + nameof(Delete));

        private static readonly ProfilerMarker _PRF_ToString = new(_PRF_PFX + nameof(ToString));

        private static readonly ProfilerMarker _PRF_HasSiblingDirectory =
            new(_PRF_PFX + nameof(HasSiblingDirectory));

        private static readonly ProfilerMarker _PRF_HasSubDirectory = new(_PRF_PFX + nameof(HasSubDirectory));

        private static readonly ProfilerMarker _PRF_IsPathInAnySubdirectory =
            new(_PRF_PFX + nameof(IsPathInAnySubdirectory));

        private static readonly ProfilerMarker _PRF_IsFileInAnySubdirectory =
            new(_PRF_PFX + nameof(IsFileInAnySubdirectory));

        private static readonly ProfilerMarker _PRF_HasSiblingFile = new(_PRF_PFX + nameof(HasSiblingFile));

#endregion

        /// <summary>Initializes a new instance of the <see cref="T:Appalachia.CI.Integration.FileSystem.AppaDirectoryInfo" /> class on the specified path.</summary>
        /// <param name="path">A string specifying the path on which to create the <see langword="AppaDirectoryInfo" />. </param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> contains invalid characters such as ", &lt;, &gt;, or |.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters. The specified path, file name,
        ///     or both are too long.
        /// </exception>
        public AppaDirectoryInfo(string path)
        {
            try
            {
                _directoryInfo = new DirectoryInfo(path);
            }
            catch
            {
                AppaLog.Error(nameof(AppaDirectoryInfo) + ": " + path);
                throw;
            }
        }

        /// <summary>Initializes a new instance of the <see cref="T:Appalachia.CI.Integration.FileSystem.AppaDirectoryInfo" /> class on the directory.</summary>
        /// <param name="info">A directory.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="info" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="info" /> contains invalid characters such as ", &lt;, &gt;, or |.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters. The specified path, file name,
        ///     or both are too long.
        /// </exception>
        public AppaDirectoryInfo(DirectoryInfo info)
        {
            _directoryInfo = info;
        }

        private readonly DirectoryInfo _directoryInfo;

        /// <summary>Gets the root portion of the directory.</summary>
        /// <returns>An object that represents the root of the directory.</returns>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public AppaDirectoryInfo Root { get; }

        /// <summary>Gets the parent directory of a specified subdirectory.</summary>
        /// <returns>
        ///     The parent directory, or <see langword="null" /> if the path is null or if the file path denotes a root (such as "\", "C:", or *
        ///     "\\server\share").
        /// </returns>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public override AppaDirectoryInfo Parent
        {
            get
            {
                var parent = _directoryInfo.Parent;

                if (parent == null)
                {
                    return null;
                }

                var parentPath = parent.FullName.CleanFullPath();

                var projectRoot = ProjectLocations.GetProjectDirectoryPath();

                if (!parentPath.Contains(projectRoot))
                {
                    throw new NotSupportedException($"Didn't find project root for {_directoryInfo.Parent}");
                }

                return parent;
            }
        }

        /// <summary>Gets a value indicating whether the directory exists.</summary>
        /// <returns>
        ///     <see langword="true" /> if the directory exists; otherwise, <see langword="false" />.
        /// </returns>
        public override bool Exists => _directoryInfo.Exists;

        /// <summary>Gets the full path of the directory.</summary>
        /// <returns>A string containing the full path.</returns>
        public override string FullPath => _directoryInfo.FullName.CleanFullPath();

        /// <summary>Gets the name of this <see cref="T:Appalachia.CI.Integration.FileSystem.AppaDirectoryInfo" /> instance.</summary>
        /// <returns>The directory name.</returns>
        public override string Name => _directoryInfo.Name;

        public override string RelativePath => FullPath.ToRelativePath();

        /// <summary>Creates a directory.</summary>
        /// <exception cref="T:System.IO.IOException">The directory cannot be created. </exception>
        public void Create()
        {
            using (_PRF_Create.Auto())
            {
                _directoryInfo.Create();
            }
        }

        /// <summary>
        ///     Creates a subdirectory or subdirectories on the specified path. The specified path can be relative to this instance of the
        ///     <see cref="T:Appalachia.CI.Integration.FileSystem.AppaDirectoryInfo" /> class.
        /// </summary>
        /// <param name="path">The specified path. This cannot be a different disk volume or Universal Naming Convention (UNC) name. </param>
        /// <returns>The last directory specified in <paramref name="path" />.</returns>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> does not specify a valid file path or contains invalid <see langword="AppaDirectoryInfo" /> characters.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive. </exception>
        /// <exception cref="T:System.IO.IOException">
        ///     The subdirectory cannot be created.-or- A file or directory already has the name specified by
        ///     <paramref name="path" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters. The specified path, file name,
        ///     or both are too long.
        /// </exception>
        /// <exception cref="T:System.Security.SecurityException">
        ///     The caller does not have code access permission to create the directory.-or-The caller does not
        ///     have code access permission to read the directory described by the returned
        ///     <see cref="T:Appalachia.CI.Integration.FileSystem.AppaDirectoryInfo" /> object.  This can occur
        ///     when the <paramref name="path" /> parameter describes an existing directory.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     <paramref name="path" /> contains a colon character (:) that is not part of a drive label ("C:\").
        /// </exception>
        public AppaDirectoryInfo CreateSubdirectory(string path)
        {
            using (_PRF_CreateSubdirectory.Auto())
            {
                return _directoryInfo.CreateSubdirectory(path);
            }
        }

        /// <summary>
        ///     Deletes this instance of a <see cref="T:Appalachia.CI.Integration.FileSystem.AppaDirectoryInfo" />, specifying whether to delete
        ///     subdirectories and files.
        /// </summary>
        /// <param name="recursive">
        ///     <see langword="true" /> to delete this directory, its subdirectories, and all files; otherwise, <see langword="false" />.
        /// </param>
        /// <exception cref="T:System.UnauthorizedAccessException">The directory contains a read-only file.</exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">
        ///     The directory described by this <see cref="T:Appalachia.CI.Integration.FileSystem.AppaDirectoryInfo" /> object does not
        ///     exist or could not be found.
        /// </exception>
        /// <exception cref="T:System.IO.IOException">
        ///     The directory is read-only.-or- The directory contains one or more files or subdirectories and
        ///     <paramref name="recursive" /> is <see langword="false" />.-or-The directory is the application's current working directory. -or-There is an open
        ///     handle on the directory or on one of its files, and the operating system is Windows XP or earlier. This open handle can result from enumerating
        ///     directories and files. For more information, see How to: Enumerate Directories and Files.
        /// </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public void Delete(bool recursive)
        {
            using (_PRF_Delete.Auto())
            {
                _directoryInfo.Delete(recursive);
            }
        }

        /// <summary>Returns an enumerable collection of directory information in the current directory.</summary>
        /// <returns>An enumerable collection of directories in the current directory.</returns>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">
        ///     The path encapsulated in the <see cref="T:Appalachia.CI.Integration.FileSystem.AppaDirectoryInfo" /> object is invalid
        ///     (for example, it is on an unmapped drive).
        /// </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public IEnumerable<AppaDirectoryInfo> EnumerateDirectories()
        {
            using (_PRF_EnumerateDirectories.Auto())
            {
                return _directoryInfo.EnumerateDirectories().Select(d => (AppaDirectoryInfo) d);
            }
        }

        /// <summary>Returns an enumerable collection of directory information that matches a specified search pattern.</summary>
        /// <param name="searchPattern">
        ///     The search string to match against the names of directories.  This parameter can contain a combination of valid literal
        ///     path and wildcard (* and ?) characters (see Remarks), but doesn't support regular expressions. The default pattern is "*", which returns all
        ///     files.
        /// </param>
        /// <returns>An enumerable collection of directories that matches <paramref name="searchPattern" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="searchPattern" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">
        ///     The path encapsulated in the <see cref="T:Appalachia.CI.Integration.FileSystem.AppaDirectoryInfo" /> object is invalid
        ///     (for example, it is on an unmapped drive).
        /// </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public IEnumerable<AppaDirectoryInfo> EnumerateDirectories(string searchPattern)
        {
            using (_PRF_EnumerateDirectories.Auto())
            {
                return _directoryInfo.EnumerateDirectories(searchPattern).Select(d => (AppaDirectoryInfo) d);
            }
        }

        /// <summary>Returns an enumerable collection of directory information that matches a specified search pattern and search subdirectory option. </summary>
        /// <param name="searchPattern">
        ///     The search string to match against the names of directories.  This parameter can contain a combination of valid literal
        ///     path and wildcard (* and ?) characters (see Remarks), but doesn't support regular expressions. The default pattern is "*", which returns all
        ///     files.
        /// </param>
        /// <param name="searchOption">
        ///     One of the enumeration values that specifies whether the search operation should include only the current directory or all
        ///     subdirectories. The default value is <see cref="F:System.IO.SearchOption.TopDirectoryOnly" />.
        /// </param>
        /// <returns>An enumerable collection of directories that matches <paramref name="searchPattern" /> and <paramref name="searchOption" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="searchPattern" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="searchOption" /> is not a valid <see cref="T:System.IO.SearchOption" /> value.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">
        ///     The path encapsulated in the <see cref="T:Appalachia.CI.Integration.FileSystem.AppaDirectoryInfo" /> object is invalid
        ///     (for example, it is on an unmapped drive).
        /// </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public IEnumerable<AppaDirectoryInfo> EnumerateDirectories(
            string searchPattern,
            SearchOption searchOption)
        {
            using (_PRF_EnumerateDirectories.Auto())
            {
                return _directoryInfo.EnumerateDirectories(searchPattern, searchOption)
                                     .Select(d => (AppaDirectoryInfo) d);
            }
        }

        /// <summary>Returns an enumerable collection of file information in the current directory.</summary>
        /// <returns>An enumerable collection of the files in the current directory.</returns>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">
        ///     The path encapsulated in the <see cref="T:Appalachia.CI.Integration.FileSystem.AppaDirectoryInfo" /> object is invalid
        ///     (for example, it is on an unmapped drive).
        /// </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public IEnumerable<AppaFileInfo> EnumerateFiles()
        {
            using (_PRF_EnumerateFiles.Auto())
            {
                return _directoryInfo.EnumerateFiles().Select(fs => (AppaFileInfo) fs);
            }
        }

        /// <summary>Returns an enumerable collection of file information that matches a search pattern.</summary>
        /// <param name="searchPattern">
        ///     The search string to match against the names of files.  This parameter can contain a combination of valid literal path
        ///     and wildcard (* and ?) characters (see Remarks), but doesn't support regular expressions. The default pattern is "*", which returns all files.
        /// </param>
        /// <returns>An enumerable collection of files that matches <paramref name="searchPattern" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="searchPattern" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">
        ///     The path encapsulated in the <see cref="T:Appalachia.CI.Integration.FileSystem.AppaDirectoryInfo" /> object is invalid,
        ///     (for example, it is on an unmapped drive).
        /// </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public IEnumerable<AppaFileInfo> EnumerateFiles(string searchPattern)
        {
            using (_PRF_EnumerateFiles.Auto())
            {
                return _directoryInfo.EnumerateFiles(searchPattern).Select(fs => (AppaFileInfo) fs);
            }
        }

        /// <summary>Returns an enumerable collection of file information that matches a specified search pattern and search subdirectory option.</summary>
        /// <param name="searchPattern">
        ///     The search string to match against the names of files.  This parameter can contain a combination of valid literal path
        ///     and wildcard (* and ?) characters (see Remarks), but doesn't support regular expressions. The default pattern is "*", which returns all files.
        /// </param>
        /// <param name="searchOption">
        ///     One of the enumeration values that specifies whether the search operation should include only the current directory or all
        ///     subdirectories. The default value is <see cref="F:System.IO.SearchOption.TopDirectoryOnly" />.
        /// </param>
        /// <returns>An enumerable collection of files that matches <paramref name="searchPattern" /> and <paramref name="searchOption" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="searchPattern" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="searchOption" /> is not a valid <see cref="T:System.IO.SearchOption" /> value.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">
        ///     The path encapsulated in the <see cref="T:Appalachia.CI.Integration.FileSystem.AppaDirectoryInfo" /> object is invalid
        ///     (for example, it is on an unmapped drive).
        /// </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public IEnumerable<AppaFileInfo> EnumerateFiles(string searchPattern, SearchOption searchOption)
        {
            using (_PRF_EnumerateFiles.Auto())
            {
                return _directoryInfo.EnumerateFiles(searchPattern, searchOption)
                                     .Select(fs => (AppaFileInfo) fs);
            }
        }

        /// <summary>Returns an enumerable collection of file system information in the current directory.</summary>
        /// <returns>An enumerable collection of file system information in the current directory. </returns>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">
        ///     The path encapsulated in the <see cref="T:Appalachia.CI.Integration.FileSystem.AppaDirectoryInfo" /> object is invalid
        ///     (for example, it is on an unmapped drive).
        /// </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public IEnumerable<AppaFileSystemInfo> EnumerateFileSystemInfos()
        {
            using (_PRF_EnumerateFileSystemInfos.Auto())
            {
                return _directoryInfo.EnumerateFileSystemInfos().Select(fs => (AppaFileSystemInfo) fs);
            }
        }

        /// <summary>Returns an enumerable collection of file system information that matches a specified search pattern.</summary>
        /// <param name="searchPattern">
        ///     The search string to match against the names of directories.  This parameter can contain a combination of valid literal
        ///     path and wildcard (* and ?) characters (see Remarks), but doesn't support regular expressions. The default pattern is "*", which returns all
        ///     files.
        /// </param>
        /// <returns>An enumerable collection of file system information objects that matches <paramref name="searchPattern" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="searchPattern" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">
        ///     The path encapsulated in the <see cref="T:Appalachia.CI.Integration.FileSystem.AppaDirectoryInfo" /> object is invalid
        ///     (for example, it is on an unmapped drive).
        /// </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public IEnumerable<AppaFileSystemInfo> EnumerateFileSystemInfos(string searchPattern)
        {
            using (_PRF_EnumerateFileSystemInfos.Auto())
            {
                return _directoryInfo.EnumerateFileSystemInfos(searchPattern)
                                     .Select(fs => (AppaFileSystemInfo) fs);
            }
        }

        /// <summary>Returns an enumerable collection of file system information that matches a specified search pattern and search subdirectory option.</summary>
        /// <param name="searchPattern">
        ///     The search string to match against the names of directories.  This parameter can contain a combination of valid literal
        ///     path and wildcard (* and ?) characters (see Remarks), but doesn't support regular expressions. The default pattern is "*", which returns all
        ///     files.
        /// </param>
        /// <param name="searchOption">
        ///     One of the enumeration values that specifies whether the search operation should include only the current directory or all
        ///     subdirectories. The default value is <see cref="F:System.IO.SearchOption.TopDirectoryOnly" />.
        /// </param>
        /// <returns>
        ///     An enumerable collection of file system information objects that matches <paramref name="searchPattern" /> and
        ///     <paramref name="searchOption" />.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="searchPattern" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="searchOption" /> is not a valid <see cref="T:System.IO.SearchOption" /> value.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">
        ///     The path encapsulated in the <see cref="T:Appalachia.CI.Integration.FileSystem.AppaDirectoryInfo" /> object is invalid
        ///     (for example, it is on an unmapped drive).
        /// </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public IEnumerable<AppaFileSystemInfo> EnumerateFileSystemInfos(
            string searchPattern,
            SearchOption searchOption)
        {
            using (_PRF_EnumerateFileSystemInfos.Auto())
            {
                return _directoryInfo.EnumerateFileSystemInfos(searchPattern, searchOption)
                                     .Select(fs => (AppaFileSystemInfo) fs);
            }
        }

        /// <summary>Returns the subdirectories of the current directory.</summary>
        /// <returns>An array of <see cref="T:Appalachia.CI.Integration.FileSystem.AppaDirectoryInfo" /> objects.</returns>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">
        ///     The path encapsulated in the <see cref="T:Appalachia.CI.Integration.FileSystem.AppaDirectoryInfo" /> object is invalid,
        ///     such as being on an unmapped drive.
        /// </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. </exception>
        public AppaDirectoryInfo[] GetDirectories()
        {
            using (_PRF_GetDirectories.Auto())
            {
                var x = _directoryInfo.GetDirectories();
                var n = new AppaDirectoryInfo[x.Length];

                for (var i = 0; i < x.Length; i++)
                {
                    n[i] = x[i];
                }

                return n;
            }
        }

        /// <summary>
        ///     Returns an array of directories in the current <see cref="T:Appalachia.CI.Integration.FileSystem.AppaDirectoryInfo" /> matching the given
        ///     search criteria.
        /// </summary>
        /// <param name="searchPattern">
        ///     The search string to match against the names of directories.  This parameter can contain a combination of valid literal
        ///     path and wildcard (* and ?) characters (see Remarks), but doesn't support regular expressions. The default pattern is "*", which returns all
        ///     files.
        /// </param>
        /// <returns>An array of type <see langword="AppaDirectoryInfo" /> matching <paramref name="searchPattern" />.</returns>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="searchPattern " />contains one or more invalid characters defined by the <see cref="M:System.IO.Path.GetInvalidPathChars" />
        ///     method.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="searchPattern" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">
        ///     The path encapsulated in the <see langword="AppaDirectoryInfo" /> object is invalid (for
        ///     example, it is on an unmapped drive).
        /// </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. </exception>
        public AppaDirectoryInfo[] GetDirectories(string searchPattern)
        {
            using (_PRF_GetDirectories.Auto())
            {
                var x = _directoryInfo.GetDirectories(searchPattern);
                var n = new AppaDirectoryInfo[x.Length];

                for (var i = 0; i < x.Length; i++)
                {
                    n[i] = x[i];
                }

                return n;
            }
        }

        /// <summary>
        ///     Returns an array of directories in the current <see cref="T:Appalachia.CI.Integration.FileSystem.AppaDirectoryInfo" /> matching the given search
        ///     criteria and using a
        ///     value to determine whether to search subdirectories.
        /// </summary>
        /// <param name="searchPattern">
        ///     The search string to match against the names of directories.  This parameter can contain a combination of valid literal
        ///     path and wildcard (* and ?) characters (see Remarks), but doesn't support regular expressions. The default pattern is "*", which returns all
        ///     files.
        /// </param>
        /// <param name="searchOption">
        ///     One of the enumeration values that specifies whether the search operation should include only the current directory or all
        ///     subdirectories.
        /// </param>
        /// <returns>An array of type <see langword="AppaDirectoryInfo" /> matching <paramref name="searchPattern" />.</returns>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="searchPattern " />contains one or more invalid characters defined by the <see cref="M:System.IO.Path.GetInvalidPathChars" />
        ///     method.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="searchPattern" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="searchOption" /> is not a valid <see cref="T:System.IO.SearchOption" /> value.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">
        ///     The path encapsulated in the <see langword="AppaDirectoryInfo" /> object is invalid (for
        ///     example, it is on an unmapped drive).
        /// </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. </exception>
        public AppaDirectoryInfo[] GetDirectories(string searchPattern, SearchOption searchOption)
        {
            using (_PRF_GetDirectories.Auto())
            {
                var x = _directoryInfo.GetDirectories(searchPattern, searchOption);
                var n = new AppaDirectoryInfo[x.Length];

                for (var i = 0; i < x.Length; i++)
                {
                    n[i] = x[i];
                }

                return n;
            }
        }

        /// <summary>Returns a file list from the current directory matching the given search pattern.</summary>
        /// <param name="searchPattern">
        ///     The search string to match against the names of files.  This parameter can contain a combination of valid literal path
        ///     and wildcard (* and ?) characters (see Remarks), but doesn't support regular expressions. The default pattern is "*", which returns all files.
        /// </param>
        /// <returns>An array of type <see cref="T:Appalachia.CI.Integration.FileSystem.AppaFileInfo" />.</returns>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="searchPattern " />contains one or more invalid characters defined by the <see cref="M:System.IO.Path.GetInvalidPathChars" />
        ///     method.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="searchPattern" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The path is invalid (for example, it is on an unmapped drive). </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public AppaFileInfo[] GetFiles(string searchPattern)
        {
            using (_PRF_GetFiles.Auto())
            {
                var x = _directoryInfo.GetFiles(searchPattern);
                var n = new AppaFileInfo[x.Length];

                for (var i = 0; i < x.Length; i++)
                {
                    n[i] = x[i];
                }

                return n;
            }
        }

        /// <summary>
        ///     Returns a file list from the current directory matching the given search pattern and using a value to determine whether to search
        ///     subdirectories.
        /// </summary>
        /// <param name="searchPattern">
        ///     The search string to match against the names of files.  This parameter can contain a combination of valid literal path
        ///     and wildcard (* and ?) characters (see Remarks), but doesn't support regular expressions. The default pattern is "*", which returns all files.
        /// </param>
        /// <param name="searchOption">
        ///     One of the enumeration values that specifies whether the search operation should include only the current directory or all
        ///     subdirectories.
        /// </param>
        /// <returns>An array of type <see cref="T:Appalachia.CI.Integration.FileSystem.AppaFileInfo" />.</returns>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="searchPattern " />contains one or more invalid characters defined by the <see cref="M:System.IO.Path.GetInvalidPathChars" />
        ///     method.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="searchPattern" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="searchOption" /> is not a valid <see cref="T:System.IO.SearchOption" /> value.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The path is invalid (for example, it is on an unmapped drive). </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public AppaFileInfo[] GetFiles(string searchPattern, SearchOption searchOption)
        {
            using (_PRF_GetFiles.Auto())
            {
                var x = _directoryInfo.GetFiles(searchPattern, searchOption);
                var n = new AppaFileInfo[x.Length];

                for (var i = 0; i < x.Length; i++)
                {
                    n[i] = x[i];
                }

                return n;
            }
        }

        /// <summary>Returns a file list from the current directory.</summary>
        /// <returns>An array of type <see cref="T:Appalachia.CI.Integration.FileSystem.AppaFileInfo" />.</returns>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The path is invalid, such as being on an unmapped drive. </exception>
        public AppaFileInfo[] GetFiles()
        {
            using (_PRF_GetFiles.Auto())
            {
                var x = _directoryInfo.GetFiles();
                var n = new AppaFileInfo[x.Length];

                for (var i = 0; i < x.Length; i++)
                {
                    n[i] = x[i];
                }

                return n;
            }
        }

        /// <summary>
        ///     Retrieves an array of strongly typed <see cref="T:Appalachia.CI.Integration.FileSystem.FileSystemInfo" /> objects representing the files and
        ///     subdirectories that match
        ///     the specified search criteria.
        /// </summary>
        /// <param name="searchPattern">
        ///     The search string to match against the names of directories and files.  This parameter can contain a combination of valid
        ///     literal path and wildcard (* and ?) characters (see Remarks), but doesn't support regular expressions. The default pattern is "*", which returns
        ///     all files.
        /// </param>
        /// <returns>An array of strongly typed <see langword="FileSystemInfo" /> objects matching the search criteria.</returns>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="searchPattern " />contains one or more invalid characters defined by the <see cref="M:System.IO.Path.GetInvalidPathChars" />
        ///     method.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="searchPattern" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive). </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public AppaFileSystemInfo[] GetFileSystemInfos(string searchPattern)
        {
            using (_PRF_GetFileSystemInfos.Auto())
            {
                var x = _directoryInfo.GetFileSystemInfos(searchPattern);
                var n = new AppaFileSystemInfo[x.Length];

                for (var i = 0; i < x.Length; i++)
                {
                    n[i] = x[i];
                }

                return n;
            }
        }

        /// <summary>
        ///     Retrieves an array of <see cref="T:Appalachia.CI.Integration.FileSystem.FileSystemInfo" /> objects that represent the files and subdirectories
        ///     matching the specified
        ///     search criteria.
        /// </summary>
        /// <param name="searchPattern">
        ///     The search string to match against the names of directories and filesa.  This parameter can contain a combination of
        ///     valid literal path and wildcard (* and ?) characters (see Remarks), but doesn't support regular expressions. The default pattern is "*", which
        ///     returns all files.
        /// </param>
        /// <param name="searchOption">
        ///     One of the enumeration values that specifies whether the search operation should include only the current directory or all
        ///     subdirectories. The default value is <see cref="F:System.IO.SearchOption.TopDirectoryOnly" />.
        /// </param>
        /// <returns>An array of file system entries that match the search criteria.</returns>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="searchPattern " />contains one or more invalid characters defined by the <see cref="M:System.IO.Path.GetInvalidPathChars" />
        ///     method.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="searchPattern" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="searchOption" /> is not a valid <see cref="T:System.IO.SearchOption" /> value.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive). </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public AppaFileSystemInfo[] GetFileSystemInfos(string searchPattern, SearchOption searchOption)
        {
            using (_PRF_GetFileSystemInfos.Auto())
            {
                var x = _directoryInfo.GetFileSystemInfos(searchPattern, searchOption);
                var n = new AppaFileSystemInfo[x.Length];

                for (var i = 0; i < x.Length; i++)
                {
                    n[i] = x[i];
                }

                return n;
            }
        }

        /// <summary>
        ///     Returns an array of strongly typed <see cref="T:Appalachia.CI.Integration.FileSystem.FileSystemInfo" /> entries representing all the files and
        ///     subdirectories in a
        ///     directory.
        /// </summary>
        /// <returns>An array of strongly typed <see cref="T:Appalachia.CI.Integration.FileSystem.FileSystemInfo" /> entries.</returns>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The path is invalid (for example, it is on an unmapped drive). </exception>
        public AppaFileSystemInfo[] GetFileSystemInfos()
        {
            using (_PRF_GetFileSystemInfos.Auto())
            {
                var x = _directoryInfo.GetFileSystemInfos();
                var n = new AppaFileSystemInfo[x.Length];

                for (var i = 0; i < x.Length; i++)
                {
                    n[i] = x[i];
                }

                return n;
            }
        }

        public bool HasSiblingDirectory(string name, out AppaDirectoryInfo siblingDirectory)
        {
            using (_PRF_HasSiblingDirectory.Auto())
            {
                foreach (var sibling in Parent.GetDirectories())
                {
                    if (sibling.Name == name)
                    {
                        siblingDirectory = sibling;
                        return true;
                    }
                }

                siblingDirectory = null;
                return false;
            }
        }

        public bool HasSiblingFile(string name, out AppaFileInfo siblingFile)
        {
            using (_PRF_HasSiblingFile.Auto())
            {
                foreach (var sibling in Parent.EnumerateFiles())
                {
                    if (sibling.Name == name)
                    {
                        siblingFile = sibling;
                        return true;
                    }
                }

                siblingFile = null;
                return false;
            }
        }

        public bool HasSubDirectory(string name)
        {
            using (_PRF_HasSubDirectory.Auto())
            {
                foreach (var child in EnumerateDirectories())
                {
                    if (child.Name == name)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public bool IsFileInAnySubdirectory(AppaFileInfo file)
        {
            using (_PRF_IsFileInAnySubdirectory.Auto())
            {
                return IsPathInAnySubdirectory(file.FullPath);
            }
        }

        public bool IsPathInAnySubdirectory(string path)
        {
            using (_PRF_IsPathInAnySubdirectory.Auto())
            {
                var lastIndex = path.LastIndexOf('/');

                var subset = path.Substring(0, lastIndex).Trim('/');

                var contains = subset.Contains(FullPath);

                return contains;
            }
        }

        /// <summary>Moves a <see cref="T:Appalachia.CI.Integration.FileSystem.AppaDirectoryInfo" /> instance and its contents to a new path.</summary>
        /// <param name="destDirName">
        ///     The name and path to which to move this directory. The destination cannot be another disk volume or a directory with the
        ///     identical name. It can be an existing directory to which you want to add this directory as a subdirectory.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="destDirName" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="destDirName" /> is an empty string (''").
        /// </exception>
        /// <exception cref="T:System.IO.IOException">
        ///     An attempt was made to move a directory to a different volume. -or-
        ///     <paramref name="destDirName" /> already exists.-or-You are not authorized to access this path.-or- The directory being moved and the destination
        ///     directory have the same name.
        /// </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The destination directory cannot be found.</exception>
        public void MoveTo(string destDirName)
        {
            using (_PRF_MoveTo.Auto())
            {
                _directoryInfo.MoveTo(destDirName);
            }
        }

        /// <summary>Deletes this <see cref="T:Appalachia.CI.Integration.FileSystem.AppaDirectoryInfo" /> if it is empty.</summary>
        /// <exception cref="T:System.UnauthorizedAccessException">The directory contains a read-only file.</exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">
        ///     The directory described by this <see cref="T:Appalachia.CI.Integration.FileSystem.AppaDirectoryInfo" /> object does not
        ///     exist or could not be found.
        /// </exception>
        /// <exception cref="T:System.IO.IOException">
        ///     The directory is not empty. -or-The directory is the application's current working directory.-or-There is
        ///     an open handle on the directory, and the operating system is Windows XP or earlier. This open handle can result from enumerating directories. For
        ///     more information, see How to: Enumerate Directories and Files.
        /// </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public override void Delete()
        {
            using (_PRF_Delete.Auto())
            {
                _directoryInfo.Delete();
            }
        }

        /// <summary>Returns the original path that was passed by the user.</summary>
        /// <returns>Returns the original path that was passed by the user.</returns>
        public override string ToString()
        {
            using (_PRF_ToString.Auto())
            {
                return _directoryInfo.ToString().CleanFullPath();
            }
        }

        protected override FileSystemInfo GetFileSystemInfo()
        {
            return _directoryInfo;
        }

        public static implicit operator AppaDirectoryInfo(DirectoryInfo o)
        {
            return new(o);
        }

        public static implicit operator DirectoryInfo(AppaDirectoryInfo o)
        {
            return o._directoryInfo;
        }

        public bool IsInRepositoryRoot()
        {
            var gitFolder = HasSiblingDirectory(".git", out _);
            var packageJson = HasSiblingFile("package.json", out _);

            return gitFolder || packageJson;
        }
    }
}
