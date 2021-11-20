#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Appalachia.CI.Integration.Extensions;
using Appalachia.CI.Integration.FileSystem;
using Appalachia.Utility.Logging;
using Appalachia.Utility.Reflection.Extensions;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Playables;
using UnityEngine.TextCore.Text;
using UnityEngine.U2D;
using Object = UnityEngine.Object;

namespace Appalachia.CI.Integration.Assets
{
    public static partial class AssetDatabaseManager
    {
        #region Profiling And Tracing Markers

        private static readonly ProfilerMarker _PRF_GetAllRuntimeMonoScripts =
            new(_PRF_PFX + nameof(GetAllRuntimeMonoScripts));

        private static readonly ProfilerMarker _PRF_DoesFileExist = new(_PRF_PFX + nameof(DoesFileExist));

        private static readonly ProfilerMarker _PRF_GetSaveLocationForOwnedAsset =
            new(_PRF_PFX + nameof(GetSaveDirectoryForOwnedAsset));

        private static readonly ProfilerMarker _PRF_GetSaveLocationForAsset =
            new(_PRF_PFX + nameof(GetSaveDirectoryForAsset));

        private static readonly ProfilerMarker _PRF_GetSaveLocationForScriptableObject =
            new(_PRF_PFX + nameof(GetSaveDirectoryForScriptableObject));

        private static readonly ProfilerMarker _PRF_GetSaveLocationMetadataInternal =
            new(_PRF_PFX + nameof(GetSaveLocationMetadataInternal));

        private static readonly ProfilerMarker _PRF_RegisterAdditionalAssetTypeFolders =
            new(_PRF_PFX + nameof(RegisterAdditionalAssetTypeFolders));

        private static readonly ProfilerMarker _PRF_PopulateAssetTypeFolderLookup =
            new(_PRF_PFX + nameof(PopulateAssetTypeFolderLookup));

        private static readonly ProfilerMarker _PRF_GetAssetFolderByType =
            new(_PRF_PFX + nameof(GetAssetFolderByType));

        private static readonly ProfilerMarker _PRF_InitializeTypeScriptLookups =
            new(_PRF_PFX + nameof(InitializeTypeScriptLookups));

        private static readonly ProfilerMarker _PRF_GetTypeFromScript =
            new(_PRF_PFX + nameof(GetTypeFromScript));

        private static readonly ProfilerMarker _PRF_GetScriptFromType =
            new(_PRF_PFX + nameof(GetScriptFromType));

        private static Dictionary<UnityEditor.MonoScript, Type> _scriptTypeLookup;
        private static Dictionary<string, string> _assetRepoLookup;
        private static Dictionary<Type, Func<Type, string, string>> _assetTypeFolderLookup;
        private static Dictionary<Type, UnityEditor.MonoScript> _typeScriptLookup;
        private static List<UnityEditor.MonoScript> _allMonoScripts;
        private static List<UnityEditor.MonoScript> _runtimeMonoScripts;

        private static readonly ProfilerMarker _PRF_GetAssetRepositoryPath =
            new ProfilerMarker(_PRF_PFX + nameof(GetAssetRepositoryPath));

        #endregion

        public static bool DoesFileExist(string path)
        {
            using (_PRF_DoesFileExist.Auto())
            {
                var info = new AppaFileInfo(path);

                return info.Exists;
            }
        }

        public static List<UnityEditor.MonoScript> GetAllMonoScripts()
        {
            using (_PRF_GetAllMonoScripts.Auto())
            {
                if ((_allMonoScripts == null) || (_allMonoScripts.Count == 0))
                {
                    _allMonoScripts = new List<UnityEditor.MonoScript>();

                    var monoScriptPaths = FindAssetPathsByExtension(".cs");

                    foreach (var monoscriptPath in monoScriptPaths)
                    {
                        var importer = UnityEditor.AssetImporter.GetAtPath(monoscriptPath);

                        if (importer is UnityEditor.MonoImporter mi)
                        {
                            var script = mi.GetScript();

                            _allMonoScripts.Add(script);
                        }
                    }
                }

                return _allMonoScripts;
            }
        }

        public static List<UnityEditor.MonoScript> GetAllRuntimeMonoScripts()
        {
            using (_PRF_GetAllRuntimeMonoScripts.Auto())
            {
                if ((_runtimeMonoScripts == null) || (_runtimeMonoScripts.Count == 0))
                {
                    _runtimeMonoScripts = UnityEditor.MonoImporter.GetAllRuntimeMonoScripts().ToList();
                }

                return _runtimeMonoScripts;
            }
        }

