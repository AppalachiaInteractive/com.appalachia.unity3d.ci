using System;
using System.Data;
using Appalachia.CI.Integration.Assets;
using Appalachia.CI.Integration.Extensions;
using Appalachia.CI.Integration.FileSystem;
using Appalachia.Utility.Logging;
using Unity.Profiling;
using UnityEngine;

namespace Appalachia.CI.Integration.Core
{
    public static class AppalachiaObjectFactory
    {
        public static ScriptableObject CreateNewAsset(
            Type t,
            string saveFolderPath = null,
            Type ownerType = null,
            bool overwriteExisting = false)
        {
            using (_PRF_CreateNewAsset.Auto())
            {
                return LoadExistingOrCreateNewAsset(
                    t,
                    $"{t.Name}_{DateTime.Now:yyyyMMdd-hhmmssfff}.asset",
                    false,
                    false,
                    saveFolderPath,
                    ownerType,
                    overwriteExisting
                );
            }
        }

        public static T LoadExistingOrCreateNewAsset<T>(
            string name,
            string dataFolder = null,
            Type ownerType = null,
            bool overwriteExisting = false)
            where T : ScriptableObject
        {
            using (_PRF_LoadOrCreateNew.Auto())
            {
                return LoadExistingOrCreateNewAsset(
                    typeof(T),
                    name,
                    dataFolder,
                    ownerType,
                    overwriteExisting
                ) as T;
            }
        }

        public static ScriptableObject LoadExistingOrCreateNewAsset(
            Type t,
            string name,
            string dataFolder = null,
            Type ownerType = null,
            bool overwriteExisting = false)
        {
            using (_PRF_LoadOrCreateNew.Auto())
            {
                return LoadExistingOrCreateNewAsset(
                    t,
                    name,
                    false,
                    false,
                    dataFolder,
                    ownerType,
                    overwriteExisting
                );
            }
        }

        public static T LoadExistingOrCreateNewAsset<T>(
            string name,
            bool prependType,
            bool appendType,
            string dataFolder = null,
            Type ownerType = null,
            bool overwriteExisting = false)
            where T : ScriptableObject
        {
            using (_PRF_LoadOrCreateNew.Auto())
            {
                return LoadExistingOrCreateNewAsset(
                    typeof(T),
                    name,
                    prependType,
                    appendType,
                    dataFolder,
                    ownerType,
                    overwriteExisting
                ) as T;
            }
        }

        public static ScriptableObject LoadExistingOrCreateNewAsset(
            Type t,
            string name,
            bool prependType,
            bool appendType,
            string saveFolder = null,
            Type ownerType = null,
            bool overwriteExisting = false)
        {
            using (_PRF_LoadOrCreateNew.Auto())
            {
                if (TryLoadExistingInstance(t, name, prependType, appendType, out var scriptableObject))
                {
                    return scriptableObject;
                }

                if (t.IsAbstract)
                {
                    AppaLog.Error($"Can not create ScriptableObject of type [{t.Name}] as it is abstract.");
                    return null;
                }
                
                var instance = ScriptableObject.CreateInstance(t);

                if (instance == null)
                {
                    AppaLog.Error($"Could not create ScriptableObject of type [{t.Name}]");
                    return null;
                }
                
                var cleanRelativeFilePath = GetCleanRelativeFilePath(name);

                return SaveInstanceToAsset(
                    t,
                    cleanRelativeFilePath,
                    instance,
                    saveFolder,
                    ownerType,
                    overwriteExisting
                );
            }
        }

#if UNITY_EDITOR
        public static void RenameAsset<T>(T instance, string newName)
            where T : ScriptableObject
        {
            using (_PRF_RenameAsset.Auto())
            {
                var path = AssetDatabaseManager.GetAssetPath(instance);
                instance.name = newName;

                AssetDatabaseManager.RenameAsset(path, newName);
            }
        }
#endif

        public static T SaveInstanceToAsset<T, TOwner>(
            string name,
            T i,
            string saveFolderPath = null,
            bool overwriteExisting = false)
            where T : ScriptableObject
        {
            using (_PRF_SaveInstanceToAsset.Auto())
            {
                return SaveInstanceToAsset(
                    typeof(T),
                    name,
                    i,
                    saveFolderPath,
                    typeof(TOwner),
                    overwriteExisting
                ) as T;
            }
        }

        public static T SaveInstanceToAsset<T>(
            string name,
            T i,
            string saveFolderPath = null,
            Type ownerType = null,
            bool overwriteExisting = false)
            where T : ScriptableObject
        {
            using (_PRF_SaveInstanceToAsset.Auto())
            {
                return SaveInstanceToAsset(
                    typeof(T),
                    name,
                    i,
                    saveFolderPath,
                    ownerType,
                    overwriteExisting
                ) as T;
            }
        }

