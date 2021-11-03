using System;
using Appalachia.CI.Integration;
using Appalachia.CI.Integration.Assets;
using Appalachia.CI.Integration.Core;
using Appalachia.CI.Integration.FileSystem;
using Unity.Profiling;
using UnityEngine;

namespace Appalachia.Core.Scriptables
{
    public static class AppalachiaObjectFactory
    {
        #region Profiling And Tracing Markers

        private const string _PRF_PFX = nameof(AppalachiaObjectFactory) + ".";
        private static readonly ProfilerMarker _PRF_CreateNew = new(_PRF_PFX + nameof(CreateNew));

        private static readonly ProfilerMarker _PRF_LoadOrCreateNew = new(_PRF_PFX + nameof(LoadOrCreateNew));

        private static readonly ProfilerMarker _PRF_Rename = new(_PRF_PFX + nameof(Rename));

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
                var ext = AppaPath.GetExtension(name);

                if (string.IsNullOrWhiteSpace(ext))
                {
                    name += ".asset";
                }

                if (dataFolder == null)
                {
                    dataFolder = AssetDatabaseManager.GetSaveLocationForScriptableObject<T>().relativePath;
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

                var instance = ScriptableObject.CreateInstance(typeof(T)) as T;

                return CreateNew(name, instance, dataFolder);
            }
        }

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
    }
}
