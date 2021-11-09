using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Unity.Profiling;

namespace Appalachia.CI.Integration.FileSystem
{
    public static class AppaFile
    {
        #region Profiling And Tracing Markers

        private const string _PRF_PFX = nameof(AppaFile) + ".";
        private static readonly ProfilerMarker _PRF_OpenText = new(_PRF_PFX + nameof(OpenText));
        private static readonly ProfilerMarker _PRF_CreateText = new(_PRF_PFX + nameof(CreateText));
        private static readonly ProfilerMarker _PRF_AppendText = new(_PRF_PFX + nameof(AppendText));
        private static readonly ProfilerMarker _PRF_Copy = new(_PRF_PFX + nameof(Copy));
        private static readonly ProfilerMarker _PRF_Create = new(_PRF_PFX + nameof(Create));
        private static readonly ProfilerMarker _PRF_Delete = new(_PRF_PFX + nameof(Delete));
        private static readonly ProfilerMarker _PRF_Exists = new(_PRF_PFX + nameof(Exists));
        private static readonly ProfilerMarker _PRF_Open = new(_PRF_PFX + nameof(Open));
        private static readonly ProfilerMarker _PRF_SetCreationTime = new(_PRF_PFX + nameof(SetCreationTime));

        private static readonly ProfilerMarker _PRF_SetCreationTimeUtc =
            new(_PRF_PFX + nameof(SetCreationTimeUtc));

        private static readonly ProfilerMarker _PRF_GetCreationTime = new(_PRF_PFX + nameof(GetCreationTime));

        private static readonly ProfilerMarker _PRF_GetCreationTimeUtc =
            new(_PRF_PFX + nameof(GetCreationTimeUtc));

        private static readonly ProfilerMarker _PRF_SetLastAccessTime =
            new(_PRF_PFX + nameof(SetLastAccessTime));

        private static readonly ProfilerMarker _PRF_SetLastAccessTimeUtc =
            new(_PRF_PFX + nameof(SetLastAccessTimeUtc));

        private static readonly ProfilerMarker _PRF_GetLastAccessTime =
            new(_PRF_PFX + nameof(GetLastAccessTime));

        private static readonly ProfilerMarker _PRF_GetLastAccessTimeUtc =
            new(_PRF_PFX + nameof(GetLastAccessTimeUtc));

        private static readonly ProfilerMarker _PRF_SetLastWriteTime =
            new(_PRF_PFX + nameof(SetLastWriteTime));

        private static readonly ProfilerMarker _PRF_SetLastWriteTimeUtc =
            new(_PRF_PFX + nameof(SetLastWriteTimeUtc));

        private static readonly ProfilerMarker _PRF_GetLastWriteTime =
            new(_PRF_PFX + nameof(GetLastWriteTime));

        private static readonly ProfilerMarker _PRF_GetLastWriteTimeUtc =
            new(_PRF_PFX + nameof(GetLastWriteTimeUtc));

        private static readonly ProfilerMarker _PRF_OpenRead = new(_PRF_PFX + nameof(OpenRead));
        private static readonly ProfilerMarker _PRF_OpenWrite = new(_PRF_PFX + nameof(OpenWrite));
        private static readonly ProfilerMarker _PRF_ReadAllText = new(_PRF_PFX + nameof(ReadAllText));
        private static readonly ProfilerMarker _PRF_WriteAllText = new(_PRF_PFX + nameof(WriteAllText));
        private static readonly ProfilerMarker _PRF_ReadAllBytes = new(_PRF_PFX + nameof(ReadAllBytes));
        private static readonly ProfilerMarker _PRF_WriteAllBytes = new(_PRF_PFX + nameof(WriteAllBytes));
        private static readonly ProfilerMarker _PRF_ReadAllLines = new(_PRF_PFX + nameof(ReadAllLines));
        private static readonly ProfilerMarker _PRF_ReadLines = new(_PRF_PFX + nameof(ReadLines));
        private static readonly ProfilerMarker _PRF_WriteAllLines = new(_PRF_PFX + nameof(WriteAllLines));
        private static readonly ProfilerMarker _PRF_AppendAllText = new(_PRF_PFX + nameof(AppendAllText));
        private static readonly ProfilerMarker _PRF_AppendAllLines = new(_PRF_PFX + nameof(AppendAllLines));

        private static readonly ProfilerMarker _PRF_Move = new(_PRF_PFX + nameof(Move));

        private static readonly ProfilerMarker _PRF_Replace = new(_PRF_PFX + nameof(Replace));

        #endregion

        /// <summary>
        ///     Appends lines to a file, and then closes the file. If the specified file does not exist, this method creates a file, writes the specified
        ///     lines to the file, and then closes the file.
        /// </summary>
        /// <param name="path">The file to append the lines to. The file is created if it doesn't already exist.</param>
        /// <param name="contents">The lines to append to the file.</param>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one more invalid characters defined by the
        ///     <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">Either<paramref name=" path " />or <paramref name="contents" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">
        ///     <paramref name="path" /> is invalid (for example, the directory doesn’t exist or it is on an unmapped drive).
        /// </exception>
        /// <exception cref="T:System.IO.FileNotFoundException">The file specified by <paramref name="path" /> was not found.</exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred while opening the file.</exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     <paramref name="path" /> exceeds the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248
        ///     characters and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     <paramref name="path" /> is in an invalid format.
        /// </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have permission to write to the file.</exception>
        /// <exception cref="T:System.UnauthorizedAccessException">
        ///     <paramref name="path" /> specifies a file that is read-only.-or-This operation is not supported on the current platform.-or-
        ///     <paramref name="path" /> is a directory.
        /// </exception>
        public static void AppendAllLines(string path, IEnumerable<string> contents)
        {
            using (_PRF_AppendAllLines.Auto())
            {
                File.AppendAllLines(path, contents);
            }
        }

        /// <summary>
        ///     Appends lines to a file by using a specified encoding, and then closes the file. If the specified file does not exist, this method creates a
        ///     file, writes the specified lines to the file, and then closes the file.
        /// </summary>
        /// <param name="path">The file to append the lines to. The file is created if it doesn't already exist.</param>
        /// <param name="contents">The lines to append to the file.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one more invalid characters defined by the
        ///     <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     Either<paramref name=" path" />, <paramref name="contents" />, or <paramref name="encoding" /> is
        ///     <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">
        ///     <paramref name="path" /> is invalid (for example, the directory doesn’t exist or it is on an unmapped drive).
        /// </exception>
        /// <exception cref="T:System.IO.FileNotFoundException">The file specified by <paramref name="path" /> was not found.</exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred while opening the file.</exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     <paramref name="path" /> exceeds the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248
        ///     characters and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     <paramref name="path" /> is in an invalid format.
        /// </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission.</exception>
        /// <exception cref="T:System.UnauthorizedAccessException">
        ///     <paramref name="path" /> specifies a file that is read-only.-or-This operation is not supported on the current platform.-or-
        ///     <paramref name="path" /> is a directory.-or-The caller does not have the required permission.
        /// </exception>
        public static void AppendAllLines(string path, IEnumerable<string> contents, Encoding encoding)
        {
            using (_PRF_AppendAllLines.Auto())
            {
                File.AppendAllLines(path, contents, encoding);
            }
        }

