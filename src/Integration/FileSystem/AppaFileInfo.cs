using System.IO;
using Appalachia.CI.Integration.Extensions;
using Unity.Profiling;

namespace Appalachia.CI.Integration.FileSystem
{
    public sealed class AppaFileInfo : AppaFileSystemInfo
    {
#region Profiling And Tracing Markers

        private const string _PRF_PFX = nameof(AppaFileInfo) + ".";
        private static readonly ProfilerMarker _PRF_OpenText = new(_PRF_PFX + nameof(OpenText));
        private static readonly ProfilerMarker _PRF_CreateText = new(_PRF_PFX + nameof(CreateText));
        private static readonly ProfilerMarker _PRF_AppendText = new(_PRF_PFX + nameof(AppendText));

        private static readonly ProfilerMarker _PRF_CopyTo = new(_PRF_PFX + nameof(CopyTo));

        private static readonly ProfilerMarker _PRF_Create = new(_PRF_PFX + nameof(Create));

        private static readonly ProfilerMarker _PRF_Delete = new(_PRF_PFX + nameof(Delete));
        private static readonly ProfilerMarker _PRF_Decrypt = new(_PRF_PFX + nameof(Decrypt));

        private static readonly ProfilerMarker _PRF_Encrypt = new(_PRF_PFX + nameof(Encrypt));

        private static readonly ProfilerMarker _PRF_Open = new(_PRF_PFX + nameof(Open));
        private static readonly ProfilerMarker _PRF_OpenRead = new(_PRF_PFX + nameof(OpenRead));
        private static readonly ProfilerMarker _PRF_OpenWrite = new(_PRF_PFX + nameof(OpenWrite));
        private static readonly ProfilerMarker _PRF_MoveTo = new(_PRF_PFX + nameof(MoveTo));
        private static readonly ProfilerMarker _PRF_Replace = new(_PRF_PFX + nameof(Replace));
        private static readonly ProfilerMarker _PRF_ToString = new(_PRF_PFX + nameof(ToString));

        private static readonly ProfilerMarker _PRF_IsWithinSubdirectoryOf =
            new(_PRF_PFX + nameof(IsWithinSubdirectoryOf));