        public static string GetAssetRepositoryPath(string assetPath, bool invalidateCache = true)
        {
            using (_PRF_GetAssetRepositoryPath.Auto())
            {
                if (string.IsNullOrWhiteSpace(assetPath))
                {
                    return null;
                }

                if (invalidateCache || (_assetRepoLookup == null))
                {
                    _assetRepoLookup = new Dictionary<string, string>();
                }

                if (_assetRepoLookup.ContainsKey(assetPath))
                {
                    return _assetRepoLookup[assetPath];
                }

                var directoryInfo = new AppaDirectoryInfo(assetPath);

                try
                {
                    while (!directoryInfo.IsInRepositoryRoot())
                    {
                        directoryInfo = directoryInfo.Parent;

                        if (directoryInfo == null)
                        {
                            return null;
                        }
                    }
                }
                catch
                {
                    AppaLog.Error(assetPath);
                }

                var rootDirectory = directoryInfo.Parent;

                _assetRepoLookup.Add(assetPath, rootDirectory.FullPath);

                return rootDirectory.FullPath;
            }
        }

        public static string GetSaveDirectoryForAsset(Type assetType, string assetPath)
        {
            using (_PRF_GetSaveLocationForAsset.Auto())
            {
                var assetName = AppaPath.GetFileName(assetPath);

                return GetSaveLocationMetadataInternal(assetPath, assetName, assetType);
            }
        }

        public static string GetSaveDirectoryForOwnedAsset<TOwner, TAsset>(string fileName)
            where TOwner : MonoBehaviour
            where TAsset : Object
        {
            using (_PRF_GetSaveLocationForOwnedAsset.Auto())
            {
                var ownerType = typeof(TOwner);
                var assetType = typeof(TAsset);

                var ownerScript = GetScriptFromType(ownerType);
                var ownerPath = GetAssetPath(ownerScript);

                return GetSaveLocationMetadataInternal(ownerPath, fileName, assetType);
            }
        }

        public static string GetSaveDirectoryForScriptableObject<T>()
            where T : ScriptableObject
        {
            using (_PRF_GetSaveLocationForScriptableObject.Auto())
            {
                var scriptType = typeof(T);

                return GetSaveDirectoryForScriptableObject(scriptType);
            }
        }

        public static string GetSaveDirectoryForScriptableObject(Type scriptType)
        {
            using (_PRF_GetSaveLocationForScriptableObject.Auto())
            {
                var script = GetScriptFromType(scriptType);
                var scriptPath = GetAssetPath(script);

                return GetSaveLocationMetadataInternal(scriptPath, null, scriptType);
            }
        }

        public static UnityEditor.MonoScript GetScriptFromType(Type t)
        {
            using (_PRF_GetScriptFromType.Auto())
            {
                InitializeTypeScriptLookups();

                if (!_typeScriptLookup.ContainsKey(t))
                {
                    return null;
                }

                return _typeScriptLookup[t];
            }
        }

        public static Type GetTypeFromScript(UnityEditor.MonoScript t)
        {
            using (_PRF_GetTypeFromScript.Auto())
            {
                InitializeTypeScriptLookups();

                if (!_scriptTypeLookup.ContainsKey(t))
                {
                    return null;
                }

                return _scriptTypeLookup[t];
            }
        }

        public static void OpenFolderInExplorer(AppaDirectoryInfo directoryInfo)
        {
            if (directoryInfo.Exists)
            {
                var startInfo =
                    new ProcessStartInfo {Arguments = directoryInfo.FullPath, FileName = "explorer.exe"};

                Process.Start(startInfo);
            }
        }

        public static void OpenFolderInExplorer(AppaFileInfo fileInfo)
        {
            if (AppaDirectory.Exists(fileInfo.ParentDirectoryFullPath))
            {
                var startInfo = new ProcessStartInfo
                {
                    Arguments = fileInfo.ParentDirectoryFullPath, FileName = "explorer.exe"
                };

                Process.Start(startInfo);
            }
        }

        public static void OpenFolderInExplorer(string folderPath)
        {
            if (AppaDirectory.Exists(folderPath))
            {
                var startInfo = new ProcessStartInfo {Arguments = folderPath, FileName = "explorer.exe"};

                Process.Start(startInfo);
            }
        }

        public static void RegisterAdditionalAssetTypeFolders<T>(
            Func<Type, string, string> folderFunction)
        {
            RegisterAdditionalAssetTypeFolders(typeof(T), folderFunction);
        }

        public static void RegisterAdditionalAssetTypeFolders(
            Type type,
            Func<Type, string, string> folderFunction)
        {
            using (_PRF_RegisterAdditionalAssetTypeFolders.Auto())
            {
                if (_assetTypeFolderLookup == null)
                {
                    PopulateAssetTypeFolderLookup();
                }

                if (_assetTypeFolderLookup.ContainsKey(type))
                {
                    _assetTypeFolderLookup[type] = folderFunction;
                }
                else
                {
                    _assetTypeFolderLookup.Add(type, folderFunction);
                }
            }
        }

        private static string GetAssetFolderByType(Type t, string fileName)
        {
            using (_PRF_GetAssetFolderByType.Auto())
            {
                string extension = null;

                if (fileName != null)
                {
                    extension = AppaPath.GetExtension(fileName.Trim('.')).ToLowerInvariant();
                }

                if (_assetTypeFolderLookup == null)
                {
                    PopulateAssetTypeFolderLookup();
                }

                foreach (var assetType in _assetTypeFolderLookup.Keys)
                {
                    if (assetType.IsAssignableFrom(t))
                    {
                        var typeFunction = _assetTypeFolderLookup[assetType];

                        return typeFunction(assetType, extension);
                    }
                }

                return "Other";
            }
        }