        /// <summary>
        ///     Opens a file, appends the specified string to the file, and then closes the file. If the file does not exist, this method creates a file,
        ///     writes the specified string to the file, then closes the file.
        /// </summary>
        /// <param name="path">The file to append the specified string to. </param>
        /// <param name="contents">The string to append to the file. </param>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by
        ///     <see cref="F:System.IO.Path.InvalidPathChars" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">
        ///     The specified path is invalid (for example, the directory doesn’t exist or it is on an
        ///     unmapped drive).
        /// </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred while opening the file. </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">
        ///     <paramref name="path" /> specified a file that is read-only.-or- This operation is not supported on the current platform.-or-
        ///     <paramref name="path" /> specified a directory.-or- The caller does not have the required permission.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     <paramref name="path" /> is in an invalid format.
        /// </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public static void AppendAllText(string path, string contents)
        {
            using (_PRF_AppendAllText.Auto())
            {
                File.AppendAllText(path, contents);
            }
        }

        /// <summary>Appends the specified string to the file, creating the file if it does not already exist.</summary>
        /// <param name="path">The file to append the specified string to. </param>
        /// <param name="contents">The string to append to the file. </param>
        /// <param name="encoding">The character encoding to use. </param>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by
        ///     <see cref="F:System.IO.Path.InvalidPathChars" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">
        ///     The specified path is invalid (for example, the directory doesn’t exist or it is on an
        ///     unmapped drive).
        /// </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred while opening the file. </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">
        ///     <paramref name="path" /> specified a file that is read-only.-or- This operation is not supported on the current platform.-or-
        ///     <paramref name="path" /> specified a directory.-or- The caller does not have the required permission.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     <paramref name="path" /> is in an invalid format.
        /// </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public static void AppendAllText(string path, string contents, Encoding encoding)
        {
            using (_PRF_AppendAllText.Auto())
            {
                File.AppendAllText(path, contents, encoding);
            }
        }

        /// <summary>
        ///     Creates a <see cref="T:System.IO.StreamWriter" /> that appends UTF-8 encoded text to an existing file, or to a new file if the specified
        ///     file does not exist.
        /// </summary>
        /// <param name="path">The path to the file to append to. </param>
        /// <returns>A stream writer that appends UTF-8 encoded text to the specified file or to a new file.</returns>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by
        ///     <see cref="F:System.IO.Path.InvalidPathChars" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">
        ///     The specified path is invalid (for example, the directory doesn’t exist or it is on an
        ///     unmapped drive).
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     <paramref name="path" /> is in an invalid format.
        /// </exception>
        public static StreamWriter AppendText(string path)
        {
            using (_PRF_AppendText.Auto())
            {
                return File.AppendText(path);
            }
        }

        /// <summary>Copies an existing file to a new file. Overwriting a file of the same name is not allowed.</summary>
        /// <param name="sourceFileName">The file to copy. </param>
        /// <param name="destFileName">The name of the destination file. This cannot be a directory or an existing file. </param>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="sourceFileName" /> or <paramref name="destFileName" /> is a zero-length string, contains only white space, or contains one or
        ///     more invalid characters as defined by <see cref="F:System.IO.Path.InvalidPathChars" />.-or-
        ///     <paramref name="sourceFileName" /> or <paramref name="destFileName" /> specifies a directory.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="sourceFileName" /> or <paramref name="destFileName" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">
        ///     The path specified in <paramref name="sourceFileName" /> or <paramref name="destFileName" />
        ///     is invalid (for example, it is on an unmapped drive).
        /// </exception>
        /// <exception cref="T:System.IO.FileNotFoundException">
        ///     <paramref name="sourceFileName" /> was not found.
        /// </exception>
        /// <exception cref="T:System.IO.IOException">
        ///     <paramref name="destFileName" /> exists.-or- An I/O error has occurred.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     <paramref name="sourceFileName" /> or <paramref name="destFileName" /> is in an invalid format.
        /// </exception>
        public static void Copy(string sourceFileName, string destFileName)
        {
            using (_PRF_Copy.Auto())
            {
                File.Copy(sourceFileName, destFileName);
            }
        }

        /// <summary>Copies an existing file to a new file. Overwriting a file of the same name is allowed.</summary>
        /// <param name="sourceFileName">The file to copy. </param>
        /// <param name="destFileName">The name of the destination file. This cannot be a directory. </param>
        /// <param name="overwrite">
        ///     <see langword="true" /> if the destination file can be overwritten; otherwise, <see langword="false" />.
        /// </param>
        /// <exception cref="T:System.UnauthorizedAccessException">
        ///     The caller does not have the required permission. -or-
        ///     <paramref name="destFileName" /> is read-only.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="sourceFileName" /> or <paramref name="destFileName" /> is a zero-length string, contains only white space, or contains one or
        ///     more invalid characters as defined by <see cref="F:System.IO.Path.InvalidPathChars" />.-or-
        ///     <paramref name="sourceFileName" /> or <paramref name="destFileName" /> specifies a directory.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="sourceFileName" /> or <paramref name="destFileName" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">
        ///     The path specified in <paramref name="sourceFileName" /> or <paramref name="destFileName" />
        ///     is invalid (for example, it is on an unmapped drive).
        /// </exception>
        /// <exception cref="T:System.IO.FileNotFoundException">
        ///     <paramref name="sourceFileName" /> was not found.
        /// </exception>
        /// <exception cref="T:System.IO.IOException">
        ///     <paramref name="destFileName" /> exists and <paramref name="overwrite" /> is <see langword="false" />.-or- An I/O error has occurred.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     <paramref name="sourceFileName" /> or <paramref name="destFileName" /> is in an invalid format.
        /// </exception>
        public static void Copy(string sourceFileName, string destFileName, bool overwrite)
        {
            using (_PRF_Copy.Auto())
            {
                File.Copy(sourceFileName, destFileName, overwrite);
            }
        }

        /// <summary>Creates or overwrites a file in the specified path.</summary>
        /// <param name="path">The path and name of the file to create. </param>
        /// <returns>A <see cref="T:System.IO.Stream" /> that provides read/write access to the file specified in <paramref name="path" />.</returns>
        /// <exception cref="T:System.UnauthorizedAccessException">
        ///     The caller does not have the required permission.-or-
        ///     <paramref name="path" /> specified a file that is read-only.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by
        ///     <see cref="F:System.IO.Path.InvalidPathChars" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive). </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred while creating the file. </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     <paramref name="path" /> is in an invalid format.
        /// </exception>
        public static Stream Create(string path)
        {
            using (_PRF_Create.Auto())
            {
                return File.Create(path);
            }
        }

