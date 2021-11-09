using System.IO;
using Appalachia.CI.Integration.Extensions;
using UnityEngine;

namespace Appalachia.CI.Integration.FileSystem
{
    public abstract class AppaFileSystemInfo
    {
        public abstract AppaDirectoryInfo Parent { get; }
        public abstract bool Exists { get; }
        public abstract string FullPath { get; }
        public abstract string Name { get; }
        public abstract string RelativePath { get; }

        /// <summary>Gets a string representing the directory's full path.</summary>
        /// <returns>A string representing the directory's full path.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <see langword="null" /> was passed in for the directory name.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">The fully qualified path is 260 or more characters.</exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public string ParentDirectoryFullPath => Parent.FullPath.CleanFullPath();

        /// <summary>Gets a string representing the directory's full path.</summary>
        /// <returns>A string representing the directory's full path.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <see langword="null" /> was passed in for the directory name.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">The fully qualified path is 260 or more characters.</exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public string ParentDirectoryRelativePath => Parent.RelativePath.CleanFullPath();

        /// <summary>Gets a string representing the directory's full path, formatted for windows system calls.</summary>
        /// <returns>A string representing the directory's full path.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <see langword="null" /> was passed in for the directory name.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">The fully qualified path is 260 or more characters.</exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>

        public string ParentWindowsDirectoryFullPath => Parent.FullPath;

        /// <summary>Gets a string representing the directory's full path, formatted for windows system calls.</summary>
        /// <returns>A string representing the directory's full path.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <see langword="null" /> was passed in for the directory name.
        /// </exception>
        /// <exception cref="T:System.IO.PathTooLongException">The fully qualified path is 260 or more characters.</exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>

        public string ParentWindowsDirectoryRelativePath =>
            FullPath.Replace(Application.dataPath, string.Empty);

        public abstract void Delete();

        protected abstract FileSystemInfo GetFileSystemInfo();

        public static implicit operator AppaFileSystemInfo(FileSystemInfo o)
        {
            return o is FileInfo fi ? (AppaFileInfo) fi : (AppaDirectoryInfo) (DirectoryInfo) o;
        }

        public static implicit operator FileSystemInfo(AppaFileSystemInfo o)
        {
            return o.GetFileSystemInfo();
        }
    }
}
