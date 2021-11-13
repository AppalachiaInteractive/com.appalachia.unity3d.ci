using System;
using Appalachia.CI.Integration.Assets;
using Appalachia.CI.Integration.Extensions;
using Appalachia.CI.Integration.FileSystem;
using Unity.Profiling;
using UnityEngine;

namespace Appalachia.CI.Integration.Core
{
    public static class AppalachiaObjectFactory
    {
        #region Profiling And Tracing Markers

        private const string _PRF_PFX = nameof(AppalachiaObjectFactory) + ".";
        private static readonly ProfilerMarker _PRF_CreateNew = new(_PRF_PFX + nameof(CreateNew));

        private static readonly ProfilerMarker _PRF_LoadOrCreateNew = new(_PRF_PFX + nameof(LoadOrCreateNew));
#if UNITY_EDITOR
        private static readonly ProfilerMarker _PRF_Rename = new(_PRF_PFX + nameof(Rename));
#endif
        #endregion

        public static T CreateNew<T>(string dataFolder = null)
            where T : ScriptableObject
        {
            using (_PRF_CreateNew.Auto())
            {
                return LoadOrCreateNew<T>(
                    $"{typeof(T).Name}_{DateTime.Now:yyyyMMdd-hhmmssfff}.asset",
                    false,
                    false,
                    dataFolder
                );
            }
        }

        public static T CreateNew<T>(string name, T i, string dataFolder = null)
            where T : ScriptableObject
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
                    dataFolder = AssetDatabaseManager.GetSaveLocationForScriptableObject<T>()
                                                     .ToRelativePath();
                }

                var assetPath = AppaPath.Combine(dataFolder, name);

                if (AppaFile.Exists(assetPath))
                {
                    throw new AccessViolationException(assetPath);
                }

                assetPath = assetPath.Replace(ProjectLocations.GetAssetsDirectoryPath(), "Assets");

                AssetDatabaseManager.CreateAsset(i, assetPath);
                
                if (i is IAppalachiaObject<T> ao)
                {
                    ao.OnCreate();
                }
#endif
                return i;
            }
        }

        public static T LoadOrCreateNew<T>(string name, string dataFolder = null)
            where T : ScriptableObject
        {
            using (_PRF_LoadOrCreateNew.Auto())
            {
                return LoadOrCreateNew<T>(name, false, false, dataFolder);
            }
        }

        public static T LoadOrCreateNew<T>(
            string name,
            bool prependType,
            bool appendType,
            string dataFolder = null)
            where T : ScriptableObject
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

                var t = typeof(T).Name;

                if (prependType)
                {
                    cleanFileName = $"{t}_{cleanFileName}";
                }

                if (appendType)
                {
                    cleanFileName = $"{cleanFileName}_{t}";
                }

                name = $"{cleanFileName}{extension}";
                
#if UNITY_EDITOR
                var any = AssetDatabaseManager.FindAssets($"t: {t} {cleanFileName}");

                for (var i = 0; i < any.Length; i++)
                {
                    var path = AssetDatabaseManager.GUIDToAssetPath(any[i]);
                    var existingName = AppaPath.GetFileNameWithoutExtension(path);

                    if ((existingName != null) &&
                        string.Equals(cleanFileName.ToLower(), existingName.ToLower()))
                    {
                        return AssetDatabaseManager.LoadAssetAtPath<T>(path);
                    }
                }
#endif
                var instance = ScriptableObject.CreateInstance(typeof(T)) as T;

                return CreateNew(name, instance, dataFolder);
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
    }
}