        /// <summary>Creates or overwrites the specified file.</summary>
        /// <param name="path">The name of the file. </param>
        /// <param name="bufferSize">The number of bytes buffered for reads and writes to the file. </param>
        /// <returns>
        ///     A <see cref="T:System.IO.Stream" /> with the specified buffer size that provides read/write access to the file specified in
        ///     <paramref name="path" />.
        /// </returns>
        /// <exception cref="T:System.UnauthorizedAccessException">
        ///     The caller does not have the required permission.-or-
        ///     <paramref name="path" /> specified a file that is read-only.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by
        ///     <see cref="F:System.IO.Path.InvalidPathChars" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive). </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred while creating the file. </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     <paramref name="path" /> is in an invalid format.
        /// </exception>
        public static Stream Create(string path, int bufferSize)
        {
            using (_PRF_Create.Auto())
            {
                return File.Create(path, bufferSize);
            }
        }

        /// <summary>
        ///     Creates or overwrites the specified file, specifying a buffer size and a <see cref="T:System.IO.FileOptions" /> value that describes how to
        ///     create or overwrite the file.
        /// </summary>
        /// <param name="path">The name of the file. </param>
        /// <param name="bufferSize">The number of bytes buffered for reads and writes to the file. </param>
        /// <param name="options">One of the <see cref="T:System.IO.FileOptions" /> values that describes how to create or overwrite the file.</param>
        /// <returns>A new file with the specified buffer size.</returns>
        /// <exception cref="T:System.UnauthorizedAccessException">
        ///     The caller does not have the required permission.-or-
        ///     <paramref name="path" /> specified a file that is read-only. -or-
        ///     <see cref="F:System.IO.FileOptions.Encrypted" /> is specified for <paramref name="options" /> and file encryption is not supported on the current
        ///     platform.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by
        ///     <see cref="F:System.IO.Path.InvalidPathChars" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive. </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred while creating the file. </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     <paramref name="path" /> is in an invalid format.
        /// </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">
        ///     The caller does not have the required permission.-or-
        ///     <paramref name="path" /> specified a file that is read-only.
        /// </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">
        ///     The caller does not have the required permission.-or-
        ///     <paramref name="path" /> specified a file that is read-only.
        /// </exception>
        public static Stream Create(string path, int bufferSize, FileOptions options)
        {
            using (_PRF_Create.Auto())
            {
                return File.Create(path, bufferSize, options);
            }
        }

        /// <summary>Creates or opens a file for writing UTF-8 encoded text.</summary>
        /// <param name="path">The file to be opened for writing. </param>
        /// <returns>A <see cref="T:System.IO.StreamWriter" /> that writes to the specified file using UTF-8 encoding.</returns>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by
        ///     <see cref="F:System.IO.Path.InvalidPathChars" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive). </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     <paramref name="path" /> is in an invalid format.
        /// </exception>
        public static StreamWriter CreateText(string path)
        {
            using (_PRF_CreateText.Auto())
            {
                return File.CreateText(path);
            }
        }

        /// <summary>Deletes the specified file. </summary>
        /// <param name="path">The name of the file to be deleted. Wildcard characters are not supported.</param>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by
        ///     <see cref="F:System.IO.Path.InvalidPathChars" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive). </exception>
        /// <exception cref="T:System.IO.IOException">
        ///     The specified file is in use. -or-There is an open handle on the file, and the operating system is Windows
        ///     XP or earlier. This open handle can result from enumerating directories and files. For more information, see How to: Enumerate Directories and
        ///     Files.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     <paramref name="path" /> is in an invalid format.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">
        ///     The caller does not have the required permission.-or- The file is an executable file that is in use.-or-
        ///     <paramref name="path" /> is a directory.-or-
        ///     <paramref name="path" /> specified a read-only file.
        /// </exception>
        public static void Delete(string path)
        {
            using (_PRF_Delete.Auto())
            {
                File.Delete(path);
            }
        }

        /// <summary>Determines whether the specified file exists.</summary>
        /// <param name="path">The file to check. </param>
        /// <returns>
        ///     <see langword="true" /> if the caller has the required permissions and <paramref name="path" /> contains the name of an existing file; otherwise,
        ///     <see langword="false" />. This method also returns <see langword="false" /> if <paramref name="path" /> is <see langword="null" />, an invalid
        ///     path, or a zero-length string. If the caller does not have sufficient permissions to read the specified file, no exception is thrown and the
        ///     method returns <see langword="false" /> regardless of the existence of <paramref name="path" />.
        /// </returns>
        public static bool Exists(string path)
        {
            using (_PRF_Exists.Auto())
            {
                return File.Exists(path);
            }
        }

        /// <summary>Returns the creation date and time of the specified file or directory.</summary>
        /// <param name="path">The file or directory for which to obtain creation date and time information. </param>
        /// <returns>
        ///     A <see cref="T:System.DateTime" /> structure set to the creation date and time for the specified file or directory. This value is expressed
        ///     in local time.
        /// </returns>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by
        ///     <see cref="F:System.IO.Path.InvalidPathChars" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     <paramref name="path" /> is in an invalid format.
        /// </exception>
        public static DateTime GetCreationTime(string path)
        {
            using (_PRF_GetCreationTime.Auto())
            {
                return File.GetCreationTime(path);
            }
        }

        /// <summary>Returns the creation date and time, in coordinated universal time (UTC), of the specified file or directory.</summary>
        /// <param name="path">The file or directory for which to obtain creation date and time information. </param>
        /// <returns>
        ///     A <see cref="T:System.DateTime" /> structure set to the creation date and time for the specified file or directory. This value is expressed
        ///     in UTC time.
        /// </returns>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by
        ///     <see cref="F:System.IO.Path.InvalidPathChars" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     <paramref name="path" /> is in an invalid format.
        /// </exception>
        public static DateTime GetCreationTimeUtc(string path)
        {
            using (_PRF_GetCreationTimeUtc.Auto())
            {
                return File.GetCreationTimeUtc(path);
            }
        }

        /// <summary>Returns the date and time the specified file or directory was last accessed.</summary>
        /// <param name="path">The file or directory for which to obtain access date and time information. </param>
        /// <returns>
        ///     A <see cref="T:System.DateTime" /> structure set to the date and time that the specified file or directory was last accessed. This value is
        ///     expressed in local time.
        /// </returns>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by
        ///     <see cref="F:System.IO.Path.InvalidPathChars" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     <paramref name="path" /> is in an invalid format.
        /// </exception>
        public static DateTime GetLastAccessTime(string path)
        {
            using (_PRF_GetLastAccessTime.Auto())
            {
                return File.GetLastAccessTime(path);
            }
        }

