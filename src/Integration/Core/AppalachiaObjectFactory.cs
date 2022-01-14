using System;
using System.Data;
using System.Linq;
using Appalachia.CI.Constants;
using Appalachia.CI.Integration.Assets;
using Appalachia.CI.Integration.Extensions;
using Appalachia.CI.Integration.FileSystem;
using Appalachia.Utility.Extensions.Debugging;
using Appalachia.Utility.Strings;
using Unity.Profiling;
using UnityEngine;

namespace Appalachia.CI.Integration.Core
{
    public static class AppalachiaObjectFactory
    {
        #region Static Fields and Autoproperties

        [NonSerialized] private static AppaContext _context;

        #endregion

        private static AppaContext Context
        {
            get
            {
                if (_context == null)
                {
                    _context = new AppaContext(typeof(AppalachiaObjectFactory));
                }

                return _context;
            }
        }

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
                    ZString.Format("{0}_{1:yyyyMMdd-hhmmssfff}.asset", t.Name, DateTime.Now),
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
                if (TryLoadExistingInstance(
                        t,
                        name,
                        prependType,
                        appendType,
                        out var scriptableObject,
                        out var searchString,
                        out var searchResults
                    ))
                {
                    return scriptableObject;
                }

                Context.Log.Debug(
                    ZString.Format(
                        "Could not load instance of type [{0}] with name [{1}]. Searched [{2}].  Had results [{3}].  Will create.",
                        t.Name,
                        name,
                        searchString,
                        string.Join(", ", searchResults)
                    )
                );

                if (t.IsAbstract)
                {
                    Context.Log.Error(
                        ZString.Format(
                            "Can not create ScriptableObject of type [{0}] as it is abstract.",
                            t.Name
                        )
                    );
                    return null;
                }

                var instance = ScriptableObject.CreateInstance(t);

                if (instance == null)
                {
                    Context.Log.Error(
                        ZString.Format("Could not create ScriptableObject of type [{0}]", t.Name)
                    );
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

                var cleanName = AppaPath.GetFileNameWithoutExtension(newName);

                instance.name = cleanName;

                AssetDatabaseManager.RenameAsset(path, cleanName);
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
                            ZString.Format("A scriptable object already exists at {0}", assetPath)
                        );
                    }
                }

                assetPath = assetPath.Replace(ProjectLocations.GetAssetsDirectoryPath(), "Assets");

                AssetDatabaseManager.CreateAsset(i, assetPath);

                i = AssetDatabaseManager.ImportAndLoadAssetAtPath(t, assetPath) as ScriptableObject;

                if (i == null)
                {
                    i = AssetDatabaseManager.ImportAndLoadAssetAtPath(t, assetPath) as ScriptableObject;
                }
#endif
                return i;
            }
        }

        public static bool TryLoadExistingInstance<T>(
            string name,
            out T instance,
            out string searchString,
            out string[] searchResults)
            where T : ScriptableObject
        {
            using (_PRF_TryLoadExistingInstance.Auto())
            {
                var result = TryLoadExistingInstance(
                    typeof(T),
                    name,
                    out var scriptableObject,
                    out searchString,
                    out searchResults
                );

                instance = scriptableObject as T;
                return result;
            }
        }

        public static bool TryLoadExistingInstance(
            Type t,
            string name,
            out ScriptableObject instance,
            out string searchString,
            out string[] searchResults)
        {
            using (_PRF_TryLoadExistingInstance.Auto())
            {
                return TryLoadExistingInstance(
                    t,
                    name,
                    false,
                    false,
                    out instance,
                    out searchString,
                    out searchResults
                );
            }
        }

        public static bool TryLoadExistingInstance(
            Type t,
            string name,
            bool prependType,
            bool appendType,
            out ScriptableObject instance,
            out string searchString,
            out string[] searchResults)
        {
            using (_PRF_TryLoadExistingInstance.Auto())
            {
                var cleanRelativeFilePath = GetCleanRelativeFilePath(name);

                return TryLoadExistingInstanceAtPath(
                    t,
                    prependType,
                    appendType,
                    cleanRelativeFilePath,
                    out instance,
                    out searchString,
                    out searchResults
                );
            }
        }

        public static bool TryLoadExistingInstanceAtPath(
            Type t,
            bool prependType,
            bool appendType,
            string cleanRelativeFilePath,
            out ScriptableObject instance,
            out string searchString,
            out string[] searchResults)
        {
            using (_PRF_TryLoadExistingInstance.Auto())
            {
#if UNITY_EDITOR
                var cleanFileName = AppaPath.GetFileNameWithoutExtension(cleanRelativeFilePath);

                var typeName = t.Name;

                if (prependType)
                {
                    cleanFileName = ZString.Format("{0}_{1}", typeName, cleanFileName);
                }

                if (appendType)
                {
                    cleanFileName = ZString.Format("{0}_{1}", cleanFileName, typeName);
                }

                try
                {
                    var newNameStyle = AppalachiaNameConfig.FromLegacyOptions(t, prependType, appendType);

                    newNameStyle.Finish(cleanRelativeFilePath);
                    var newCleanFileName = newNameStyle.NameWithoutExtension;

                    APPADEBUG.BREAKPOINT(
                        () => ZString.Format(
                            "Name issue! OLD:[{0}] vs. NEW:[{1}]",
                            cleanFileName,
                            newCleanFileName
                        ),
                        null,
                        () => cleanFileName != newCleanFileName
                    );
                }
                catch
                {
                    APPADEBUG.BREAKPOINT(() => "Exception generating new asset name", null);
                }

                searchString = SearchStringBuilder.Build.AddType(t).AddTerm(cleanFileName).Finish();

                var any = AssetDatabaseManager.FindAssets(searchString);
                searchResults = any.Select(AssetDatabaseManager.GUIDToAssetPath)
                                   .Select(p => p.fileNameWithoutExtension)
                                   .ToArray();

                for (var i = 0; i < any.Length; i++)
                {
                    var path = AssetDatabaseManager.GUIDToAssetPath(any[i]);
                    var existingName = path.fileNameWithoutExtension;

                    if ((existingName != null) &&
                        string.Equals(cleanFileName.ToLower(), existingName.ToLower()))
                    {
                        instance = AssetDatabaseManager.LoadAssetAtPath(path, t) as ScriptableObject;
                        return true;
                    }
                }

#else
                searchString = null;
                searchResults = null;
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
