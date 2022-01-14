using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Appalachia.CI.Constants;
using Appalachia.CI.Integration.Extensions;
using Appalachia.Utility.Strings;
using Unity.Profiling;

namespace Appalachia.CI.Integration.FileSystem
{
    public static class AppaDirectory
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
                    _context = new AppaContext(typeof(AppaDirectory));
                }

                return _context;
            }
        }

        /// <summary>Creates all directories and subdirectories in the specified path unless they already exist.</summary>
        /// <param name="path">The directory to create. </param>
        /// <returns>
        ///     An object that represents the directory at the specified path. This object is returned regardless of whether a directory at the specified
        ///     path already exists.
        /// </returns>
        /// <exception cref="T:System.IO.IOException">The directory specified by <paramref name="path" /> is a file.-or-The network name is not known.</exception>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters. You can query for
        ///     invalid characters by using the <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.-or-
        ///     <paramref name="path" /> is prefixed with, or contains, only a colon character (:).
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive). </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     <paramref name="path" /> contains a colon character (:) that is not part of a drive label ("C:\").
        /// </exception>
        public static AppaDirectoryInfo CreateDirectory(string path)
        {
            using (_PRF_CreateDirectory.Auto())
            {
                return Directory.CreateDirectory(path);
            }
        }

        /// <summary>Creates all directories and subdirectories in the specified path unless they already exist.</summary>
        /// <param name="path">The directory to create. </param>
        /// <returns>
        ///     An object that represents the directory at the specified path. This object is returned regardless of whether a directory at the specified
        ///     path already exists.
        /// </returns>
        /// <exception cref="T:System.IO.IOException">The directory specified by <paramref name="path" /> is a file.-or-The network name is not known.</exception>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters. You can query for
        ///     invalid characters by using the <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.-or-
        ///     <paramref name="path" /> is prefixed with, or contains, only a colon character (:).
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive). </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     <paramref name="path" /> contains a colon character (:) that is not part of a drive label ("C:\").
        /// </exception>
        public static AppaDirectoryInfo CreateDirectoryStructureForFilePath(string path)
        {
            using (_PRF_CreateDirectoryStructureForFilePath.Auto())
            {
                var directory = GetParent(path);

                if (!directory.Exists)
                {
                    directory.Create();
                }

                return directory;
            }
        }

        /// <summary>Deletes an empty directory from a specified path.</summary>
        /// <param name="path">The name of the empty directory to remove. This directory must be writable and empty. </param>
        /// <exception cref="T:System.IO.IOException">
        ///     A file with the same name and location specified by <paramref name="path" /> exists.-or-The directory is
        ///     the application's current working directory.-or-The directory specified by <paramref name="path" /> is not empty.-or-The directory is read-only
        ///     or contains a read-only file.-or-The directory is being used by another process.
        /// </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters. You can query for
        ///     invalid characters by using the <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">
        ///     <paramref name="path" /> does not exist or could not be found.-or-The specified path is invalid (for example, it is on an unmapped drive).
        /// </exception>
        public static void Delete(string path)
        {
            using (_PRF_Delete.Auto())
            {
                Directory.Delete(path);
            }
        }

        /// <summary>Deletes the specified directory and, if indicated, any subdirectories and files in the directory. </summary>
        /// <param name="path">The name of the directory to remove. </param>
        /// <param name="recursive">
        ///     <see langword="true" /> to remove directories, subdirectories, and files in <paramref name="path" />; otherwise, <see langword="false" />.
        /// </param>
        /// <exception cref="T:System.IO.IOException">
        ///     A file with the same name and location specified by <paramref name="path" /> exists.-or-The directory
        ///     specified by <paramref name="path" /> is read-only, or <paramref name="recursive" /> is <see langword="false" /> and <paramref name="path" /> is
        ///     not an empty directory. -or-The directory is the application's current working directory. -or-The directory contains a read-only file.-or-The
        ///     directory is being used by another process.
        /// </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission.</exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters. You can query for
        ///     invalid characters by using the <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">
        ///     <paramref name="path" /> does not exist or could not be found.-or-The specified path is invalid (for example, it is on an unmapped drive).
        /// </exception>
        public static void Delete(string path, bool recursive)
        {
            using (_PRF_Delete.Auto())
            {
                Directory.Delete(path, recursive);
            }
        }

        /// <summary>Returns an enumerable collection of directory names in a specified path.</summary>
        /// <param name="path">The relative or absolute path to the directory to search. This string is not case-sensitive.</param>
        /// <returns>An enumerable collection of the full names (including paths) for the directories in the directory specified by <paramref name="path" />.</returns>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path " />is a zero-length string, contains only white space, or contains invalid characters. You can query for invalid characters
        ///     by using the <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">
        ///     <paramref name="path" /> is invalid, such as referring to an unmapped drive.
        /// </exception>
        /// <exception cref="T:System.IO.IOException">
        ///     <paramref name="path" /> is a file name.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or combined exceed the system-defined maximum length. For example,
        ///     on Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission.</exception>
        public static IEnumerable<string> EnumerateDirectories(string path)
        {
            using (_PRF_EnumerateDirectories.Auto())
            {
                return Directory.EnumerateDirectories(path);
            }
        }

        /// <summary>Returns an enumerable collection of directory names that match a search pattern in a specified path.</summary>
        /// <param name="path">The relative or absolute path to the directory to search. This string is not case-sensitive.</param>
        /// <param name="searchPattern">
        ///     The search string to match against the names of directories in <paramref name="path" />.  This parameter can contain a
        ///     combination of valid literal path and wildcard (* and ?) characters (see Remarks), but doesn't support regular expressions.
        /// </param>
        /// <returns>
        ///     An enumerable collection of the full names (including paths) for the directories in the directory specified by <paramref name="path" /> and
        ///     that match the specified search pattern.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path " />is a zero-length string, contains only white space, or contains invalid characters. You can query for invalid characters
        ///     with the  <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.- or -
        ///     <paramref name="searchPattern" /> does not contain a valid pattern.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.-or-
        ///     <paramref name="searchPattern" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">
        ///     <paramref name="path" /> is invalid, such as referring to an unmapped drive.
        /// </exception>
        /// <exception cref="T:System.IO.IOException">
        ///     <paramref name="path" /> is a file name.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or combined exceed the system-defined maximum length. For example,
        ///     on Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission.</exception>
        public static IEnumerable<string> EnumerateDirectories(string path, string searchPattern)
        {
            using (_PRF_EnumerateDirectories.Auto())
            {
                return Directory.EnumerateDirectories(path, searchPattern);
            }
        }

        /// <summary>Returns an enumerable collection of directory names that match a search pattern in a specified path, and optionally searches subdirectories.</summary>
        /// <param name="path">The relative or absolute path to the directory to search. This string is not case-sensitive.</param>
        /// <param name="searchPattern">
        ///     The search string to match against the names of directories in <paramref name="path" />.  This parameter can contain a
        ///     combination of valid literal path and wildcard (* and ?) characters (see Remarks), but doesn't support regular expressions.
        /// </param>
        /// <param name="searchOption">
        ///     One of the enumeration values that specifies whether the search operation should include only the current directory or
        ///     should include all subdirectories.The default value is <see cref="F:System.IO.SearchOption.TopDirectoryOnly" />.
        /// </param>
        /// <returns>
        ///     An enumerable collection of the full names (including paths) for the directories in the directory specified by <paramref name="path" /> and
        ///     that match the specified search pattern and option.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path " />is a zero-length string, contains only white space, or contains invalid characters. You can query for invalid characters
        ///     by using the  <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.- or -
        ///     <paramref name="searchPattern" /> does not contain a valid pattern.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.-or-
        ///     <paramref name="searchPattern" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="searchOption" /> is not a valid <see cref="T:System.IO.SearchOption" /> value.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">
        ///     <paramref name="path" /> is invalid, such as referring to an unmapped drive.
        /// </exception>
        /// <exception cref="T:System.IO.IOException">
        ///     <paramref name="path" /> is a file name.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or combined exceed the system-defined maximum length. For example,
        ///     on Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission.</exception>
        public static IEnumerable<string> EnumerateDirectories(
            string path,
            string searchPattern,
            SearchOption searchOption)
        {
            using (_PRF_EnumerateDirectories.Auto())
            {
                return Directory.EnumerateDirectories(path, searchPattern, searchOption);
            }
        }

        /// <summary>Returns an enumerable collection of file names in a specified path.</summary>
        /// <param name="path">The relative or absolute path to the directory to search. This string is not case-sensitive.</param>
        /// <returns>An enumerable collection of the full names (including paths) for the files in the directory specified by <paramref name="path" />.</returns>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path " />is a zero-length string, contains only white space, or contains invalid characters. You can query for invalid characters
        ///     by using the <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">
        ///     <paramref name="path" /> is invalid, such as referring to an unmapped drive.
        /// </exception>
        /// <exception cref="T:System.IO.IOException">
        ///     <paramref name="path" /> is a file name.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or combined exceed the system-defined maximum length. For example,
        ///     on Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission.</exception>
        public static IEnumerable<string> EnumerateFiles(string path)
        {
            using (_PRF_EnumerateFiles.Auto())
            {
                return Directory.EnumerateFiles(path);
            }
        }

        /// <summary>Returns an enumerable collection of file names that match a search pattern in a specified path.</summary>
        /// <param name="path">The relative or absolute path to the directory to search. This string is not case-sensitive.</param>
        /// <param name="searchPattern">
        ///     The search string to match against the names of files in <paramref name="path" />.  This parameter can contain a
        ///     combination of valid literal path and wildcard (* and ?) characters (see Remarks), but doesn't support regular expressions.
        /// </param>
        /// <returns>
        ///     An enumerable collection of the full names (including paths) for the files in the directory specified by <paramref name="path" /> and that
        ///     match the specified search pattern.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path " />is a zero-length string, contains only white space, or contains invalid characters. You can query for invalid characters
        ///     by using the <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.- or -
        ///     <paramref name="searchPattern" /> does not contain a valid pattern.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.-or-
        ///     <paramref name="searchPattern" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">
        ///     <paramref name="path" /> is invalid, such as referring to an unmapped drive.
        /// </exception>
        /// <exception cref="T:System.IO.IOException">
        ///     <paramref name="path" /> is a file name.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or combined exceed the system-defined maximum length. For example,
        ///     on Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission.</exception>
        public static IEnumerable<string> EnumerateFiles(string path, string searchPattern)
        {
            using (_PRF_EnumerateFiles.Auto())
            {
                return Directory.EnumerateFiles(path, searchPattern);
            }
        }

        /// <summary>Returns an enumerable collection of file names that match a search pattern in a specified path, and optionally searches subdirectories.</summary>
        /// <param name="path">The relative or absolute path to the directory to search. This string is not case-sensitive.</param>
        /// <param name="searchPattern">
        ///     The search string to match against the names of files in <paramref name="path" />.  This parameter can contain a
        ///     combination of valid literal path and wildcard (* and ?) characters (see Remarks), but doesn't support regular expressions.
        /// </param>
        /// <param name="searchOption">
        ///     One of the enumeration values that specifies whether the search operation should include only the current directory or
        ///     should include all subdirectories.The default value is <see cref="F:System.IO.SearchOption.TopDirectoryOnly" />.
        /// </param>
        /// <returns>
        ///     An enumerable collection of the full names (including paths) for the files in the directory specified by <paramref name="path" /> and that
        ///     match the specified search pattern and option.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path " />is a zero-length string, contains only white space, or contains invalid characters. You can query for invalid characters
        ///     by using the <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.- or -
        ///     <paramref name="searchPattern" /> does not contain a valid pattern.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.-or-
        ///     <paramref name="searchPattern" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="searchOption" /> is not a valid <see cref="T:System.IO.SearchOption" /> value.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">
        ///     <paramref name="path" /> is invalid, such as referring to an unmapped drive.
        /// </exception>
        /// <exception cref="T:System.IO.IOException">
        ///     <paramref name="path" /> is a file name.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or combined exceed the system-defined maximum length. For example,
        ///     on Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission.</exception>
        public static IEnumerable<string> EnumerateFiles(
            string path,
            string searchPattern,
            SearchOption searchOption)
        {
            using (_PRF_EnumerateFiles.Auto())
            {
                return Directory.EnumerateFiles(path, searchPattern, searchOption);
            }
        }

        /// <summary>Returns an enumerable collection of file names and directory names in a specified path. </summary>
        /// <param name="path">The relative or absolute path to the directory to search. This string is not case-sensitive.</param>
        /// <returns>An enumerable collection of file-system entries in the directory specified by <paramref name="path" />.</returns>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path " />is a zero-length string, contains only white space, or contains invalid characters. You can query for invalid characters
        ///     by using the <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">
        ///     <paramref name="path" /> is invalid, such as referring to an unmapped drive.
        /// </exception>
        /// <exception cref="T:System.IO.IOException">
        ///     <paramref name="path" /> is a file name.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or combined exceed the system-defined maximum length. For example,
        ///     on Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission.</exception>
        public static IEnumerable<string> EnumerateFileSystemEntries(string path)
        {
            using (_PRF_EnumerateFileSystemEntries.Auto())
            {
                return Directory.EnumerateFileSystemEntries(path);
            }
        }

        /// <summary>Returns an enumerable collection of file names and directory names that  match a search pattern in a specified path.</summary>
        /// <param name="path">The relative or absolute path to the directory to search. This string is not case-sensitive. </param>
        /// <param name="searchPattern">
        ///     The search string to match against the names of file-system entries in <paramref name="path" />.  This parameter can
        ///     contain a combination of valid literal path and wildcard (* and ?) characters (see Remarks), but doesn't support regular expressions.
        /// </param>
        /// <returns>
        ///     An enumerable collection of file-system entries in the directory specified by <paramref name="path" /> and that match the specified search
        ///     pattern.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path " />is a zero-length string, contains only white space, or contains invalid characters. You can query for invalid characters
        ///     by using the <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.- or -
        ///     <paramref name="searchPattern" /> does not contain a valid pattern.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.-or-
        ///     <paramref name="searchPattern" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">
        ///     <paramref name="path" /> is invalid, such as referring to an unmapped drive.
        /// </exception>
        /// <exception cref="T:System.IO.IOException">
        ///     <paramref name="path" /> is a file name.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or combined exceed the system-defined maximum length. For example,
        ///     on Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission.</exception>
        public static IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern)
        {
            using (_PRF_EnumerateFileSystemEntries.Auto())
            {
                return Directory.EnumerateFileSystemEntries(path, searchPattern);
            }
        }

        /// <summary>
        ///     Returns an enumerable collection of file names and directory names that match a search pattern in a specified path, and optionally searches
        ///     subdirectories.
        /// </summary>
        /// <param name="path">The relative or absolute path to the directory to search. This string is not case-sensitive.</param>
        /// <param name="searchPattern">
        ///     The search string to match against file-system entries in <paramref name="path" />.  This parameter can contain a
        ///     combination of valid literal path and wildcard (* and ?) characters (see Remarks), but doesn't support regular expressions.
        /// </param>
        /// <param name="searchOption">
        ///     One of the enumeration values  that specifies whether the search operation should include only the current directory or
        ///     should include all subdirectories.The default value is <see cref="F:System.IO.SearchOption.TopDirectoryOnly" />.
        /// </param>
        /// <returns>
        ///     An enumerable collection of file-system entries in the directory specified by <paramref name="path" /> and that match the specified search
        ///     pattern and option.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path " />is a zero-length string, contains only white space, or contains invalid characters. You can query for invalid characters
        ///     by using the <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.- or -
        ///     <paramref name="searchPattern" /> does not contain a valid pattern.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.-or-
        ///     <paramref name="searchPattern" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="searchOption" /> is not a valid <see cref="T:System.IO.SearchOption" /> value.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">
        ///     <paramref name="path" /> is invalid, such as referring to an unmapped drive.
        /// </exception>
        /// <exception cref="T:System.IO.IOException">
        ///     <paramref name="path" /> is a file name.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or combined exceed the system-defined maximum length. For example,
        ///     on Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission.</exception>
        public static IEnumerable<string> EnumerateFileSystemEntries(
            string path,
            string searchPattern,
            SearchOption searchOption)
        {
            using (_PRF_EnumerateFileSystemEntries.Auto())
            {
                return Directory.EnumerateFileSystemEntries(path, searchPattern, searchOption);
            }
        }

        /// <summary>Determines whether the given path refers to an existing directory on disk.</summary>
        /// <param name="path">The path to test. </param>
        /// <returns>
        ///     <see langword="true" /> if <paramref name="path" /> refers to an existing directory; <see langword="false" /> if the directory does not exist or
        ///     an error occurs when trying to determine if the specified directory exists.
        /// </returns>
        public static bool Exists(string path)
        {
            using (_PRF_Exists.Auto())
            {
                return Directory.Exists(path);
            }
        }

        /// <summary>Gets the creation date and time of a directory.</summary>
        /// <param name="path">The path of the directory. </param>
        /// <returns>A structure that is set to the creation date and time for the specified directory. This value is expressed in local time.</returns>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters. You can query for
        ///     invalid characters by using the <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters.
        /// </exception>
        public static DateTime GetCreationTime(string path)
        {
            using (_PRF_GetCreationTime.Auto())
            {
                return Directory.GetCreationTime(path);
            }
        }

        /// <summary>Gets the creation date and time, in Coordinated Universal Time (UTC) format, of a directory.</summary>
        /// <param name="path">The path of the directory. </param>
        /// <returns>A structure that is set to the creation date and time for the specified directory. This value is expressed in UTC time.</returns>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters. You can query for
        ///     invalid characters by using the <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters.
        /// </exception>
        public static DateTime GetCreationTimeUtc(string path)
        {
            using (_PRF_GetCreationTimeUtc.Auto())
            {
                return Directory.GetCreationTimeUtc(path);
            }
        }

        /// <summary>Gets the current working directory of the application.</summary>
        /// <returns>A string that contains the path of the current working directory, and does not end with a backslash (\).</returns>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     The operating system is Windows CE, which does not have current directory functionality.This method
        ///     is available in the .NET Compact Framework, but is not currently supported.
        /// </exception>
        public static string GetCurrentDirectory()
        {
            using (_PRF_GetCurrentDirectory.Auto())
            {
                return Directory.GetCurrentDirectory().CleanFullPath();
            }
        }

        /// <summary>Returns the names of subdirectories (including their paths) in the specified directory.</summary>
        /// <param name="path">The relative or absolute path to the directory to search. This string is not case-sensitive.</param>
        /// <returns>An array of the full names (including paths) of subdirectories in the specified path, or an empty array if no directories are found.</returns>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters. You can query for
        ///     invalid characters by using the <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.IO.IOException">
        ///     <paramref name="path" /> is a file name.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive). </exception>
        public static string[] GetDirectories(string path)
        {
            using (_PRF_GetDirectories.Auto())
            {
                return Directory.GetDirectories(path).Select(p => p.CleanFullPath()).ToArray();
            }
        }

        /// <summary>Returns the names of subdirectories (including their paths) that match the specified search pattern in the specified directory.</summary>
        /// <param name="path">The relative or absolute path to the directory to search. This string is not case-sensitive.</param>
        /// <param name="searchPattern">
        ///     The search string to match against the names of subdirectories in <paramref name="path" />. This parameter can contain a
        ///     combination of valid literal and wildcard characters (see Remarks), but doesn't support regular expressions.
        /// </param>
        /// <returns>
        ///     An array of the full names (including paths) of the subdirectories that match the search pattern in the specified directory, or an empty
        ///     array if no directories are found.
        /// </returns>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters. You can query for
        ///     invalid characters by using <see cref="M:System.IO.Path.GetInvalidPathChars" />.-or-
        ///     <paramref name="searchPattern" /> doesn't contain a valid pattern.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> or <paramref name="searchPattern" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.IO.IOException">
        ///     <paramref name="path" /> is a file name.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive). </exception>
        public static string[] GetDirectories(string path, string searchPattern)
        {
            using (_PRF_GetDirectories.Auto())
            {
                return Directory.GetDirectories(path, searchPattern).Select(p => p.CleanFullPath()).ToArray();
            }
        }

        /// <summary>
        ///     Returns the names of the subdirectories (including their paths) that match the specified search pattern in the specified directory, and
        ///     optionally searches subdirectories.
        /// </summary>
        /// <param name="path">The relative or absolute path to the directory to search. This string is not case-sensitive.</param>
        /// <param name="searchPattern">
        ///     The search string to match against the names of subdirectories in <paramref name="path" />. This parameter can contain a
        ///     combination of valid literal and wildcard characters (see Remarks), but doesn't support regular expressions.
        /// </param>
        /// <param name="searchOption">
        ///     One of the enumeration values that specifies whether the search operation should include all subdirectories or only the
        ///     current directory.
        /// </param>
        /// <returns>
        ///     An array of the full names (including paths) of the subdirectories that match the specified criteria, or an empty array if no directories
        ///     are found.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters. You can query for
        ///     invalid characters by using the <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.-or-
        ///     <paramref name="searchPattern" /> does not contain a valid pattern.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> or <paramref name="searchPattern" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="searchOption" /> is not a valid <see cref="T:System.IO.SearchOption" /> value.
        /// </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.IO.IOException">
        ///     <paramref name="path" /> is a file name.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive). </exception>
        public static string[] GetDirectories(
            string path,
            string searchPattern,
            SearchOption searchOption,
            bool throwIfMissing = true)
        {
            using (_PRF_GetDirectories.Auto())
            {
                try
                {
                    return Directory.GetDirectories(path, searchPattern, searchOption)
                                    .Select(p => p.CleanFullPath())
                                    .ToArray();
                }
                catch (Exception ex)
                {
                    Context.Log.Error(
                        ZString.Format(
                            "Failed to find directories at {0} with search pattern {1}.",
                            path,
                            searchPattern
                        ),
                        ex
                    );

                    if (!throwIfMissing)
                    {
                        return null;
                    }

                    throw;
                }
            }
        }

        /// <summary>Returns the volume information, root information, or both for the specified path.</summary>
        /// <param name="path">The path of a file or directory. </param>
        /// <returns>A string that contains the volume information, root information, or both for the specified path.</returns>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters. You can query for
        ///     invalid characters with <see cref="M:System.IO.Path.GetInvalidPathChars" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters.
        /// </exception>
        public static string GetDirectoryRoot(string path)
        {
            using (_PRF_GetDirectoryRoot.Auto())
            {
                return Directory.GetDirectoryRoot(path);
            }
        }

        /// <summary>Returns the names of files (including their paths) in the specified directory.</summary>
        /// <param name="path">The relative or absolute path to the directory to search. This string is not case-sensitive.</param>
        /// <returns>An array of the full names (including paths) for the files in the specified directory, or an empty array if no files are found.</returns>
        /// <exception cref="T:System.IO.IOException">
        ///     <paramref name="path" /> is a file name.-or-A network error has occurred.
        /// </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters. You can query for
        ///     invalid characters by using the <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is not found or is invalid (for example, it is on an unmapped drive). </exception>
        public static AssetPath[] GetFiles(string path)
        {
            using (_PRF_GetFiles.Auto())
            {
                return Directory.GetFiles(path).Select(AssetPath.FromAbsolutePath).ToArray();
            }
        }

        /// <summary>Returns the names of files (including their paths) that match the specified search pattern in the specified directory.</summary>
        /// <param name="path">The relative or absolute path to the directory to search. This string is not case-sensitive.</param>
        /// <param name="searchPattern">
        ///     The search string to match against the names of files in <paramref name="path" />.  This parameter can contain a
        ///     combination of valid literal path and wildcard (* and ?) characters (see Remarks), but doesn't support regular expressions.
        /// </param>
        /// <returns>
        ///     An array of the full names (including paths) for the files in the specified directory that match the specified search pattern, or an empty
        ///     array if no files are found.
        /// </returns>
        /// <exception cref="T:System.IO.IOException">
        ///     <paramref name="path" /> is a file name.-or-A network error has occurred.
        /// </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters. You can query for
        ///     invalid characters by using <see cref="M:System.IO.Path.GetInvalidPathChars" />.-or-
        ///     <paramref name="searchPattern" /> doesn't contain a valid pattern.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> or <paramref name="searchPattern" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is not found or is invalid (for example, it is on an unmapped drive). </exception>
        public static AssetPath[] GetFiles(string path, string searchPattern)
        {
            using (_PRF_GetFiles.Auto())
            {
                return Directory.GetFiles(path, searchPattern).Select(AssetPath.FromAbsolutePath).ToArray();
            }
        }

        /// <summary>
        ///     Returns the names of files (including their paths) that match the specified search pattern in the specified directory, using a value to
        ///     determine whether to search subdirectories.
        /// </summary>
        /// <param name="path">The relative or absolute path to the directory to search. This string is not case-sensitive.</param>
        /// <param name="searchPattern">
        ///     The search string to match against the names of files in <paramref name="path" />.  This parameter can contain a
        ///     combination of valid literal path and wildcard (* and ?) characters (see Remarks), but doesn't support regular expressions.
        /// </param>
        /// <param name="searchOption">
        ///     One of the enumeration values that specifies whether the search operation should include all subdirectories or only the
        ///     current directory.
        /// </param>
        /// <returns>
        ///     An array of the full names (including paths) for the files in the specified directory that match the specified search pattern and option, or
        ///     an empty array if no files are found.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters. You can query for
        ///     invalid characters with the <see cref="M:System.IO.Path.GetInvalidPathChars" /> method. -or-
        ///     <paramref name="searchPattern" /> does not contain a valid pattern.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> or <paramref name="searchPattern" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="searchOption" /> is not a valid <see cref="T:System.IO.SearchOption" /> value.
        /// </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is not found or is invalid (for example, it is on an unmapped drive). </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.IO.IOException">
        ///     <paramref name="path" /> is a file name.-or-A network error has occurred.
        /// </exception>
        public static AssetPath[] GetFiles(string path, string searchPattern, SearchOption searchOption)
        {
            using (_PRF_GetFiles.Auto())
            {
                if (!Directory.Exists(path))
                {
                    return Array.Empty<AssetPath>();
                }

                var files = Directory.GetFiles(path, searchPattern, searchOption)
                                     .Select(AssetPath.FromAbsolutePath)
                                     .ToArray();
                return files;
            }
        }

        /// <summary>Returns the names of all files and subdirectories in a specified path.</summary>
        /// <param name="path">The relative or absolute path to the directory to search. This string is not case-sensitive.</param>
        /// <returns>An array of the names of files and subdirectories in the specified directory, or an empty array if no files or subdirectories are found.</returns>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters. You can query for
        ///     invalid characters with <see cref="M:System.IO.Path.GetInvalidPathChars" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.IO.IOException">
        ///     <paramref name="path" /> is a file name.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive). </exception>
        public static string[] GetFileSystemEntries(string path)
        {
            using (_PRF_GetFileSystemEntries.Auto())
            {
                return Directory.GetFileSystemEntries(path).Select(p => p.CleanFullPath()).ToArray();
            }
        }

        /// <summary>Returns an array of file names and directory names that that match a search pattern in a specified path.</summary>
        /// <param name="path">The relative or absolute path to the directory to search. This string is not case-sensitive.</param>
        /// <param name="searchPattern">
        ///     The search string to match against the names of file and directories in <paramref name="path" />.  This parameter can
        ///     contain a combination of valid literal path and wildcard (* and ?) characters (see Remarks), but doesn't support regular expressions.
        /// </param>
        /// <returns>An array of file names and directory names that match the specified search criteria, or an empty array if no files or directories are found.</returns>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters. You can query for
        ///     invalid characters with the <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.-or-
        ///     <paramref name="searchPattern" /> does not contain a valid pattern.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> or <paramref name="searchPattern" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.IO.IOException">
        ///     <paramref name="path" /> is a file name.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive). </exception>
        public static string[] GetFileSystemEntries(string path, string searchPattern)
        {
            using (_PRF_GetFileSystemEntries.Auto())
            {
                return Directory.GetFileSystemEntries(path, searchPattern)
                                .Select(p => p.CleanFullPath())
                                .ToArray();
            }
        }

        /// <summary>
        ///     Returns an array of all the file names and directory names that match a search pattern in a specified path, and optionally searches
        ///     subdirectories.
        /// </summary>
        /// <param name="path">The relative or absolute path to the directory to search. This string is not case-sensitive.</param>
        /// <param name="searchPattern">
        ///     The search string to match against the names of files and directories in <paramref name="path" />.  This parameter can
        ///     contain a combination of valid literal path and wildcard (* and ?) characters (see Remarks), but doesn't support regular expressions.
        /// </param>
        /// <param name="searchOption">
        ///     One of the enumeration values that specifies whether the search operation should include only the current directory or
        ///     should include all subdirectories.The default value is <see cref="F:System.IO.SearchOption.TopDirectoryOnly" />.
        /// </param>
        /// <returns>
        ///     An array of file the file names and directory names that match the specified search criteria, or an empty array if no files or directories
        ///     are found.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path " />is a zero-length string, contains only white space, or contains invalid characters. You can query for invalid characters
        ///     by using the <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.- or -
        ///     <paramref name="searchPattern" /> does not contain a valid pattern.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.-or-
        ///     <paramref name="searchPattern" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="searchOption" /> is not a valid <see cref="T:System.IO.SearchOption" /> value.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">
        ///     <paramref name="path" /> is invalid, such as referring to an unmapped drive.
        /// </exception>
        /// <exception cref="T:System.IO.IOException">
        ///     <paramref name="path" /> is a file name.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or combined exceed the system-defined maximum length. For example,
        ///     on Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission.</exception>
        public static string[] GetFileSystemEntries(
            string path,
            string searchPattern,
            SearchOption searchOption)
        {
            using (_PRF_GetFileSystemEntries.Auto())
            {
                return Directory.GetFileSystemEntries(path, searchPattern, searchOption)
                                .Select(p => p.CleanFullPath())
                                .ToArray();
            }
        }

        /// <summary>Returns the date and time the specified file or directory was last accessed.</summary>
        /// <param name="path">The file or directory for which to obtain access date and time information. </param>
        /// <returns>A structure that is set to the date and time the specified file or directory was last accessed. This value is expressed in local time.</returns>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters. You can query for
        ///     invalid characters with the <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">The <paramref name="path" /> parameter is in an invalid format. </exception>
        public static DateTime GetLastAccessTime(string path)
        {
            using (_PRF_GetLastAccessTime.Auto())
            {
                return Directory.GetLastAccessTime(path);
            }
        }

        /// <summary>Returns the date and time, in Coordinated Universal Time (UTC) format, that the specified file or directory was last accessed.</summary>
        /// <param name="path">The file or directory for which to obtain access date and time information. </param>
        /// <returns>A structure that is set to the date and time the specified file or directory was last accessed. This value is expressed in UTC time.</returns>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters. You can query for
        ///     invalid characters with the  <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">The <paramref name="path" /> parameter is in an invalid format. </exception>
        public static DateTime GetLastAccessTimeUtc(string path)
        {
            using (_PRF_GetLastAccessTimeUtc.Auto())
            {
                return Directory.GetLastAccessTimeUtc(path);
            }
        }

        /// <summary>Returns the date and time the specified file or directory was last written to.</summary>
        /// <param name="path">The file or directory for which to obtain modification date and time information. </param>
        /// <returns>A structure that is set to the date and time the specified file or directory was last written to. This value is expressed in local time.</returns>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters. You can query for
        ///     invalid characters with the  <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters.
        /// </exception>
        public static DateTime GetLastWriteTime(string path)
        {
            using (_PRF_GetLastWriteTime.Auto())
            {
                return Directory.GetLastWriteTime(path);
            }
        }

        /// <summary>Returns the date and time, in Coordinated Universal Time (UTC) format, that the specified file or directory was last written to.</summary>
        /// <param name="path">The file or directory for which to obtain modification date and time information. </param>
        /// <returns>A structure that is set to the date and time the specified file or directory was last written to. This value is expressed in UTC time.</returns>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters. You can query for
        ///     invalid characters with the  <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters.
        /// </exception>
        public static DateTime GetLastWriteTimeUtc(string path)
        {
            using (_PRF_GetLastWriteTimeUtc.Auto())
            {
                return Directory.GetLastWriteTimeUtc(path);
            }
        }

        /// <summary>Retrieves the names of the logical drives on this computer in the form "&lt;drive letter&gt;:\".</summary>
        /// <returns>The logical drives on this computer.</returns>
        /// <exception cref="T:System.IO.IOException">An I/O error occured (for example, a disk error). </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. </exception>
        public static string[] GetLogicalDrives()
        {
            using (_PRF_GetLogicalDrives.Auto())
            {
                return Directory.GetLogicalDrives();
            }
        }

        /// <summary>Retrieves the parent directory of the specified path, including both absolute and relative paths.</summary>
        /// <param name="path">The path for which to retrieve the parent directory. </param>
        /// <returns>
        ///     The parent directory, or <see langword="null" /> if <paramref name="path" /> is the root directory, including the root of a UNC server or
        ///     share name.
        /// </returns>
        /// <exception cref="T:System.IO.IOException">The directory specified by <paramref name="path" /> is read-only. </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters. You can query for
        ///     invalid characters with the  <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path was not found. </exception>
        public static AppaDirectoryInfo GetParent(string path)
        {
            using (_PRF_GetParent.Auto())
            {
                return Directory.GetParent(path);
            }
        }

        /// <summary>Moves a file or a directory and its contents to a new location.</summary>
        /// <param name="sourceDirName">The path of the file or directory to move. </param>
        /// <param name="destDirName">
        ///     The path to the new location for <paramref name="sourceDirName" />. If <paramref name="sourceDirName" /> is a file, then
        ///     <paramref name="destDirName" /> must also be a file name.
        /// </param>
        /// <exception cref="T:System.IO.IOException">
        ///     An attempt was made to move a directory to a different volume. -or-
        ///     <paramref name="destDirName" /> already exists. -or- The <paramref name="sourceDirName" /> and <paramref name="destDirName" /> parameters refer
        ///     to the same file or directory. -or-The directory or a file within it is being used by another process.
        /// </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="sourceDirName" /> or <paramref name="destDirName" /> is a zero-length string, contains only white space, or contains one or more
        ///     invalid characters. You can query for invalid characters with the  <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="sourceDirName" /> or <paramref name="destDirName" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">
        ///     The path specified by <paramref name="sourceDirName" /> is invalid (for example, it is on an
        ///     unmapped drive).
        /// </exception>
        public static void Move(string sourceDirName, string destDirName)
        {
            using (_PRF_Move.Auto())
            {
                try
                {
                    Directory.Move(sourceDirName, destDirName);
                }
                catch (IOException)
                {
                    Context.Log.Error(ZString.Format("[{0}] is locked!", sourceDirName));

                    throw;
                }
            }
        }

        /// <summary>Sets the creation date and time for the specified file or directory.</summary>
        /// <param name="path">The file or directory for which to set the creation date and time information. </param>
        /// <param name="creationTime">The date and time the file or directory was last written to. This value is expressed in local time.</param>
        /// <exception cref="T:System.IO.FileNotFoundException">The specified path was not found. </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters. You can query for
        ///     invalid characters with the  <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="creationTime" /> specifies a value outside the range of dates or times permitted for this operation.
        /// </exception>
        /// <exception cref="T:System.PlatformNotSupportedException">The current operating system is not Windows NT or later.</exception>
        public static void SetCreationTime(string path, DateTime creationTime)
        {
            using (_PRF_SetCreationTime.Auto())
            {
                Directory.SetCreationTime(path, creationTime);
            }
        }

        /// <summary>Sets the creation date and time, in Coordinated Universal Time (UTC) format, for the specified file or directory.</summary>
        /// <param name="path">The file or directory for which to set the creation date and time information. </param>
        /// <param name="creationTimeUtc">The date and time the directory or file was created. This value is expressed in local time.</param>
        /// <exception cref="T:System.IO.FileNotFoundException">The specified path was not found. </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters. You can query for
        ///     invalid characters with the  <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="creationTimeUtc" /> specifies a value outside the range of dates or times permitted for this operation.
        /// </exception>
        /// <exception cref="T:System.PlatformNotSupportedException">The current operating system is not Windows NT or later.</exception>
        public static void SetCreationTimeUtc(string path, DateTime creationTimeUtc)
        {
            using (_PRF_SetCreationTimeUtc.Auto())
            {
                Directory.SetCreationTimeUtc(path, creationTimeUtc);
            }
        }

        /// <summary>Sets the application's current working directory to the specified directory.</summary>
        /// <param name="path">The path to which the current working directory is set. </param>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred. </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters. You can query for
        ///     invalid characters with the  <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission to access unmanaged code. </exception>
        /// <exception cref="T:System.IO.FileNotFoundException">The specified path was not found. </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified directory was not found.</exception>
        public static void SetCurrentDirectory(string path)
        {
            using (_PRF_SetCurrentDirectory.Auto())
            {
                Directory.SetCurrentDirectory(path);
            }
        }

        /// <summary>Sets the date and time the specified file or directory was last accessed.</summary>
        /// <param name="path">The file or directory for which to set the access date and time information. </param>
        /// <param name="lastAccessTime">
        ///     An object that contains the value to set for the access date and time of <paramref name="path" />. This value is
        ///     expressed in local time.
        /// </param>
        /// <exception cref="T:System.IO.FileNotFoundException">The specified path was not found. </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters. You can query for
        ///     invalid characters with the  <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.PlatformNotSupportedException">The current operating system is not Windows NT or later.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="lastAccessTime" /> specifies a value outside the range of dates or times permitted for this operation.
        /// </exception>
        public static void SetLastAccessTime(string path, DateTime lastAccessTime)
        {
            using (_PRF_SetLastAccessTime.Auto())
            {
                Directory.SetLastAccessTime(path, lastAccessTime);
            }
        }

        /// <summary>Sets the date and time, in Coordinated Universal Time (UTC) format, that the specified file or directory was last accessed.</summary>
        /// <param name="path">The file or directory for which to set the access date and time information. </param>
        /// <param name="lastAccessTimeUtc">
        ///     An object that  contains the value to set for the access date and time of <paramref name="path" />. This value is
        ///     expressed in UTC time.
        /// </param>
        /// <exception cref="T:System.IO.FileNotFoundException">The specified path was not found. </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters. You can query for
        ///     invalid characters with the  <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.PlatformNotSupportedException">The current operating system is not Windows NT or later.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="lastAccessTimeUtc" /> specifies a value outside the range of dates or times permitted for this operation.
        /// </exception>
        public static void SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc)
        {
            using (_PRF_SetLastAccessTimeUtc.Auto())
            {
                Directory.SetLastAccessTimeUtc(path, lastAccessTimeUtc);
            }
        }

        /// <summary>Sets the date and time a directory was last written to.</summary>
        /// <param name="path">The path of the directory. </param>
        /// <param name="lastWriteTime">The date and time the directory was last written to. This value is expressed in local time.  </param>
        /// <exception cref="T:System.IO.FileNotFoundException">The specified path was not found. </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters. You can query for
        ///     invalid characters with the  <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.PlatformNotSupportedException">The current operating system is not Windows NT or later.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="lastWriteTime" /> specifies a value outside the range of dates or times permitted for this operation.
        /// </exception>
        public static void SetLastWriteTime(string path, DateTime lastWriteTime)
        {
            using (_PRF_SetLastWriteTime.Auto())
            {
                Directory.SetLastWriteTime(path, lastWriteTime);
            }
        }

        /// <summary>Sets the date and time, in Coordinated Universal Time (UTC) format, that a directory was last written to.</summary>
        /// <param name="path">The path of the directory. </param>
        /// <param name="lastWriteTimeUtc">The date and time the directory was last written to. This value is expressed in UTC time. </param>
        /// <exception cref="T:System.IO.FileNotFoundException">The specified path was not found. </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters. You can query for
        ///     invalid characters with the  <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.PlatformNotSupportedException">The current operating system is not Windows NT or later.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="lastWriteTimeUtc" /> specifies a value outside the range of dates or times permitted for this operation.
        /// </exception>
        public static void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc)
        {
            using (_PRF_SetLastWriteTimeUtc.Auto())
            {
                Directory.SetLastWriteTimeUtc(path, lastWriteTimeUtc);
            }
        }

        #region Profiling

        private const string _PRF_PFX = nameof(AppaDirectory) + ".";
        private static readonly ProfilerMarker _PRF_GetParent = new(_PRF_PFX + nameof(GetParent));
        private static readonly ProfilerMarker _PRF_CreateDirectory = new(_PRF_PFX + nameof(CreateDirectory));

        private static readonly ProfilerMarker _PRF_Exists = new(_PRF_PFX + nameof(Exists));
        private static readonly ProfilerMarker _PRF_SetCreationTime = new(_PRF_PFX + nameof(SetCreationTime));

        private static readonly ProfilerMarker _PRF_SetCreationTimeUtc =
            new(_PRF_PFX + nameof(SetCreationTimeUtc));

        private static readonly ProfilerMarker _PRF_GetCreationTime = new(_PRF_PFX + nameof(GetCreationTime));

        private static readonly ProfilerMarker _PRF_GetCreationTimeUtc =
            new(_PRF_PFX + nameof(GetCreationTimeUtc));

        private static readonly ProfilerMarker _PRF_SetLastWriteTime =
            new(_PRF_PFX + nameof(SetLastWriteTime));

        private static readonly ProfilerMarker _PRF_SetLastWriteTimeUtc =
            new(_PRF_PFX + nameof(SetLastWriteTimeUtc));

        private static readonly ProfilerMarker _PRF_GetLastWriteTime =
            new(_PRF_PFX + nameof(GetLastWriteTime));

        private static readonly ProfilerMarker _PRF_GetLastWriteTimeUtc =
            new(_PRF_PFX + nameof(GetLastWriteTimeUtc));

        private static readonly ProfilerMarker _PRF_SetLastAccessTime =
            new(_PRF_PFX + nameof(SetLastAccessTime));

        private static readonly ProfilerMarker _PRF_SetLastAccessTimeUtc =
            new(_PRF_PFX + nameof(SetLastAccessTimeUtc));

        private static readonly ProfilerMarker _PRF_GetLastAccessTime =
            new(_PRF_PFX + nameof(GetLastAccessTime));

        private static readonly ProfilerMarker _PRF_GetLastAccessTimeUtc =
            new(_PRF_PFX + nameof(GetLastAccessTimeUtc));

        private static readonly ProfilerMarker _PRF_GetFiles = new(_PRF_PFX + nameof(GetFiles));
        private static readonly ProfilerMarker _PRF_GetDirectories = new(_PRF_PFX + nameof(GetDirectories));

        private static readonly ProfilerMarker _PRF_GetFileSystemEntries =
            new(_PRF_PFX + nameof(GetFileSystemEntries));

        private static readonly ProfilerMarker _PRF_EnumerateDirectories =
            new(_PRF_PFX + nameof(EnumerateDirectories));

        private static readonly ProfilerMarker _PRF_EnumerateFiles = new(_PRF_PFX + nameof(EnumerateFiles));

        private static readonly ProfilerMarker _PRF_EnumerateFileSystemEntries =
            new(_PRF_PFX + nameof(EnumerateFileSystemEntries));

        private static readonly ProfilerMarker _PRF_GetLogicalDrives =
            new(_PRF_PFX + nameof(GetLogicalDrives));

        private static readonly ProfilerMarker _PRF_GetDirectoryRoot =
            new(_PRF_PFX + nameof(GetDirectoryRoot));

        private static readonly ProfilerMarker _PRF_GetCurrentDirectory =
            new(_PRF_PFX + nameof(GetCurrentDirectory));

        private static readonly ProfilerMarker _PRF_SetCurrentDirectory =
            new(_PRF_PFX + nameof(SetCurrentDirectory));

        private static readonly ProfilerMarker _PRF_Move = new(_PRF_PFX + nameof(Move));
        private static readonly ProfilerMarker _PRF_Delete = new(_PRF_PFX + nameof(Delete));

        private static readonly ProfilerMarker _PRF_CreateDirectoryStructureForFilePath =
            new ProfilerMarker(_PRF_PFX + nameof(CreateDirectoryStructureForFilePath));

        #endregion
    }
}
