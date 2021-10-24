using System;
using System.Collections.Generic;
using System.Linq;
using Appalachia.CI.Integration.Assemblies;
using Appalachia.CI.Integration.FileSystem;
using Appalachia.CI.Integration.Packages;
using Appalachia.CI.Integration.Repositories;
using Unity.Profiling;
using UnityEditor;

namespace Appalachia.CI.Integration.Core
{
    [Serializable, InitializeOnLoad]
    public abstract class IntegrationMetadata
    {
        public static IReadOnlyList<T> ClearAll<T>()
            where T : IntegrationMetadata<T>
        {
            if (typeof(T) == typeof(AssemblyDefinitionMetadata))
            {
                IntegrationMetadataRegistry<T>.Clear();
                return FindAll<T>();
            }

            if (typeof(T) == typeof(PackageMetadata))
            {
                IntegrationMetadataRegistry<T>.Clear();
                return FindAll<T>();
            }

            if (typeof(T) == typeof(RepositoryMetadata))
            {
                IntegrationMetadataRegistry<T>.Clear();
                return FindAll<T>();
            }

            throw new NotSupportedException(typeof(T).Name);
        }

        public static IReadOnlyList<T> FindAll<T>()
            where T : IntegrationMetadata<T>
        {
            if (typeof(T) == typeof(AssemblyDefinitionMetadata))
            {
                return (IReadOnlyList<T>) AssemblyDefinitionMetadata.FindAll();
            }

            if (typeof(T) == typeof(PackageMetadata))
            {
                return (IReadOnlyList<T>) PackageMetadata.FindAll();
            }

            if (typeof(T) == typeof(RepositoryMetadata))
            {
                return (IReadOnlyList<T>) RepositoryMetadata.FindAll();
            }

            throw new NotSupportedException(typeof(T).Name);
        }
    }

    [Serializable]
    public abstract class IntegrationMetadata<T> : IntegrationMetadata,
                                                   IComparable<T>,
                                                   IComparable,
                                                   IEquatable<T>
        where T : IntegrationMetadata<T>
    {
#region Profiling And Tracing Markers

        private const string _PRF_PFX = nameof(IntegrationMetadata<T>) + ".";
        private static readonly ProfilerMarker _PRF_ToString = new(_PRF_PFX + nameof(ToString));

#endregion

        public static IReadOnlyDictionary<string, T> InstancesByID => _instancesByID;

        public static IReadOnlyDictionary<string, T> InstancesByName => _instancesByName;

        public static IReadOnlyDictionary<string, T> InstancesByPath => _instancesByPath;

        public static IReadOnlyList<T> Instances => _instances;

        static IntegrationMetadata()
        {
            Initialize();
        }

        private static bool _allPrepared;
        private static Dictionary<string, T> _instancesByID;
        private static Dictionary<string, T> _instancesByName;
        private static Dictionary<string, T> _instancesByPath;
        private static HashSet<T> _instancesUnique;
        private static List<T> _instances;

        public AppaDirectoryInfo directory;
        public bool readOnly;
        public string id;

        public abstract string Name { get; }
        public abstract string Path { get; }

        public bool IsAppalachia => Name.Contains("Appalachia") || Name.StartsWith("com.appalachia");
        public bool IsAsset => Path?.StartsWith("Asset") ?? false;
        public bool IsLibrary => Path?.StartsWith("Library") ?? false;
        public bool IsPackage => Path?.StartsWith("Package") ?? false;

        public abstract void InitializeForAnalysis();
        public abstract int CompareTo(object obj);

        public abstract int CompareTo(T other);
        public abstract bool Equals(T other);

        protected abstract IEnumerable<string> GetIds();

        public static IReadOnlyList<T> FindAll()
        {
            ProcessAll();

            return _instances;
        }

        public static T FindByFile(AppaFileInfo file)
        {
            return InstancesByPath[file.RelativePath];
        }

        public static T FindByFilePath(string path)
        {
            var file = new AppaFileInfo(path);
            return FindByFile(file);
        }

        public static T FindById(string id)
        {
            if (!InstancesByID.ContainsKey(id))
            {
                return null;
            }

            var instance = InstancesByID[id];

            return instance;
        }

        public static T FindByName(string name)
        {
            if (!InstancesByName.ContainsKey(name))
            {
                return null;
            }

            var instance = InstancesByName[name];

            return instance;
        }

        public static T FindInDirectory(AppaDirectoryInfo directory)
        {
            return InstancesByPath[directory.RelativePath];
        }

        public static T FindInDirectoryPath(string path)
        {
            var directory = new AppaDirectoryInfo(path);
            return FindInDirectory(directory);
        }

        public static void Initialize()
        {
            if (_instancesByID == null)
            {
                _instancesByID = new Dictionary<string, T>();
            }

            if (_instancesByName == null)
            {
                _instancesByName = new Dictionary<string, T>();
            }

            if (_instancesByPath == null)
            {
                _instancesByPath = new Dictionary<string, T>();
            }

            if (_instances == null)
            {
                _instances = new List<T>();
            }

            if (_instancesUnique == null)
            {
                _instancesUnique = new HashSet<T>();
            }
        }

        public static void ProcessAll()
        {
            if (_allPrepared)
            {
                return;
            }

            var all = IntegrationMetadataRegistry<T>.FindAll();

            foreach (var item in all)
            {
                var ids = item.GetIds().ToArray();

                foreach (var id in ids)
                {
                    if (id == null)
                    {
                        continue;
                    }

                    if (!_instancesByID.ContainsKey(id))
                    {
                        _instancesByID.Add(id, item);
                    }
                }

                if (!_instancesByName.ContainsKey(item.Name))
                {
                    _instancesByName.Add(item.Name, item);
                }

                if (!_instancesByPath.ContainsKey(item.Path))
                {
                    _instancesByPath.Add(item.Path, item);
                }

                _instancesUnique.Add(item);
            }

            _instances = _instancesUnique.ToList();

            _allPrepared = true;
        }

        protected static string GetFilePath(string path, bool testFile)
        {
            if (testFile)
            {
                path += ".test";
            }

            return path;
        }
    }
}
