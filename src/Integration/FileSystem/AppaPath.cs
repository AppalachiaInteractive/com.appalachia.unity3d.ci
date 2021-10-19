using System.IO;
using Appalachia.CI.Integration.Extensions;

namespace Appalachia.CI.Integration.FileSystem
{
    public static class AppaPath
    {
        public static char DirectorySeparatorChar => Path.DirectorySeparatorChar;

        public static string Combine(string path1, string path2)
        {
            return Path.Combine(path1, path2).CleanFullPath();
        }

        public static string Combine(string path1, string path2, string path3)
        {
            return Path.Combine(path1, path2, path3).CleanFullPath();
        }

        public static string Combine(string path1, string path2, string path3, string path4)
        {
            return Path.Combine(path1, path2, path3, path4).CleanFullPath();
        }

        public static string Combine(params string[] paths)
        {
            return Path.Combine(paths).CleanFullPath();
        }

        public static string GetDirectoryName(string path)
        {
            return Path.GetDirectoryName(path).CleanFullPath();
        }

        public static string GetExtension(string path)
        {
            return Path.GetExtension(path);
        }

        public static string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }

        public static string GetFileNameWithoutExtension(string path)
        {
            return Path.GetFileNameWithoutExtension(path);
        }

        public static string GetFullPath(string path)
        {
            return Path.GetFullPath(path).CleanFullPath();
        }
    }
}