        /// <summary>Returns the date and time, in coordinated universal time (UTC), that the specified file or directory was last accessed.</summary>
        /// <param name="path">The file or directory for which to obtain access date and time information. </param>
        /// <returns>
        ///     A <see cref="T:System.DateTime" /> structure set to the date and time that the specified file or directory was last accessed. This value is
        ///     expressed in UTC time.
        /// </returns>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by
        ///     <see cref="F:System.IO.Path.InvalidPathChars" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     <paramref name="path" /> is in an invalid format.
        /// </exception>
        public static DateTime GetLastAccessTimeUtc(string path)
        {
            using (_PRF_GetLastAccessTimeUtc.Auto())
            {
                return File.GetLastAccessTimeUtc(path);
            }
        }

        /// <summary>Returns the date and time the specified file or directory was last written to.</summary>
        /// <param name="path">The file or directory for which to obtain write date and time information. </param>
        /// <returns>
        ///     A <see cref="T:System.DateTime" /> structure set to the date and time that the specified file or directory was last written to. This value
        ///     is expressed in local time.
        /// </returns>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by
        ///     <see cref="F:System.IO.Path.InvalidPathChars" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     <paramref name="path" /> is in an invalid format.
        /// </exception>
        public static DateTime GetLastWriteTime(string path)
        {
            using (_PRF_GetLastWriteTime.Auto())
            {
                return File.GetLastWriteTime(path);
            }
        }

        /// <summary>Returns the date and time, in coordinated universal time (UTC), that the specified file or directory was last written to.</summary>
        /// <param name="path">The file or directory for which to obtain write date and time information. </param>
        /// <returns>
        ///     A <see cref="T:System.DateTime" /> structure set to the date and time that the specified file or directory was last written to. This value
        ///     is expressed in UTC time.
        /// </returns>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by
        ///     <see cref="F:System.IO.Path.InvalidPathChars" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     <paramref name="path" /> is in an invalid format.
        /// </exception>
        public static DateTime GetLastWriteTimeUtc(string path)
        {
            using (_PRF_GetLastWriteTimeUtc.Auto())
            {
                return File.GetLastWriteTimeUtc(path);
            }
        }

        /// <summary>Moves a specified file to a new location, providing the option to specify a new file name.</summary>
        /// <param name="sourceFileName">The name of the file to move. Can include a relative or absolute path.</param>
        /// <param name="destFileName">The new path and name for the file.</param>
        /// <exception cref="T:System.IO.IOException">
        ///     The destination file already exists.-or-
        ///     <paramref name="sourceFileName" /> was not found.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="sourceFileName" /> or <paramref name="destFileName" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="sourceFileName" /> or <paramref name="destFileName" /> is a zero-length string, contains only white space, or contains invalid
        ///     characters as defined in <see cref="F:System.IO.Path.InvalidPathChars" />.
        /// </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">
        ///     The path specified in <paramref name="sourceFileName" /> or <paramref name="destFileName" />
        ///     is invalid, (for example, it is on an unmapped drive).
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     <paramref name="sourceFileName" /> or <paramref name="destFileName" /> is in an invalid format.
        /// </exception>
        public static void Move(string sourceFileName, string destFileName)
        {
            using (_PRF_Move.Auto())
            {
                File.Move(sourceFileName, destFileName);
            }
        }

        /// <summary>Opens a <see cref="T:System.IO.Stream" /> on the specified path with read/write access.</summary>
        /// <param name="path">The file to open. </param>
        /// <param name="mode">
        ///     A <see cref="T:System.IO.FileMode" /> value that specifies whether a file is created if one does not exist, and determines whether
        ///     the contents of existing files are retained or overwritten.
        /// </param>
        /// <returns>A <see cref="T:System.IO.Stream" /> opened in the specified mode and path, with read/write access and not shared.</returns>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by
        ///     <see cref="F:System.IO.Path.InvalidPathChars" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid, (for example, it is on an unmapped drive). </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred while opening the file. </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">
        ///     <paramref name="path" /> specified a file that is read-only.-or- This operation is not supported on the current platform.-or-
        ///     <paramref name="path" /> specified a directory.-or- The caller does not have the required permission. -or-
        ///     <paramref name="mode" /> is <see cref="F:System.IO.FileMode.Create" /> and the specified file is a hidden file.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="mode" /> specified an invalid value.
        /// </exception>
        /// <exception cref="T:System.IO.FileNotFoundException">The file specified in <paramref name="path" /> was not found. </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     <paramref name="path" /> is in an invalid format.
        /// </exception>
        public static Stream Open(string path, FileMode mode)
        {
            using (_PRF_Open.Auto())
            {
                return File.Open(path, mode);
            }
        }

        /// <summary>Opens a <see cref="T:System.IO.Stream" /> on the specified path, with the specified mode and access.</summary>
        /// <param name="path">The file to open. </param>
        /// <param name="mode">
        ///     A <see cref="T:System.IO.FileMode" /> value that specifies whether a file is created if one does not exist, and determines whether
        ///     the contents of existing files are retained or overwritten.
        /// </param>
        /// <param name="access">A <see cref="T:System.IO.FileAccess" /> value that specifies the operations that can be performed on the file. </param>
        /// <returns>An unshared <see cref="T:System.IO.Stream" /> that provides access to the specified file, with the specified mode and access.</returns>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by
        ///     <see cref="F:System.IO.Path.InvalidPathChars" />.-or-
        ///     <paramref name="access" /> specified <see langword="Read" /> and <paramref name="mode" /> specified <see langword="Create" />,
        ///     <see langword="CreateNew" />, <see langword="Truncate" />, or <see langword="Append" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid, (for example, it is on an unmapped drive). </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred while opening the file. </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">
        ///     <paramref name="path" /> specified a file that is read-only and <paramref name="access" /> is not <see langword="Read" />.-or-
        ///     <paramref name="path" /> specified a directory.-or- The caller does not have the required permission. -or-
        ///     <paramref name="mode" /> is <see cref="F:System.IO.FileMode.Create" /> and the specified file is a hidden file.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="mode" /> or <paramref name="access" /> specified an invalid value.
        /// </exception>
        /// <exception cref="T:System.IO.FileNotFoundException">The file specified in <paramref name="path" /> was not found. </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     <paramref name="path" /> is in an invalid format.
        /// </exception>
        public static Stream Open(string path, FileMode mode, FileAccess access)
        {
            using (_PRF_Open.Auto())
            {
                return File.Open(path, mode, access);
            }
        }