        private static readonly ProfilerMarker _PRF_ReadAllText = new(_PRF_PFX + nameof(ReadAllText));
        private static readonly ProfilerMarker _PRF_ReadAllLines = new(_PRF_PFX + nameof(ReadAllLines));

#endregion

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Appalachia.CI.Integration.FileSystem.AppaFileInfo" /> class, which acts as a wrapper for a
        ///     file path.
        /// </summary>
        /// <param name="fileName">
        ///     The fully qualified name of the new file, or the relative file name. Do not end the path with the directory separator
        ///     character.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="fileName" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.ArgumentException">The file name is empty, contains only white spaces, or contains invalid characters. </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">Access to <paramref name="fileName" /> is denied. </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     <paramref name="fileName" /> contains a colon (:) in the middle of the string.
        /// </exception>
        public AppaFileInfo(string fileName)
        {
            _fileInfo = new FileInfo(fileName);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Appalachia.CI.Integration.FileSystem.AppaFileInfo" /> class, which acts as a wrapper for a
        ///     file.
        /// </summary>
        /// <param name="info">
        ///     The fully qualified name of the new file, or the relative file name. Do not end the path with the directory separator
        ///     character.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="info" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.ArgumentException">The file name is empty, contains only white spaces, or contains invalid characters. </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">Access to <paramref name="info" /> is denied. </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     <paramref name="info" /> contains a colon (:) in the middle of the string.
        /// </exception>
        public AppaFileInfo(FileInfo info)
        {
            _fileInfo = info;
        }

        private readonly FileInfo _fileInfo;

        private string _lastDirectoryName;

        /// <summary>Gets a value indicating whether a file exists.</summary>
        /// <returns>
        ///     <see langword="true" /> if the file exists; <see langword="false" /> if the file does not exist or if the file is a directory.
        /// </returns>
        public override bool Exists => _fileInfo.Exists;

        public override AppaDirectoryInfo Parent => _fileInfo.Directory;

        /// <summary>Gets the full path of the file.</summary>
        /// <returns>A string containing the full path.</returns>
        /// <exception cref="T:System.IO.PathTooLongException">The fully qualified path and file name is 260 or more characters.</exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public override string FullPath => _fileInfo.FullName.CleanFullPath();

        /// <summary>Gets the name of the file.</summary>
        /// <returns>The name of the file.</returns>
        public override string Name => _fileInfo.Name;

        public override string RelativePath => FullPath.ToRelativePath();

        /// <summary>Gets an instance of the parent directory.</summary>
        /// <returns>A <see cref="T:Appalachia.CI.Integration.FileSystem.AppaDirectoryInfo" /> object representing the parent directory of this file.</returns>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive. </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public AppaDirectoryInfo Directory => _fileInfo.Directory;

        public long FileSize => Length;

        /// <summary>Gets the size, in bytes, of the current file.</summary>
        /// <returns>The size of the current file in bytes.</returns>
        /// <exception cref="T:System.IO.IOException">
        ///     <see cref="M:Appalachia.CI.Integration.FileSystem.FileSystemInfo.Refresh" /> cannot update the state of the file or directory.
        /// </exception>
        /// <exception cref="T:System.IO.FileNotFoundException">The file does not exist.-or- The <see langword="Length" /> property is called for a directory. </exception>
        public long Length => _fileInfo.Length;

        /// <summary>Gets a string representing the directory's full path.</summary>
        /// <returns>A string representing the directory's full path.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <see langword="null" /> was passed in for the directory name.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">The fully qualified path is 260 or more characters.</exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public string DirectoryPath => _fileInfo.DirectoryName.CleanFullPath();

        public string WindowsDirectoryPath => _fileInfo.DirectoryName;

        /// <summary>Gets a string representing the directory's last folder name.</summary>
        /// <returns>A string representing the directory's full path.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <see langword="null" /> was passed in for the directory name.
        /// </exception>
        public string LastDirectoryName
        {
            get
            {
                if (_lastDirectoryName == null)
                {
                    var splits = DirectoryPath.Split('/');
                    _lastDirectoryName = splits[splits.Length - 1];
                }

                return _lastDirectoryName;
            }
        }

        /// <summary>Gets or sets a value that determines if the current file is read only.</summary>
        /// <returns>
        ///     <see langword="true" /> if the current file is read only; otherwise, <see langword="false" />.
        /// </returns>
        /// <exception cref="T:System.IO.FileNotFoundException">
        ///     The file described by the current <see cref="T:Appalachia.CI.Integration.FileSystem.AppaFileInfo" /> object could not be
        ///     found.
        /// </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred while opening the file.</exception>
        /// <exception cref="T:System.UnauthorizedAccessException">
        ///     This operation is not supported on the current platform.-or- The caller does not have the
        ///     required permission.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">The user does not have write permission, but attempted to set this property to <see langword="false" />.</exception>
        public bool IsReadOnly
        {
            get => _fileInfo.IsReadOnly;
            set => _fileInfo.IsReadOnly = value;
        }

        /// <summary>
        ///     Creates a <see cref="T:System.IO.StreamWriter" /> that appends text to the file represented by this instance of the
        ///     <see cref="T:Appalachia.CI.Integration.FileSystem.AppaFileInfo" />.
        /// </summary>
        /// <returns>A new <see langword="StreamWriter" />.</returns>
        public StreamWriter AppendText()
        {
            using (_PRF_AppendText.Auto())
            {
                return _fileInfo.AppendText();
            }
        }

        /// <summary>Copies an existing file to a new file, disallowing the overwriting of an existing file.</summary>
        /// <param name="destFileName">The name of the new file to copy to. </param>
        /// <returns>A new file with a fully qualified path.</returns>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="destFileName" /> is empty, contains only white spaces, or contains invalid characters.
        /// </exception>
        /// <exception cref="T:System.IO.IOException">An error occurs, or the destination file already exists. </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="destFileName" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">A directory path is passed in, or the file is being moved to a different drive. </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The directory specified in <paramref name="destFileName" /> does not exist.</exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     <paramref name="destFileName" /> contains a colon (:) within the string but does not specify the volume.
        /// </exception>
        public AppaFileInfo CopyTo(string destFileName)
        {
            using (_PRF_CopyTo.Auto())
            {
                return _fileInfo.CopyTo(destFileName);
            }
        }

        /// <summary>Copies an existing file to a new file, allowing the overwriting of an existing file.</summary>
        /// <param name="destFileName">The name of the new file to copy to. </param>
        /// <param name="overwrite">
        ///     <see langword="true" /> to allow an existing file to be overwritten; otherwise, <see langword="false" />.
        /// </param>
        /// <returns>
        ///     A new file, or an overwrite of an existing file if <paramref name="overwrite" /> is <see langword="true" />. If the file exists and
        ///     <paramref name="overwrite" /> is <see langword="false" />, an <see cref="T:System.IO.IOException" /> is thrown.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="destFileName" /> is empty, contains only white spaces, or contains invalid characters.
        /// </exception>
        /// <exception cref="T:System.IO.IOException">
        ///     An error occurs, or the destination file already exists and <paramref name="overwrite" /> is
        ///     <see langword="false" />.
        /// </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="destFileName" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The directory specified in <paramref name="destFileName" /> does not exist.</exception>
        /// <exception cref="T:System.UnauthorizedAccessException">A directory path is passed in, or the file is being moved to a different drive. </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     <paramref name="destFileName" /> contains a colon (:) in the middle of the string.
        /// </exception>
        public AppaFileInfo CopyTo(string destFileName, bool overwrite)
        {
            using (_PRF_CopyTo.Auto())
            {
                return _fileInfo.CopyTo(destFileName, overwrite);
            }
        }

        /// <summary>Creates a file.</summary>
        /// <returns>A new file.</returns>
        public Stream Create()
        {
            using (_PRF_Create.Auto())
            {
                return _fileInfo.Create();
            }
        }

        /// <summary>Creates a <see cref="T:System.IO.StreamWriter" /> that writes a new text file.</summary>
        /// <returns>A new <see langword="StreamWriter" />.</returns>
        /// <exception cref="T:System.UnauthorizedAccessException">The file name is a directory. </exception>
        /// <exception cref="T:System.IO.IOException">The disk is read-only. </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public StreamWriter CreateText()
        {
            using (_PRF_CreateText.Auto())
            {
                return _fileInfo.CreateText();
            }
        }

        /// <summary>
        ///     Decrypts a file that was encrypted by the current account using the
        ///     <see cref="M:Appalachia.CI.Integration.FileSystem.AppaFileInfo.Encrypt" /> method.
        /// </summary>
        /// <exception cref="T:System.IO.DriveNotFoundException">An invalid drive was specified. </exception>
        /// <exception cref="T:System.IO.FileNotFoundException">
        ///     The file described by the current <see cref="T:Appalachia.CI.Integration.FileSystem.AppaFileInfo" /> object could not be
        ///     found.
        /// </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred while opening the file.</exception>
        /// <exception cref="T:System.NotSupportedException">The file system is not NTFS.</exception>
        /// <exception cref="T:System.PlatformNotSupportedException">The current operating system is not Microsoft Windows NT or later.</exception>
        /// <exception cref="T:System.UnauthorizedAccessException">
        ///     The file described by the current <see cref="T:Appalachia.CI.Integration.FileSystem.AppaFileInfo" /> object is
        ///     read-only.-or- This operation is not supported on the current platform.-or- The caller does not have the required permission.
        /// </exception>
        public void Decrypt()
        {
            using (_PRF_Decrypt.Auto())
            {
                _fileInfo.Decrypt();
            }
        }

        /// <summary>Encrypts a file so that only the account used to encrypt the file can decrypt it.</summary>
        /// <exception cref="T:System.IO.DriveNotFoundException">An invalid drive was specified. </exception>
        /// <exception cref="T:System.IO.FileNotFoundException">
        ///     The file described by the current <see cref="T:Appalachia.CI.Integration.FileSystem.AppaFileInfo" /> object could not be
        ///     found.
        /// </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred while opening the file.</exception>
        /// <exception cref="T:System.NotSupportedException">The file system is not NTFS.</exception>
        /// <exception cref="T:System.PlatformNotSupportedException">The current operating system is not Microsoft Windows NT or later.</exception>
        /// <exception cref="T:System.UnauthorizedAccessException">
        ///     The file described by the current <see cref="T:Appalachia.CI.Integration.FileSystem.AppaFileInfo" /> object is
        ///     read-only.-or- This operation is not supported on the current platform.-or- The caller does not have the required permission.
        /// </exception>
        public void Encrypt()
        {
            using (_PRF_Encrypt.Auto())
            {
                _fileInfo.Encrypt();
            }
        }

        public bool IsWithinSubdirectoryOf(string path)
        {
            using (_PRF_IsWithinSubdirectoryOf.Auto())
            {
                var directory = new AppaDirectoryInfo(path);
                return directory.IsFileInAnySubdirectory(this);
            }
        }

        public bool IsWithinSubdirectoryOf(AppaDirectoryInfo directory)
        {
            using (_PRF_IsWithinSubdirectoryOf.Auto())
            {
                return directory.IsFileInAnySubdirectory(this);
            }
        }

        /// <summary>Moves a specified file to a new location, providing the option to specify a new file name.</summary>
        /// <param name="destFileName">The path to move the file to, which can specify a different file name. </param>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs, such as the destination file already exists or the destination device is not ready. </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="destFileName" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="destFileName" /> is empty, contains only white spaces, or contains invalid characters.
        /// </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">
        ///     <paramref name="destFileName" /> is read-only or is a directory.
        /// </exception>
        /// <exception cref="T:System.IO.FileNotFoundException">The file is not found. </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive. </exception>
        /// <exception cref="T:System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length. For example, on
        ///     Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     <paramref name="destFileName" /> contains a colon (:) in the middle of the string.
        /// </exception>
        public void MoveTo(string destFileName)
        {
            using (_PRF_MoveTo.Auto())
            {
                _fileInfo.MoveTo(destFileName);
            }
        }

        /// <summary>Opens a file in the specified mode.</summary>
        /// <param name="mode">
        ///     A <see cref="T:System.IO.FileMode" /> constant specifying the mode (for example, <see langword="Open" /> or
        ///     <see langword="Append" />) in which to open the file.
        /// </param>
        /// <returns>A file opened in the specified mode, with read/write access and unshared.</returns>
        /// <exception cref="T:System.IO.FileNotFoundException">The file is not found. </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">The file is read-only or is a directory. </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive. </exception>
        /// <exception cref="T:System.IO.IOException">The file is already open. </exception>
        public Stream Open(FileMode mode)
        {
            using (_PRF_Open.Auto())
            {
                return _fileInfo.Open(mode);
            }
        }

        /// <summary>Opens a file in the specified mode with read, write, or read/write access.</summary>
        /// <param name="mode">
        ///     A <see cref="T:System.IO.FileMode" /> constant specifying the mode (for example, <see langword="Open" /> or
        ///     <see langword="Append" />) in which to open the file.
        /// </param>
        /// <param name="access">
        ///     A <see cref="T:System.IO.FileAccess" /> constant specifying whether to open the file with <see langword="Read" />,
        ///     <see langword="Write" />, or <see langword="ReadWrite" /> file access.
        /// </param>
        /// <returns>A <see cref="T:System.IO.Stream" /> object opened in the specified mode and access, and unshared.</returns>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.IO.FileNotFoundException">The file is not found. </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">
        ///     <paramref name="path" /> is read-only or is a directory.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive. </exception>
        /// <exception cref="T:System.IO.IOException">The file is already open. </exception>
        public Stream Open(FileMode mode, FileAccess access)
        {
            using (_PRF_Open.Auto())
            {
                return _fileInfo.Open(mode, access);
            }
        }

        /// <summary>Opens a file in the specified mode with read, write, or read/write access and the specified sharing option.</summary>
        /// <param name="mode">
        ///     A <see cref="T:System.IO.FileMode" /> constant specifying the mode (for example, <see langword="Open" /> or
        ///     <see langword="Append" />) in which to open the file.
        /// </param>
        /// <param name="access">
        ///     A <see cref="T:System.IO.FileAccess" /> constant specifying whether to open the file with <see langword="Read" />,
        ///     <see langword="Write" />, or <see langword="ReadWrite" /> file access.
        /// </param>
        /// <param name="share">
        ///     A <see cref="T:System.IO.FileShare" /> constant specifying the type of access other <see langword="Stream" /> objects have to
        ///     this file.
        /// </param>
        /// <returns>A <see cref="T:System.IO.Stream" /> object opened with the specified mode, access, and sharing options.</returns>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.IO.FileNotFoundException">The file is not found. </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">
        ///     <paramref name="path" /> is read-only or is a directory.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive. </exception>
        /// <exception cref="T:System.IO.IOException">The file is already open. </exception>
        public Stream Open(FileMode mode, FileAccess access, FileShare share)
        {
            using (_PRF_Open.Auto())
            {
                return _fileInfo.Open(mode, access, share);
            }
        }

        /// <summary>Creates a read-only <see cref="T:System.IO.Stream" />.</summary>
        /// <returns>A new read-only <see cref="T:System.IO.Stream" /> object.</returns>
        /// <exception cref="T:System.UnauthorizedAccessException">
        ///     <paramref name="path" /> is read-only or is a directory.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive. </exception>
        /// <exception cref="T:System.IO.IOException">The file is already open. </exception>
        public Stream OpenRead()
        {
            using (_PRF_OpenRead.Auto())
            {
                return _fileInfo.OpenRead();
            }
        }

        /// <summary>Creates a <see cref="T:System.IO.StreamReader" /> with UTF8 encoding that reads from an existing text file.</summary>
        /// <returns>A new <see langword="StreamReader" /> with UTF8 encoding.</returns>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.IO.FileNotFoundException">The file is not found. </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">
        ///     <paramref name="path" /> is read-only or is a directory.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive. </exception>
        public StreamReader OpenText()
        {
            using (_PRF_OpenText.Auto())
            {
                return _fileInfo.OpenText();
            }
        }

        /// <summary>Creates a write-only <see cref="T:System.IO.Stream" />.</summary>
        /// <returns>A write-only unshared <see cref="T:System.IO.Stream" /> object for a new or existing file.</returns>
        /// <exception cref="T:System.UnauthorizedAccessException">
        ///     The path specified when creating an instance of the <see cref="T:Appalachia.CI.Integration.FileSystem.AppaFileInfo" />
        ///     object is read-only or is a directory.
        /// </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">
        ///     The path specified when creating an instance of the <see cref="T:Appalachia.CI.Integration.FileSystem.AppaFileInfo" />
        ///     object is invalid, such as being on an unmapped drive.
        /// </exception>
        public Stream OpenWrite()
        {
            using (_PRF_OpenWrite.Auto())
            {
                return _fileInfo.OpenWrite();
            }
        }

        public string[] ReadAllLines()
        {
            using (_PRF_ReadAllLines.Auto())
            {
                return AppaFile.ReadAllLines(FullPath);
            }
        }

        public string ReadAllText()
        {
            using (_PRF_ReadAllText.Auto())
            {
                return AppaFile.ReadAllText(FullPath);
            }
        }

        /// <summary>
        ///     Replaces the contents of a specified file with the file described by the current
        ///     <see cref="T:Appalachia.CI.Integration.FileSystem.AppaFileInfo" /> object, deleting
        ///     the original file, and creating a backup of the replaced file.
        /// </summary>
        /// <param name="destinationFileName">The name of a file to replace with the current file.</param>
        /// <param name="destinationBackupFileName">
        ///     The name of a file with which to create a backup of the file described by the
        ///     <paramref name="destFileName" /> parameter.
        /// </param>
        /// <returns>
        ///     A <see cref="T:Appalachia.CI.Integration.FileSystem.AppaFileInfo" /> object that encapsulates information about the file described by the
        ///     <paramref name="destFileName" /> parameter.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        ///     The path described by the <paramref name="destFileName" /> parameter was not of a legal form.-or-The
        ///     path described by the <paramref name="destBackupFileName" /> parameter was not of a legal form.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="destFileName" /> parameter is <see langword="null" />.</exception>
        /// <exception cref="T:System.IO.FileNotFoundException">
        ///     The file described by the current <see cref="T:Appalachia.CI.Integration.FileSystem.AppaFileInfo" /> object could not be
        ///     found.-or-The file described by the <paramref name="destinationFileName" /> parameter could not be found.
        /// </exception>
        /// <exception cref="T:System.PlatformNotSupportedException">The current operating system is not Microsoft Windows NT or later.</exception>
        public AppaFileInfo Replace(string destinationFileName, string destinationBackupFileName)
        {
            using (_PRF_Replace.Auto())
            {
                return _fileInfo.Replace(destinationFileName, destinationBackupFileName);
            }
        }

        /// <summary>
        ///     Replaces the contents of a specified file with the file described by the current
        ///     <see cref="T:Appalachia.CI.Integration.FileSystem.AppaFileInfo" /> object, deleting
        ///     the original file, and creating a backup of the replaced file.  Also specifies whether to ignore merge errors.
        /// </summary>
        /// <param name="destinationFileName">The name of a file to replace with the current file.</param>
        /// <param name="destinationBackupFileName">
        ///     The name of a file with which to create a backup of the file described by the
        ///     <paramref name="destFileName" /> parameter.
        /// </param>
        /// <param name="ignoreMetadataErrors">
        ///     <see langword="true" /> to ignore merge errors (such as attributes and ACLs) from the replaced file to the replacement file; otherwise
        ///     <see langword="false" />.
        /// </param>
        /// <returns>
        ///     A <see cref="T:Appalachia.CI.Integration.FileSystem.AppaFileInfo" /> object that encapsulates information about the file described by the
        ///     <paramref name="destFileName" /> parameter.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        ///     The path described by the <paramref name="destFileName" /> parameter was not of a legal form.-or-The
        ///     path described by the <paramref name="destBackupFileName" /> parameter was not of a legal form.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="destFileName" /> parameter is <see langword="null" />.</exception>
        /// <exception cref="T:System.IO.FileNotFoundException">
        ///     The file described by the current <see cref="T:Appalachia.CI.Integration.FileSystem.AppaFileInfo" /> object could not be
        ///     found.-or-The file described by the <paramref name="destinationFileName" /> parameter could not be found.
        /// </exception>
        /// <exception cref="T:System.PlatformNotSupportedException">The current operating system is not Microsoft Windows NT or later.</exception>
        public AppaFileInfo Replace(
            string destinationFileName,
            string destinationBackupFileName,
            bool ignoreMetadataErrors)
        {
            using (_PRF_Replace.Auto())
            {
                return _fileInfo.Replace(
                    destinationFileName,
                    destinationBackupFileName,
                    ignoreMetadataErrors
                );
            }
        }

        /// <summary>Permanently deletes a file.</summary>
        /// <exception cref="T:System.IO.IOException">
        ///     The target file is open or memory-mapped on a computer running Microsoft Windows NT.-or-There is an open
        ///     handle on the file, and the operating system is Windows XP or earlier. This open handle can result from enumerating directories and files. For
        ///     more information, see How to: Enumerate Directories and Files.
        /// </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.UnauthorizedAccessException">The path is a directory. </exception>
        public override void Delete()
        {
            using (_PRF_Delete.Auto())
            {
                _fileInfo.Delete();
            }
        }

        /// <summary>Returns the path as a string.</summary>
        /// <returns>A string representing the path.</returns>
        public override string ToString()
        {
            using (_PRF_ToString.Auto())
            {
                return _fileInfo.ToString().CleanFullPath();
            }
        }

        protected override FileSystemInfo GetFileSystemInfo()
        {
            return _fileInfo;
        }

        public static implicit operator AppaFileInfo(FileInfo o)
        {
            return new(o);
        }

        public static implicit operator FileInfo(AppaFileInfo o)
        {
            return o._fileInfo;
        }
    }
}