        private static string GetSaveLocationMetadataInternal(
            string relativePathToRepositoryMember,
            string saveFileName,
            Type saveFiletype)
        {
            using (_PRF_GetSaveLocationMetadataInternal.Auto())
            {
                var assetBasePath = ProjectLocations.GetAssetsDirectoryPath();

                string baseDataFolder;

                var repositoryPath = GetAssetRepositoryPath(relativePathToRepositoryMember);

                if (repositoryPath == null)
                {
                    baseDataFolder = AppaPath.Combine(assetBasePath, "Appalachia", "asset");
                }
                else
                {
                    baseDataFolder = AppaPath.Combine(repositoryPath, "asset");
                }

                string finalFolderName;

                var assetFolderName = GetAssetFolderByType(saveFiletype, saveFileName);
                var typeFolderName = saveFiletype.GetSimpleReadableName();

                if (assetFolderName != "Other")
                {
                    finalFolderName = assetFolderName;
                }
                else
                {
                    finalFolderName = typeFolderName;
                }

                var finalFolder = AppaPath.Combine(baseDataFolder, finalFolderName);

                return finalFolder.ToRelativePath();
            }
        }

        private static void InitializeTypeScriptLookups()
        {
            using (_PRF_InitializeTypeScriptLookups.Auto())
            {
                var initialize = false;
                if (_typeScriptLookup == null)
                {
                    _typeScriptLookup = new Dictionary<Type, UnityEditor.MonoScript>();
                    initialize = true;
                }

                if (_scriptTypeLookup == null)
                {
                    _scriptTypeLookup = new Dictionary<UnityEditor.MonoScript, Type>();
                    initialize = true;
                }

                if (initialize)
                {
                    var scripts = GetAllMonoScripts();

                    for (var index = 0; index < scripts.Count; index++)
                    {
                        var script = scripts[index];

                        var scriptType = script.GetClass();

                        _scriptTypeLookup.Add(script, scriptType);

                        if (scriptType == null)
                        {
                            continue;
                        }

                        if (_typeScriptLookup.ContainsKey(scriptType))
                        {
                            _typeScriptLookup[scriptType] = script;
                        }
                        else
                        {
                            _typeScriptLookup.Add(scriptType, script);
                        }
                    }
                }
            }
        }

        private static void PopulateAssetTypeFolderLookup()
        {
            using (_PRF_PopulateAssetTypeFolderLookup.Auto())
            {
                _assetTypeFolderLookup = new Dictionary<Type, Func<Type, string, string>>();

                var atfl = _assetTypeFolderLookup;

                atfl.Add(typeof(AnimationClip),              (_, _) => "Animations");
                atfl.Add(typeof(AnimatorOverrideController), (_, _) => "Animations");
                atfl.Add(typeof(UnityEditor.Animations.BlendTree),      (_, _) => "Animations");

                atfl.Add(typeof(AudioClip),  (_, _) => "Audio");
                atfl.Add(typeof(AudioMixer), (_, _) => "Audio");

                atfl.Add(typeof(ComputeShader), (_, _) => "ComputeShaders");

                atfl.Add(typeof(Cubemap), (_, _) => "Cubemaps");

                atfl.Add(typeof(FontAsset), (_, _) => "Fonts");

                atfl.Add(typeof(GUISkin), (_, _) => "GUISkins");

                atfl.Add(typeof(GameObject), (_, s) => s != "prefab" ? "Models" : "Prefabs");

                atfl.Add(typeof(Material), (_, _) => "Materials");

                atfl.Add(typeof(Mesh), (_, _) => "Meshes");

                atfl.Add(typeof(PhysicMaterial), (_, _) => "PhysicsMaterials");

                atfl.Add(typeof(PlayableAsset), (_, _) => "Playables");

                atfl.Add(typeof(UnityEditor.Presets.Preset), (_, _) => "Presets");

                atfl.Add(typeof(UnityEditor.SceneAsset), (_, _) => "Scenes");

                atfl.Add(typeof(Shader),                  (_, _) => "Shaders");
                atfl.Add(typeof(ShaderVariantCollection), (_, _) => "ShaderVariantCollections");

                atfl.Add(typeof(UnityEditor.ShaderInclude), (_, _) => "CGIncludes");

                atfl.Add(typeof(TerrainData),  (_, _) => "Terrains");
                atfl.Add(typeof(TerrainLayer), (_, _) => "Terrains");

                atfl.Add(typeof(Texture), (_, _) => "Textures");

                atfl.Add(typeof(Sprite),                       (_, _) => "Sprites");
                atfl.Add(typeof(SpriteAsset),                  (_, _) => "Sprites");
                atfl.Add(typeof(SpriteAtlas),                  (_, _) => "Sprites");
                atfl.Add(typeof(UnityEditor.U2D.SpriteAtlasAsset), (_, _) => "Sprites");
            }
        }
    }
}

#endif