        /// <summary>
        ///     Opens a <see cref="T:System.IO.Stream" /> on the specified path, having the specified mode with read, write, or read/write access and
        ///     the specified sharing option.
        /// </summary>
        /// <param name="path">The file to open. </param>
        /// <param name="mode">
        ///     A <see cref="T:System.IO.FileMode" /> value that specifies whether a file is created if one does not exist, and determines whether
        ///     the contents of existing files are retained or overwritten.
        /// </param>
        /// <param name="access">A <see cref="T:System.IO.FileAccess" /> value that specifies the operations that can be performed on the file. </param>
        /// <param name="share">A <see cref="T:System.IO.FileShare" /> value specifying the type of access other threads have to the file. </param>
        /// <returns>
        ///     A <see cref="T:System.IO.Stream" /> on the specified path, having the specified mode with read, write, or read/write access and the
        ///     specified sharing option.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by
        ///     <see cref="F:System.IO.Path.InvalidPathChars" />.-or-
        ///     <paramref name="access" /> specified <see langword="Read" /> and <paramref name="mode" /> specified <see langword="Create" />,
        ///     <see langword="CreateNew" />, <see langword="Truncate" />, or <see langword="Append" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid, (for example, it is on an unmapped drive). </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred while opening the file. </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">
        ///     <paramref name="path" /> specified a file that is read-only and <paramref name="access" /> is not <see langword="Read" />.-or-
        ///     <paramref name="path" /> specified a directory.-or- The caller does not have the required permission. -or-
        ///     <paramref name="mode" /> is <see cref="F:System.IO.FileMode.Create" /> and the specified file is a hidden file.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="mode" />, <paramref name="access" />, or <paramref name="share" /> specified an invalid value.
        /// </exception>
        /// <exception cref="T:System.IO.FileNotFoundException">The file specified in <paramref name="path" /> was not found. </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     <paramref name="path" /> is in an invalid format.
        /// </exception>
        public static Stream Open(string path, FileMode mode, FileAccess access, FileShare share)
        {
            using (_PRF_Open.Auto())
            {
                return File.Open(path, mode, access, share);
            }
        }

        /// <summary>Opens an existing file for reading.</summary>
        /// <param name="path">The file to be opened for reading. </param>
        /// <returns>A read-only <see cref="T:System.IO.Stream" /> on the specified path.</returns>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by
        ///     <see cref="F:System.IO.Path.InvalidPathChars" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid, (for example, it is on an unmapped drive). </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">
        ///     <paramref name="path" /> specified a directory.-or- The caller does not have the required permission.
        /// </exception>
        /// <exception cref="T:System.IO.FileNotFoundException">The file specified in <paramref name="path" /> was not found. </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     <paramref name="path" /> is in an invalid format.
        /// </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred while opening the file. </exception>
        public static Stream OpenRead(string path)
        {
            using (_PRF_OpenRead.Auto())
            {
                return File.OpenRead(path);
            }
        }

        /// <summary>Opens an existing UTF-8 encoded text file for reading.</summary>
        /// <param name="path">The file to be opened for reading. </param>
        /// <returns>A <see cref="T:System.IO.StreamReader" /> on the specified path.</returns>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by
        ///     <see cref="F:System.IO.Path.InvalidPathChars" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid, (for example, it is on an unmapped drive). </exception>
        /// <exception cref="T:System.IO.FileNotFoundException">The file specified in <paramref name="path" /> was not found. </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     <paramref name="path" /> is in an invalid format.
        /// </exception>
        public static StreamReader OpenText(string path)
        {
            using (_PRF_OpenText.Auto())
            {
                return File.OpenText(path);
            }
        }

        /// <summary>Opens an existing file or creates a new file for writing.</summary>
        /// <param name="path">The file to be opened for writing. </param>
        /// <returns>An unshared <see cref="T:System.IO.Stream" /> object on the specified path with <see cref="F:System.IO.FileAccess.Write" /> access.</returns>
        /// <exception cref="T:System.UnauthorizedAccessException">
        ///     The caller does not have the required permission.-or-
        ///     <paramref name="path" /> specified a read-only file or directory.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by
        ///     <see cref="F:System.IO.Path.InvalidPathChars" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid, (for example, it is on an unmapped drive). </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     <paramref name="path" /> is in an invalid format.
        /// </exception>
        public static Stream OpenWrite(string path)
        {
            using (_PRF_OpenWrite.Auto())
            {
                return File.OpenWrite(path);
            }
        }

        /// <summary>Opens a binary file, reads the contents of the file into a byte array, and then closes the file.</summary>
        /// <param name="path">The file to open for reading. </param>
        /// <returns>A byte array containing the contents of the file.</returns>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by
        ///     <see cref="F:System.IO.Path.InvalidPathChars" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive). </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred while opening the file. </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">
        ///     This operation is not supported on the current platform.-or-
        ///     <paramref name="path" /> specified a directory.-or- The caller does not have the required permission.
        /// </exception>
        /// <exception cref="T:System.IO.FileNotFoundException">The file specified in <paramref name="path" /> was not found. </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     <paramref name="path" /> is in an invalid format.
        /// </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public static byte[] ReadAllBytes(string path)
        {
            using (_PRF_ReadAllBytes.Auto())
            {
                return File.ReadAllBytes(path);
            }
        }

        /// <summary>Opens a text file, reads all lines of the file, and then closes the file.</summary>
        /// <param name="path">The file to open for reading. </param>
        /// <returns>A string array containing all lines of the file.</returns>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by
        ///     <see cref="F:System.IO.Path.InvalidPathChars" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive). </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred while opening the file. </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">
        ///     <paramref name="path" /> specified a file that is read-only.-or- This operation is not supported on the current platform.-or-
        ///     <paramref name="path" /> specified a directory.-or- The caller does not have the required permission.
        /// </exception>
        /// <exception cref="T:System.IO.FileNotFoundException">The file specified in <paramref name="path" /> was not found. </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     <paramref name="path" /> is in an invalid format.
        /// </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public static string[] ReadAllLines(string path)
        {
            using (_PRF_ReadAllLines.Auto())
            {
                return File.ReadAllLines(path);
            }
        }

        /// <summary>Opens a file, reads all lines of the file with the specified encoding, and then closes the file.</summary>
        /// <param name="path">The file to open for reading. </param>
        /// <param name="encoding">The encoding applied to the contents of the file. </param>
        /// <returns>A string array containing all lines of the file.</returns>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by
        ///     <see cref="F:System.IO.Path.InvalidPathChars" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive). </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred while opening the file. </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">
        ///     <paramref name="path" /> specified a file that is read-only.-or- This operation is not supported on the current platform.-or-
        ///     <paramref name="path" /> specified a directory.-or- The caller does not have the required permission.
        /// </exception>
        /// <exception cref="T:System.IO.FileNotFoundException">The file specified in <paramref name="path" /> was not found. </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     <paramref name="path" /> is in an invalid format.
        /// </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public static string[] ReadAllLines(string path, Encoding encoding)
        {
            using (_PRF_ReadAllLines.Auto())
            {
                return File.ReadAllLines(path, encoding);
            }
        }

