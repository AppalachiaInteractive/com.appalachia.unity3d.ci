using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Appalachia.CI.Integration.FileSystem;
using Appalachia.CI.Integration.Paths;
using Appalachia.Utility.Reflection.Extensions;
using Unity.Profiling;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.Presets;
using UnityEditor.U2D;
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
            new(_PRF_PFX + nameof(GetSaveLocationForOwnedAsset));

        private static readonly ProfilerMarker _PRF_GetSaveLocationForAsset =
            new(_PRF_PFX + nameof(GetSaveLocationForAsset));

        private static readonly ProfilerMarker _PRF_GetSaveLocationForScriptableObject =
            new(_PRF_PFX + nameof(GetSaveLocationForScriptableObject));

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

        #endregion

        private static Dictionary<MonoScript, Type> _scriptTypeLookup;
        private static Dictionary<Type, Func<Type, string, string>> _assetTypeFolderLookup;
        private static Dictionary<Type, MonoScript> _typeScriptLookup;
        private static List<MonoScript> _allMonoScripts;
        private static List<MonoScript> _runtimeMonoScripts;

        public static bool DoesFileExist(string path)
        {
            using (_PRF_DoesFileExist.Auto())
            {
                var info = new AppaFileInfo(path);

                return info.Exists;
            }
        }

        public static List<MonoScript> GetAllMonoScripts()
        {
            using (_PRF_GetAllMonoScripts.Auto())
            {
                if ((_allMonoScripts == null) || (_allMonoScripts.Count == 0))
                {
                    _allMonoScripts = new List<MonoScript>();

                    var monoScriptPaths = FindAssetPathsByExtension(".cs");

                    foreach (var monoscriptPath in monoScriptPaths)
                    {
                        var importer = AssetImporter.GetAtPath(monoscriptPath);

                        if (importer is MonoImporter mi)
                        {
                            var script = mi.GetScript();

                            _allMonoScripts.Add(script);
                        }
                    }
                }

                return _allMonoScripts;
            }
        }

        public static List<MonoScript> GetAllRuntimeMonoScripts()
        {
            using (_PRF_GetAllRuntimeMonoScripts.Auto())
            {
                if ((_runtimeMonoScripts == null) || (_runtimeMonoScripts.Count == 0))
                {
                    _runtimeMonoScripts = MonoImporter.GetAllRuntimeMonoScripts().ToList();
                }

                return _runtimeMonoScripts;
            }
        }

        public static AssetPath GetSaveLocationForAsset(Type assetType, string assetPath)
        {
            using (_PRF_GetSaveLocationForAsset.Auto())
            {
                var assetName = AppaPath.GetFileName(assetPath);

                return GetSaveLocationMetadataInternal(assetPath, assetName, assetType);
            }
        }

        public static AssetPath GetSaveLocationForOwnedAsset<TOwner, TAsset>(string fileName)
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

        public static AssetPath GetSaveLocationForScriptableObject<T>()
            where T : ScriptableObject
        {
            using (_PRF_GetSaveLocationForScriptableObject.Auto())
            {
                var scriptType = typeof(T);

                return GetSaveLocationForScriptableObject(scriptType);
            }
        }

        public static AssetPath GetSaveLocationForScriptableObject(Type scriptType)
        {
            using (_PRF_GetSaveLocationForScriptableObject.Auto())
            {
                var script = GetScriptFromType(scriptType);
                var scriptPath = GetAssetPath(script);

                return GetSaveLocationMetadataInternal(scriptPath, null, scriptType);
            }
        }

        public static MonoScript GetScriptFromType(Type t)
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

        public static Type GetTypeFromScript(MonoScript t)
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

        public static void OpenFolderInExplorer(string folderPath)
        {
            if (AppaDirectory.Exists(folderPath))
            {
                var startInfo = new ProcessStartInfo {Arguments = folderPath, FileName = "explorer.exe"};

                Process.Start(startInfo);
            }
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

                _assetTypeFolderLookup.Add(type, folderFunction);
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

        private static AssetPath GetSaveLocationMetadataInternal(
            string relativePathToRepositoryMember,
            string saveFileName,
            Type saveFiletype)
        {
            using (_PRF_GetSaveLocationMetadataInternal.Auto())
            {
                var assetBasePath = ProjectLocations.GetAssetsDirectoryPath();

                string baseDataFolder;

                var repository = ProjectLocations.GetAssetRepository(relativePathToRepositoryMember);

                AssetPathType pathType;

                if (repository == null)
                {
                    baseDataFolder = AppaPath.Combine(assetBasePath, "Appalachia", "Data");
                    pathType = AssetPathType.ProjectDataFolder;
                }
                else
                {
                    baseDataFolder = repository.DataDirectory.FullPath;
                    pathType = AssetPathType.RepositoryDataFolder;
                }

                var isLibrary = baseDataFolder.StartsWith("Library");
                var isPackage = baseDataFolder.StartsWith("Packages");

                if (isLibrary)
                {
                    pathType = AssetPathType.LibraryResource;
                }
                else if (isPackage)
                {
                    pathType = AssetPathType.PackageResource;
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

                var output = new AssetPath(finalFolder, true);

                output.pathType = pathType;
                return output;
            }
        }

        private static void InitializeTypeScriptLookups()
        {
            using (_PRF_InitializeTypeScriptLookups.Auto())
            {
                var initialize = false;
                if (_typeScriptLookup == null)
                {
                    _typeScriptLookup = new Dictionary<Type, MonoScript>();
                    initialize = true;
                }

                if (_scriptTypeLookup == null)
                {
                    _scriptTypeLookup = new Dictionary<MonoScript, Type>();
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
                atfl.Add(typeof(BlendTree),                  (_, _) => "Animations");

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

                atfl.Add(typeof(Preset), (_, _) => "Presets");

                atfl.Add(typeof(SceneAsset), (_, _) => "Scenes");

                atfl.Add(typeof(Shader),                  (_, _) => "Shaders");
                atfl.Add(typeof(ShaderVariantCollection), (_, _) => "ShaderVariantCollections");

                atfl.Add(typeof(ShaderInclude), (_, _) => "CGIncludes");

                atfl.Add(typeof(TerrainData),  (_, _) => "Terrains");
                atfl.Add(typeof(TerrainLayer), (_, _) => "Terrains");

                atfl.Add(typeof(Texture), (_, _) => "Textures");

                atfl.Add(typeof(Sprite),           (_, _) => "Sprites");
                atfl.Add(typeof(SpriteAsset),      (_, _) => "Sprites");
                atfl.Add(typeof(SpriteAtlas),      (_, _) => "Sprites");
                atfl.Add(typeof(SpriteAtlasAsset), (_, _) => "Sprites");
            }
        }
    }
}
