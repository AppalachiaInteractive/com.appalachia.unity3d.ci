using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Appalachia.CI.Integration.Assemblies;
using Appalachia.CI.Integration.FileSystem;
using Appalachia.CI.Integration.Packages;
using Appalachia.CI.Integration.Repositories;
using Appalachia.Utility.Enums;
using Unity.Profiling;
using UnityEditor;

namespace Appalachia.CI.Integration.Core
{
    [Serializable, InitializeOnLoad]
    public abstract class IntegrationMetadata
    {
        public abstract string Id { get; }
        public abstract string Name { get; }
        public abstract string Path { get; }

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
        private static string _lastFlagCheckName;

        private static string _lastFlagCheckPath;

        public AppaDirectoryInfo directory;
        public bool readOnly;

        public IntegrationTypeFlags flags;

        public static IReadOnlyDictionary<string, T> InstancesByID => _instancesByID;

        public static IReadOnlyDictionary<string, T> InstancesByName => _instancesByName;

        public static IReadOnlyDictionary<string, T> InstancesByPath => _instancesByPath;

        public static IReadOnlyList<T> Instances => _instances;

        public bool IsAsset => flags.HasFlag(IntegrationTypeFlags.IsAsset);
        public bool IsLibrary => flags.HasFlag(IntegrationTypeFlags.IsLibrary);
        public bool IsPackage => flags.HasFlag(IntegrationTypeFlags.IsPackage);
        public bool IsAppalachia => flags.HasFlag(IntegrationTypeFlags.IsAppalachia);
        public bool IsBuiltinUnity => flags.HasFlag(IntegrationTypeFlags.IsBuiltinUnity);
        public bool IsCustomUnity => flags.HasFlag(IntegrationTypeFlags.IsCustomUnity);
        public bool IsAppalachiaManaged => IsAppalachia || IsThirdParty || IsCustomUnity;
        public bool IsThirdParty => !IsAppalachia && !IsUnity;
        public bool IsUnity => IsBuiltinUnity || IsCustomUnity;

        /*
        private string _unifiedName;
        
        public string UnifiedName
        {
            get
            {
                if (_unifiedName == null)
                {
                    var n = Name.ToLowerInvariant();

                    if (n.StartsWith("com."))
                    {
                        if (IsAppalachia)
                        {
                            n = n.Replace("com.appalachia.unity3d", "appalachia");
                        }
                        else
                        {
                            n = n.Replace("com.appalachia.unity3d.third-party", null);
                        }
                    }
                    else
                    {
                        
                    }
                    
                    
                }
                
                return _unifiedName;
            }
        }*/

        public event ReanalyzeHandler OnReanalyzeNecessary;

        public abstract void InitializeForAnalysis();
        public abstract int CompareTo(object obj);

        public abstract int CompareTo(T other);
        public abstract bool Equals(T other);

        protected abstract IEnumerable<string> GetIds();

        protected virtual void ExecuteReanalyzeNecessary()
        {
            OnReanalyzeNecessary?.Invoke();
        }

        public static IReadOnlyList<T> FindAll()
        {
            ProcessAll();

            return _instances;
        }

        public static T FindByFile(AppaFileInfo file)
        {
            CheckIntegrationRegistration();

            return InstancesByPath[file.RelativePath];
        }

        public static T FindByFilePath(string path)
        {
            CheckIntegrationRegistration();

            var file = new AppaFileInfo(path);
            return FindByFile(file);
        }

        public static T FindById(string id)
        {
            CheckIntegrationRegistration();

            if (!InstancesByID.ContainsKey(id))
            {
                return null;
            }

            var instance = InstancesByID[id];

            return instance;
        }

        public static T FindByName(string name)
        {
            CheckIntegrationRegistration();

            if (!InstancesByName.ContainsKey(name))
            {
                return null;
            }

            var instance = InstancesByName[name];

            return instance;
        }

        public static T FindInDirectory(AppaDirectoryInfo directory)
        {
            CheckIntegrationRegistration();

            if (!InstancesByPath.ContainsKey(directory.RelativePath))
            {
                throw new NotSupportedException(directory.RelativePath);
            }

            return InstancesByPath[directory.RelativePath];
        }

        public static T FindInDirectoryPath(string path)
        {
            CheckIntegrationRegistration();

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
                item.SetFlags();
                
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

        private static void CheckIntegrationRegistration()
        {
            if (!IntegrationMetadataRegisterExecutor.HasExecuted)
            {
                IntegrationMetadataRegisterExecutor.Execute();
            }
        }

        private void SetFlags()
        {
            var n = Name.ToLowerInvariant();
            var p = Path.ToLowerInvariant();

            if ((n == _lastFlagCheckName) && (p == _lastFlagCheckPath))
            {
                return;
            }

            _lastFlagCheckName = n;
            _lastFlagCheckPath = p;

            flags = IntegrationTypeFlags.None;

            if (p.StartsWith("assets"))
            {
                flags = flags.SetFlag(IntegrationTypeFlags.IsAsset);
            }

            if (p.StartsWith("library"))
            {
                flags = flags.SetFlag(IntegrationTypeFlags.IsLibrary);
            }

            if (p.StartsWith("package"))
            {
                flags = flags.SetFlag(IntegrationTypeFlags.IsPackage);
            }

            if (n.Contains("appalachia") && !n.Contains("third-party.") && !n.Contains("unity."))
            {
                flags = flags.SetFlag(IntegrationTypeFlags.IsAppalachia);
            }

            if (n.StartsWith("unity.") || n.StartsWith("com.unity") || n.Contains("textmeshpro") || n.StartsWith("com.autodesk"))
            {
                flags = flags.SetFlag(IntegrationTypeFlags.IsBuiltinUnity);
            }

            if ((n == "com.appalachia.unity3d.third-party.unity") ||
                n.StartsWith("unity.visualeffectgraph") ||
                n.StartsWith("cinemachine") ||
                n.StartsWith("unity.postprocessing") ||
                n.StartsWith("unity.ai.navigation"))
            {
                flags = flags.SetFlag(IntegrationTypeFlags.IsCustomUnity);
            }
        }

        #region Nested Types

        public delegate void ReanalyzeHandler();

        #endregion
    }
}