        /// <summary>Opens a text file, reads all lines of the file, and then closes the file.</summary>
        /// <param name="path">The file to open for reading. </param>
        /// <returns>A string containing all lines of the file.</returns>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by
        ///     <see cref="F:System.IO.Path.InvalidPathChars" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive). </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred while opening the file. </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">
        ///     <paramref name="path" /> specified a file that is read-only.-or- This operation is not supported on the current platform.-or-
        ///     <paramref name="path" /> specified a directory.-or- The caller does not have the required permission.
        /// </exception>
        /// <exception cref="T:System.IO.FileNotFoundException">The file specified in <paramref name="path" /> was not found. </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     <paramref name="path" /> is in an invalid format.
        /// </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public static string ReadAllText(string path)
        {
            using (_PRF_ReadAllText.Auto())
            {
                return File.ReadAllText(path);
            }
        }

        /// <summary>Opens a file, reads all lines of the file with the specified encoding, and then closes the file.</summary>
        /// <param name="path">The file to open for reading. </param>
        /// <param name="encoding">The encoding applied to the contents of the file. </param>
        /// <returns>A string containing all lines of the file.</returns>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by
        ///     <see cref="F:System.IO.Path.InvalidPathChars" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive). </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred while opening the file. </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">
        ///     <paramref name="path" /> specified a file that is read-only.-or- This operation is not supported on the current platform.-or-
        ///     <paramref name="path" /> specified a directory.-or- The caller does not have the required permission.
        /// </exception>
        /// <exception cref="T:System.IO.FileNotFoundException">The file specified in <paramref name="path" /> was not found. </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     <paramref name="path" /> is in an invalid format.
        /// </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public static string ReadAllText(string path, Encoding encoding)
        {
            using (_PRF_ReadAllText.Auto())
            {
                return File.ReadAllText(path, encoding);
            }
        }

        /// <summary>Reads the lines of a file.</summary>
        /// <param name="path">The file to read.</param>
        /// <returns>All the lines of the file, or the lines that are the result of a query.</returns>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters defined by the
        ///     <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">
        ///     <paramref name="path" /> is invalid (for example, it is on an unmapped drive).
        /// </exception>
        /// <exception cref="T:System.IO.FileNotFoundException">The file specified by <paramref name="path" /> was not found.</exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred while opening the file.</exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     <paramref name="path" /> exceeds the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248
        ///     characters and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission.</exception>
        /// <exception cref="T:System.UnauthorizedAccessException">
        ///     <paramref name="path" /> specifies a file that is read-only.-or-This operation is not supported on the current platform.-or-
        ///     <paramref name="path" /> is a directory.-or-The caller does not have the required permission.
        /// </exception>
        public static IEnumerable<string> ReadLines(string path)
        {
            using (_PRF_ReadLines.Auto())
            {
                return File.ReadLines(path);
            }
        }

        /// <summary>Read the lines of a file that has a specified encoding.</summary>
        /// <param name="path">The file to read.</param>
        /// <param name="encoding">The encoding that is applied to the contents of the file. </param>
        /// <returns>All the lines of the file, or the lines that are the result of a query.</returns>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by the
        ///     <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">
        ///     <paramref name="path" /> is invalid (for example, it is on an unmapped drive).
        /// </exception>
        /// <exception cref="T:System.IO.FileNotFoundException">The file specified by <paramref name="path" /> was not found.</exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred while opening the file.</exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     <paramref name="path" /> exceeds the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248
        ///     characters and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission.</exception>
        /// <exception cref="T:System.UnauthorizedAccessException">
        ///     <paramref name="path" /> specifies a file that is read-only.-or-This operation is not supported on the current platform.-or-
        ///     <paramref name="path" /> is a directory.-or-The caller does not have the required permission.
        /// </exception>
        public static IEnumerable<string> ReadLines(string path, Encoding encoding)
        {
            using (_PRF_ReadLines.Auto())
            {
                return File.ReadLines(path, encoding);
            }
        }

        /// <summary>
        ///     Replaces the contents of a specified file with the contents of another file, deleting the original file, and creating a backup of the
        ///     replaced file.
        /// </summary>
        /// <param name="sourceFileName">The name of a file that replaces the file specified by <paramref name="destinationFileName" />.</param>
        /// <param name="destinationFileName">The name of the file being replaced.</param>
        /// <param name="destinationBackupFileName">The name of the backup file.</param>
        /// <exception cref="T:System.ArgumentException">
        ///     The path described by the <paramref name="destinationFileName" /> parameter was not of a legal
        ///     form.-or-The path described by the <paramref name="destinationBackupFileName" /> parameter was not of a legal form.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="destinationFileName" /> parameter is <see langword="null" />.</exception>
        /// <exception cref="T:System.IO.DriveNotFoundException">An invalid drive was specified. </exception>
        /// <exception cref="T:System.IO.FileNotFoundException">
        ///     The file described by the current <see cref="T:Appalachia.CI.Integration.FileSystem.AppaFileInfo" /> object could not be
        ///     found.-or-The file described by the <paramref name="destinationBackupFileName" /> parameter could not be found.
        /// </exception>
        /// <exception cref="T:System.IO.IOException">
        ///     An I/O error occurred while opening the file.- or -The <paramref name="sourceFileName" /> and
        ///     <paramref name="destinationFileName" /> parameters specify the same file.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.PlatformNotSupportedException">
        ///     The operating system is Windows 98 Second Edition or earlier and the files system is not
        ///     NTFS.
        /// </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">
        ///     The <paramref name="sourceFileName" /> or <paramref name="destinationFileName" /> parameter
        ///     specifies a file that is read-only.-or- This operation is not supported on the current platform.-or- Source or destination parameters specify a
        ///     directory instead of a file.-or- The caller does not have the required permission.
        /// </exception>
        public static void Replace(
            string sourceFileName,
            string destinationFileName,
            string destinationBackupFileName)
        {
            using (_PRF_Replace.Auto())
            {
                File.Replace(sourceFileName, destinationFileName, destinationBackupFileName);
            }
        }