        public static ScriptableObject SaveInstanceToAsset(
            Type t,
            string name,
            ScriptableObject i,
            string saveFolderPath = null,
            Type ownerType = null,
            bool overwriteExisting = false)
        {
            using (_PRF_SaveInstanceToAsset.Auto())
            {
#if UNITY_EDITOR
                var ext = AppaPath.GetExtension(name);

                if (string.IsNullOrWhiteSpace(ext))
                {
                    name += ".asset";
                }

                if (saveFolderPath == null)
                {
                    saveFolderPath = AssetDatabaseManager.GetSaveDirectoryForScriptableObject(t, ownerType)
                                                         .ToRelativePath();
                }

                var assetPath = AppaPath.Combine(saveFolderPath, name);

                if (AppaFile.Exists(assetPath))
                {
                    if (overwriteExisting)
                    {
                        AssetDatabaseManager.DeleteAsset(assetPath);
                    }
                    else
                    {
                        throw new DuplicateNameException(
                            $"A scriptable object already exists at {assetPath}"
                        );
                    }
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

        public static bool TryLoadExistingInstance<T>(string name, out T instance)
            where T : ScriptableObject
        {
            using (_PRF_TryLoadExistingInstance.Auto())
            {
                var result = TryLoadExistingInstance(typeof(T), name, out var scriptableObject);

                instance = scriptableObject as T;
                return result;
            }
        }

        public static bool TryLoadExistingInstance(Type t, string name, out ScriptableObject instance)
        {
            using (_PRF_TryLoadExistingInstance.Auto())
            {
                return TryLoadExistingInstance(t, name, false, false, out instance);
            }
        }

        public static bool TryLoadExistingInstance(
            Type t,
            string name,
            bool prependType,
            bool appendType,
            out ScriptableObject instance)
        {
            using (_PRF_TryLoadExistingInstance.Auto())
            {
                var cleanRelativeFilePath = GetCleanRelativeFilePath(name);

                return TryLoadExistingInstanceAtPath(
                    t,
                    prependType,
                    appendType,
                    cleanRelativeFilePath,
                    out instance
                );
            }
        }

        public static bool TryLoadExistingInstanceAtPath(
            Type t,
            bool prependType,
            bool appendType,
            string cleanRelativeFilePath,
            out ScriptableObject instance)
        {
            using (_PRF_TryLoadExistingInstance.Auto())
            {
#if UNITY_EDITOR
                var cleanFileName = AppaPath.GetFileNameWithoutExtension(cleanRelativeFilePath);

                var typeName = t.Name;

                if (prependType)
                {
                    cleanFileName = $"{typeName}_{cleanFileName}";
                }

                if (appendType)
                {
                    cleanFileName = $"{cleanFileName}_{typeName}";
                }

                var any = AssetDatabaseManager.FindAssets($"t:{typeName} {cleanFileName}");

                for (var i = 0; i < any.Length; i++)
                {
                    var path = AssetDatabaseManager.GUIDToAssetPath(any[i]);
                    var existingName = AppaPath.GetFileNameWithoutExtension(path);

                    if ((existingName != null) &&
                        string.Equals(cleanFileName.ToLower(), existingName.ToLower()))
                    {
                        {
                            instance = AssetDatabaseManager.LoadAssetAtPath(path, t) as ScriptableObject;
                            return true;
                        }
                    }
                }
#endif
                instance = null;
                return false;
            }
        }

        private static string GetCleanRelativeFilePath(string name)
        {
            var cleanRelativeFilePath = name;

            var extension = AppaPath.GetExtension(name);

            if ((extension == null) || (extension == "."))
            {
                cleanRelativeFilePath += ".asset";
            }

            cleanRelativeFilePath = cleanRelativeFilePath.Replace("..", ".").Trim('.').Trim();
            return cleanRelativeFilePath;
        }

        #region Profiling

        private const string _PRF_PFX = nameof(AppalachiaObjectFactory) + ".";

        private static readonly ProfilerMarker _PRF_TryLoadExistingInstance =
            new ProfilerMarker(_PRF_PFX + nameof(TryLoadExistingInstanceAtPath));

        private static readonly ProfilerMarker _PRF_SaveInstanceToAsset =
            new(_PRF_PFX + nameof(SaveInstanceToAsset));

        private static readonly ProfilerMarker _PRF_LoadOrCreateNew =
            new(_PRF_PFX + nameof(LoadExistingOrCreateNewAsset));

        private static readonly ProfilerMarker _PRF_CreateNewAsset =
            new ProfilerMarker(_PRF_PFX + nameof(CreateNewAsset));

#if UNITY_EDITOR
        private static readonly ProfilerMarker _PRF_RenameAsset = new(_PRF_PFX + nameof(RenameAsset));
#endif

        #endregion
    }
}
