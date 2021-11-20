using System;
using System.Data;
using Appalachia.CI.Integration.Assets;
using Appalachia.CI.Integration.Extensions;
using Appalachia.CI.Integration.FileSystem;
using Unity.Profiling;
using UnityEngine;

namespace Appalachia.CI.Integration.Core
{
    public static class AppalachiaObjectFactory
    {
        public static T CreateNew<T>(string dataFolder = null,
                                     Type ownerType = null)
            where T : ScriptableObject
        {
            using (_PRF_CreateNew.Auto())
            {
                return CreateNew(typeof(T), dataFolder, ownerType) as T;
            }
        }

        public static ScriptableObject CreateNew(Type t, string dataFolder = null,
                                                 Type ownerType = null)
        {
            using (_PRF_CreateNew.Auto())
            {
                return LoadOrCreateNew(
                    t,
                    $"{t.Name}_{DateTime.Now:yyyyMMdd-hhmmssfff}.asset",
                    false,
                    false,
                    dataFolder,
                    ownerType
                );
            }
        }

        public static ScriptableObject CreateNew(
            Type t,
            string name,
            ScriptableObject i,
            string dataFolder = null,
            Type ownerType = null)
        {
            using (_PRF_CreateNew.Auto())
            {
#if UNITY_EDITOR
                var ext = AppaPath.GetExtension(name);

                if (string.IsNullOrWhiteSpace(ext))
                {
                    name += ".asset";
                }

                if (dataFolder == null)
                {
                    dataFolder = AssetDatabaseManager.GetSaveDirectoryForScriptableObject(t, ownerType).ToRelativePath();
                }

                var assetPath = AppaPath.Combine(dataFolder, name);

                if (AppaFile.Exists(assetPath))
                {
                    throw new DuplicateNameException($"A scriptable object already exists at {assetPath}");
                }

                assetPath = assetPath.Replace(ProjectLocations.GetAssetsDirectoryPath(), "Assets");

                AssetDatabaseManager.CreateAsset(i, assetPath);

                if (i is IAppalachiaObject ao)
                {
                    ao.OnCreate();
                }

                i = AssetDatabaseManager.ImportAndLoadAssetAtPath(t, assetPath) as ScriptableObject;
#endif
                return i;
            }
        }

        public static T LoadOrCreateNew<T>(string name, string dataFolder = null,
                                           Type ownerType = null)
            where T : ScriptableObject
        {
            using (_PRF_LoadOrCreateNew.Auto())
            {
                return LoadOrCreateNew(typeof(T), name, dataFolder, ownerType) as T;
            }
        }

        public static ScriptableObject LoadOrCreateNew(Type t, string name, string dataFolder = null,
                                                       Type ownerType = null)
        {
            using (_PRF_LoadOrCreateNew.Auto())
            {
                return LoadOrCreateNew(t, name, false, false, dataFolder, ownerType);
            }
        }

        public static T LoadOrCreateNew<T>(
            string name,
            bool prependType,
            bool appendType,
            string dataFolder = null,
            Type ownerType = null)
            where T : ScriptableObject
        {
            using (_PRF_LoadOrCreateNew.Auto())
            {
                return LoadOrCreateNew(typeof(T), name, prependType, appendType, dataFolder, ownerType) as T;
            }
        }

        public static ScriptableObject LoadOrCreateNew(
            Type t,
            string name,
            bool prependType,
            bool appendType,
            string dataFolder = null,
            Type ownerType = null)
        {
            using (_PRF_LoadOrCreateNew.Auto())
            {
                var cleanFileName = name;
                var hasDot = name.Contains(".");
                var lastIsDot = name.EndsWith(".");

                if (lastIsDot)
                {
                    name += "asset";
                    cleanFileName = name.TrimEnd('.');
                }
                else if (!hasDot)
                {
                    name += ".asset";
                }
                else
                {
                    cleanFileName = AppaPath.GetFileNameWithoutExtension(name);
                }

                var extension = AppaPath.GetExtension(name);

                var tName = t.Name;

                if (prependType)
                {
                    cleanFileName = $"{tName}_{cleanFileName}";
                }

                if (appendType)
                {
                    cleanFileName = $"{cleanFileName}_{tName}";
                }

                name = $"{cleanFileName}{extension}";

#if UNITY_EDITOR
                var any = AssetDatabaseManager.FindAssets($"t:{tName} {cleanFileName}");

                for (var i = 0; i < any.Length; i++)
                {
                    var path = AssetDatabaseManager.GUIDToAssetPath(any[i]);
                    var existingName = AppaPath.GetFileNameWithoutExtension(path);

                    if ((existingName != null) &&
                        string.Equals(cleanFileName.ToLower(), existingName.ToLower()))
                    {
                        return AssetDatabaseManager.LoadAssetAtPath(path, t) as ScriptableObject;
                    }
                }
#endif
                var instance = ScriptableObject.CreateInstance(t);

                return CreateNew(t, name, instance, dataFolder, ownerType);
            }
        }

#if UNITY_EDITOR
        public static void Rename<T>(T instance, string newName)
            where T : ScriptableObject
        {
            using (_PRF_Rename.Auto())
            {
                var path = AssetDatabaseManager.GetAssetPath(instance);
                instance.name = newName;

                AssetDatabaseManager.RenameAsset(path, newName);
            }
        }
#endif

        #region Profiling

        private const string _PRF_PFX = nameof(AppalachiaObjectFactory) + ".";
        private static readonly ProfilerMarker _PRF_CreateNew = new(_PRF_PFX + nameof(CreateNew));

        private static readonly ProfilerMarker _PRF_LoadOrCreateNew = new(_PRF_PFX + nameof(LoadOrCreateNew));
#if UNITY_EDITOR
        private static readonly ProfilerMarker _PRF_Rename = new(_PRF_PFX + nameof(Rename));
#endif

        #endregion
    }
}