        /// <summary>
        ///     Replaces the contents of a specified file with the contents of another file, deleting the original file, and creating a backup of the
        ///     replaced file and optionally ignores merge errors.
        /// </summary>
        /// <param name="sourceFileName">The name of a file that replaces the file specified by <paramref name="destinationFileName" />.</param>
        /// <param name="destinationFileName">The name of the file being replaced.</param>
        /// <param name="destinationBackupFileName">The name of the backup file.</param>
        /// <param name="ignoreMetadataErrors">
        ///     <see langword="true" /> to ignore merge errors (such as attributes and access control lists (ACLs)) from the replaced file to the replacement
        ///     file; otherwise, <see langword="false" />.
        /// </param>
        /// <exception cref="T:System.ArgumentException">
        ///     The path described by the <paramref name="destinationFileName" /> parameter was not of a legal
        ///     form.-or-The path described by the <paramref name="destinationBackupFileName" /> parameter was not of a legal form.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="destinationFileName" /> parameter is <see langword="null" />.</exception>
        /// <exception cref="T:System.IO.DriveNotFoundException">An invalid drive was specified. </exception>
        /// <exception cref="T:System.IO.FileNotFoundException">
        ///     The file described by the current <see cref="T:Appalachia.CI.Integration.FileSystem.AppaFileInfo" /> object could not be
        ///     found.-or-The file described by the <paramref name="destinationBackupFileName" /> parameter could not be found.
        /// </exception>
        /// <exception cref="T:System.IO.IOException">
        ///     An I/O error occurred while opening the file.- or -The <paramref name="sourceFileName" /> and
        ///     <paramref name="destinationFileName" /> parameters specify the same file.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.PlatformNotSupportedException">
        ///     The operating system is Windows 98 Second Edition or earlier and the files system is not
        ///     NTFS.
        /// </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">
        ///     The <paramref name="sourceFileName" /> or <paramref name="destinationFileName" /> parameter
        ///     specifies a file that is read-only.-or- This operation is not supported on the current platform.-or- Source or destination parameters specify a
        ///     directory instead of a file.-or- The caller does not have the required permission.
        /// </exception>
        public static void Replace(
            string sourceFileName,
            string destinationFileName,
            string destinationBackupFileName,
            bool ignoreMetadataErrors)
        {
            using (_PRF_Replace.Auto())
            {
                File.Replace(
                    sourceFileName,
                    destinationFileName,
                    destinationBackupFileName,
                    ignoreMetadataErrors
                );
            }
        }

        /// <summary>Sets the date and time the file was created.</summary>
        /// <param name="path">The file for which to set the creation date and time information. </param>
        /// <param name="creationTime">
        ///     A <see cref="T:System.DateTime" /> containing the value to set for the creation date and time of <paramref name="path" />.
        ///     This value is expressed in local time.
        /// </param>
        /// <exception cref="T:System.IO.FileNotFoundException">The specified path was not found. </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by
        ///     <see cref="F:System.IO.Path.InvalidPathChars" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred while performing the operation. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="creationTime" /> specifies a value outside the range of dates, times, or both permitted for this operation.
        /// </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     <paramref name="path" /> is in an invalid format.
        /// </exception>
        public static void SetCreationTime(string path, DateTime creationTime)
        {
            using (_PRF_SetCreationTime.Auto())
            {
                File.SetCreationTime(path, creationTime);
            }
        }

        /// <summary>Sets the date and time, in coordinated universal time (UTC), that the file was created.</summary>
        /// <param name="path">The file for which to set the creation date and time information. </param>
        /// <param name="creationTimeUtc">
        ///     A <see cref="T:System.DateTime" /> containing the value to set for the creation date and time of
        ///     <paramref name="path" />. This value is expressed in UTC time.
        /// </param>
        /// <exception cref="T:System.IO.FileNotFoundException">The specified path was not found. </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by
        ///     <see cref="F:System.IO.Path.InvalidPathChars" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred while performing the operation. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="creationTime" /> specifies a value outside the range of dates, times, or both permitted for this operation.
        /// </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     <paramref name="path" /> is in an invalid format.
        /// </exception>
        public static void SetCreationTimeUtc(string path, DateTime creationTimeUtc)
        {
            using (_PRF_SetCreationTimeUtc.Auto())
            {
                File.SetCreationTimeUtc(path, creationTimeUtc);
            }
        }

        /// <summary>Sets the date and time the specified file was last accessed.</summary>
        /// <param name="path">The file for which to set the access date and time information. </param>
        /// <param name="lastAccessTime">
        ///     A <see cref="T:System.DateTime" /> containing the value to set for the last access date and time of
        ///     <paramref name="path" />. This value is expressed in local time.
        /// </param>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by
        ///     <see cref="F:System.IO.Path.InvalidPathChars" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.IO.FileNotFoundException">The specified path was not found. </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     <paramref name="path" /> is in an invalid format.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="lastAccessTime" /> specifies a value outside the range of dates or times permitted for this operation.
        /// </exception>
        public static void SetLastAccessTime(string path, DateTime lastAccessTime)
        {
            using (_PRF_SetLastAccessTime.Auto())
            {
                File.SetLastAccessTime(path, lastAccessTime);
            }
        }

        /// <summary>Sets the date and time, in coordinated universal time (UTC), that the specified file was last accessed.</summary>
        /// <param name="path">The file for which to set the access date and time information. </param>
        /// <param name="lastAccessTimeUtc">
        ///     A <see cref="T:System.DateTime" /> containing the value to set for the last access date and time of
        ///     <paramref name="path" />. This value is expressed in UTC time.
        /// </param>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by
        ///     <see cref="F:System.IO.Path.InvalidPathChars" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.IO.FileNotFoundException">The specified path was not found. </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     <paramref name="path" /> is in an invalid format.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="lastAccessTimeUtc" /> specifies a value outside the range of dates or times permitted for this operation.
        /// </exception>
        public static void SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc)
        {
            using (_PRF_SetLastAccessTimeUtc.Auto())
            {
                File.SetLastAccessTimeUtc(path, lastAccessTimeUtc);
            }
        }

        /// <summary>Sets the date and time that the specified file was last written to.</summary>
        /// <param name="path">The file for which to set the date and time information. </param>
        /// <param name="lastWriteTime">
        ///     A <see cref="T:System.DateTime" /> containing the value to set for the last write date and time of
        ///     <paramref name="path" />. This value is expressed in local time.
        /// </param>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by
        ///     <see cref="F:System.IO.Path.InvalidPathChars" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.IO.FileNotFoundException">The specified path was not found. </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     <paramref name="path" /> is in an invalid format.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="lastWriteTime" /> specifies a value outside the range of dates or times permitted for this operation.
        /// </exception>
        public static void SetLastWriteTime(string path, DateTime lastWriteTime)
        {
            using (_PRF_SetLastWriteTime.Auto())
            {
                File.SetLastWriteTime(path, lastWriteTime);
            }
        }

