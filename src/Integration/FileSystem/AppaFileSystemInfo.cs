using System.IO;

namespace Appalachia.CI.Integration.FileSystem
{
    public abstract class AppaFileSystemInfo
    {
        public abstract AppaDirectoryInfo Parent { get; }
        public abstract bool Exists { get; }
        public abstract string FullPath { get; }
        public abstract string Name { get; }
        public abstract string RelativePath { get; }

        public static implicit operator AppaFileSystemInfo(FileSystemInfo o)
        {
            return o is FileInfo fi ? (AppaFileInfo) fi : (AppaDirectoryInfo) (DirectoryInfo) o;
        }

        public static implicit operator FileSystemInfo(AppaFileSystemInfo o)
        {
            return o.GetFileSystemInfo();
        }

        public abstract void Delete();

        protected abstract FileSystemInfo GetFileSystemInfo();
    }
}