        /// <summary>Sets the date and time, in coordinated universal time (UTC), that the specified file was last written to.</summary>
        /// <param name="path">The file for which to set the date and time information. </param>
        /// <param name="lastWriteTimeUtc">
        ///     A <see cref="T:System.DateTime" /> containing the value to set for the last write date and time of
        ///     <paramref name="path" />. This value is expressed in UTC time.
        /// </param>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by
        ///     <see cref="F:System.IO.Path.InvalidPathChars" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.IO.FileNotFoundException">The specified path was not found. </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     <paramref name="path" /> is in an invalid format.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="lastWriteTimeUtc" /> specifies a value outside the range of dates or times permitted for this operation.
        /// </exception>
        public static void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc)
        {
            using (_PRF_SetLastWriteTimeUtc.Auto())
            {
                File.SetLastWriteTimeUtc(path, lastWriteTimeUtc);
            }
        }

        /// <summary>
        ///     Creates a new file, writes the specified byte array to the file, and then closes the file. If the target file already exists, it is
        ///     overwritten.
        /// </summary>
        /// <param name="path">The file to write to. </param>
        /// <param name="bytes">The bytes to write to the file. </param>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by
        ///     <see cref="F:System.IO.Path.InvalidPathChars" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" /> or the byte array is empty.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive). </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred while opening the file. </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">
        ///     <paramref name="path" /> specified a file that is read-only.-or- This operation is not supported on the current platform.-or-
        ///     <paramref name="path" /> specified a directory.-or- The caller does not have the required permission.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     <paramref name="path" /> is in an invalid format.
        /// </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public static void WriteAllBytes(string path, byte[] bytes)
        {
            using (_PRF_WriteAllBytes.Auto())
            {
                File.WriteAllBytes(path, bytes);
            }
        }

        /// <summary>Creates a new file, write the specified string array to the file, and then closes the file. </summary>
        /// <param name="path">The file to write to. </param>
        /// <param name="contents">The string array to write to the file. </param>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by
        ///     <see cref="F:System.IO.Path.InvalidPathChars" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">Either <paramref name="path" /> or <paramref name="contents" /> is <see langword="null" />.  </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive). </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred while opening the file. </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">
        ///     <paramref name="path" /> specified a file that is read-only.-or- This operation is not supported on the current platform.-or-
        ///     <paramref name="path" /> specified a directory.-or- The caller does not have the required permission.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     <paramref name="path" /> is in an invalid format.
        /// </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public static void WriteAllLines(string path, string[] contents)
        {
            using (_PRF_WriteAllLines.Auto())
            {
                File.WriteAllLines(path, contents);
            }
        }

        /// <summary>Creates a new file, writes the specified string array to the file by using the specified encoding, and then closes the file. </summary>
        /// <param name="path">The file to write to. </param>
        /// <param name="contents">The string array to write to the file. </param>
        /// <param name="encoding">An <see cref="T:System.Text.Encoding" /> object that represents the character encoding applied to the string array.</param>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by
        ///     <see cref="F:System.IO.Path.InvalidPathChars" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">Either <paramref name="path" /> or <paramref name="contents" /> is <see langword="null" />.  </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive). </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred while opening the file. </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">
        ///     <paramref name="path" /> specified a file that is read-only.-or- This operation is not supported on the current platform.-or-
        ///     <paramref name="path" /> specified a directory.-or- The caller does not have the required permission.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     <paramref name="path" /> is in an invalid format.
        /// </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public static void WriteAllLines(string path, string[] contents, Encoding encoding)
        {
            using (_PRF_WriteAllLines.Auto())
            {
                File.WriteAllLines(path, contents, encoding);
            }
        }

        /// <summary>Creates a new file, writes a collection of strings to the file, and then closes the file.</summary>
        /// <param name="path">The file to write to.</param>
        /// <param name="contents">The lines to write to the file.</param>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters defined by the
        ///     <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">Either<paramref name=" path " />or <paramref name="contents" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">
        ///     <paramref name="path" /> is invalid (for example, it is on an unmapped drive).
        /// </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred while opening the file.</exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     <paramref name="path" /> exceeds the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248
        ///     characters and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     <paramref name="path" /> is in an invalid format.
        /// </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission.</exception>
        /// <exception cref="T:System.UnauthorizedAccessException">
        ///     <paramref name="path" /> specifies a file that is read-only.-or-This operation is not supported on the current platform.-or-
        ///     <paramref name="path" /> is a directory.-or-The caller does not have the required permission.
        /// </exception>
        public static void WriteAllLines(string path, IEnumerable<string> contents)
        {
            using (_PRF_WriteAllLines.Auto())
            {
                File.WriteAllLines(path, contents);
            }
        }

        /// <summary>Creates a new file by using the specified encoding, writes a collection of strings to the file, and then closes the file.</summary>
        /// <param name="path">The file to write to.</param>
        /// <param name="contents">The lines to write to the file.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters defined by the
        ///     <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     Either<paramref name=" path" />,<paramref name=" contents" />, or <paramref name="encoding" /> is
        ///     <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">
        ///     <paramref name="path" /> is invalid (for example, it is on an unmapped drive).
        /// </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred while opening the file.</exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     <paramref name="path" /> exceeds the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248
        ///     characters and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     <paramref name="path" /> is in an invalid format.
        /// </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission.</exception>
        /// <exception cref="T:System.UnauthorizedAccessException">
        ///     <paramref name="path" /> specifies a file that is read-only.-or-This operation is not supported on the current platform.-or-
        ///     <paramref name="path" /> is a directory.-or-The caller does not have the required permission.
        /// </exception>
        public static void WriteAllLines(string path, IEnumerable<string> contents, Encoding encoding)
        {
            using (_PRF_WriteAllLines.Auto())
            {
                File.WriteAllLines(path, contents, encoding);
            }
        }

        /// <summary>Creates a new file, writes the specified string to the file, and then closes the file. If the target file already exists, it is overwritten.</summary>
        /// <param name="path">The file to write to. </param>
        /// <param name="contents">The string to write to the file. </param>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by
        ///     <see cref="F:System.IO.Path.InvalidPathChars" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" /> or <paramref name="contents" /> is empty.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive). </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred while opening the file. </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">
        ///     <paramref name="path" /> specified a file that is read-only.-or- This operation is not supported on the current platform.-or-
        ///     <paramref name="path" /> specified a directory.-or- The caller does not have the required permission.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     <paramref name="path" /> is in an invalid format.
        /// </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public static void WriteAllText(string path, string contents)
        {
            using (_PRF_WriteAllText.Auto())
            {
                File.WriteAllText(path, contents);
            }
        }

        /// <summary>
        ///     Creates a new file, writes the specified string to the file using the specified encoding, and then closes the file. If the target file
        ///     already exists, it is overwritten.
        /// </summary>
        /// <param name="path">The file to write to. </param>
        /// <param name="contents">The string to write to the file. </param>
        /// <param name="encoding">The encoding to apply to the string.</param>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by
        ///     <see cref="F:System.IO.Path.InvalidPathChars" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="path" /> is <see langword="null" /> or <paramref name="contents" /> is empty.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive). </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred while opening the file. </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">
        ///     <paramref name="path" /> specified a file that is read-only.-or- This operation is not supported on the current platform.-or-
        ///     <paramref name="path" /> specified a directory.-or- The caller does not have the required permission.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     <paramref name="path" /> is in an invalid format.
        /// </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public static void WriteAllText(string path, string contents, Encoding encoding)
        {
            using (_PRF_WriteAllText.Auto())
            {
                File.WriteAllText(path, contents, encoding);
            }
        }
    }
}
