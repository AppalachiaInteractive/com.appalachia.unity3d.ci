#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Appalachia.CI.Integration.Extensions;
using Appalachia.CI.Integration.FileSystem;
using Appalachia.Utility.Constants;
using Appalachia.Utility.Extensions;
using Appalachia.Utility.Strings;
using Unity.Profiling;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Appalachia.CI.Integration.Assets
{
    [UnityEditor.InitializeOnLoad]
    public static partial class AssetDatabaseManager
    {
        public static event UnityEditor.AssetDatabase.ImportPackageCallback importPackageCancelled
        {
            add => UnityEditor.AssetDatabase.importPackageCancelled += value;
            remove => UnityEditor.AssetDatabase.importPackageCancelled -= value;
        }

        public static event UnityEditor.AssetDatabase.ImportPackageCallback importPackageCompleted
        {
            add => UnityEditor.AssetDatabase.importPackageCompleted += value;
            remove => UnityEditor.AssetDatabase.importPackageCompleted -= value;
        }

        public static event UnityEditor.AssetDatabase.ImportPackageFailedCallback importPackageFailed
        {
            add => UnityEditor.AssetDatabase.importPackageFailed += value;
            remove => UnityEditor.AssetDatabase.importPackageFailed -= value;
        }

        public static event UnityEditor.AssetDatabase.ImportPackageCallback importPackageStarted
        {
            add => UnityEditor.AssetDatabase.importPackageStarted += value;
            remove => UnityEditor.AssetDatabase.importPackageStarted -= value;
        }

        static AssetDatabaseManager()
        {
        }

        /// <summary>
        ///     <para>Changes during Refresh if anything has changed that can invalidate any artifact.</para>
        /// </summary>
        public static uint GlobalArtifactDependencyVersion =>
            UnityEditor.AssetDatabase.GlobalArtifactDependencyVersion;

        /// <summary>
        ///     <para>Changes whenever a new artifact is added to the artifact database.</para>
        /// </summary>
        public static uint GlobalArtifactProcessedVersion =>
            UnityEditor.AssetDatabase.GlobalArtifactProcessedVersion;

        /// <summary>
        ///     <para>Callback raised whenever a package import successfully completes that lists the items selected to be imported.</para>
        /// </summary>
        public static Action<string[]> onImportPackageItemsCompleted
        {
            get => UnityEditor.AssetDatabase.onImportPackageItemsCompleted;
            set => UnityEditor.AssetDatabase.onImportPackageItemsCompleted = value;
        }

        /// <summary>
        ///     <para>The desired number of processes to use when importing assets, during an asset database refresh.</para>
        /// </summary>
        public static int DesiredWorkerCount
        {
            get => UnityEditor.AssetDatabase.DesiredWorkerCount;
            set => UnityEditor.AssetDatabase.DesiredWorkerCount = value;
        }

        /// <summary>
        ///     <para>Gets the refresh import mode currently in use by the asset database.</para>
        /// </summary>
        public static UnityEditor.AssetDatabase.RefreshImportMode ActiveRefreshImportMode
        {
            get => UnityEditor.AssetDatabase.ActiveRefreshImportMode;
            set => UnityEditor.AssetDatabase.ActiveRefreshImportMode = value;
        }

        /// <summary>
        ///     <para>Adds objectToAdd to an existing asset at path.</para>
        /// </summary>
        /// <param name="objectToAdd">Object to add to the existing asset.</param>
        /// <param name="path">Filesystem path to the asset.</param>
        public static void AddObjectToAsset(Object objectToAdd, string path)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_AddObjectToAsset.Auto())
            {
                var relativePath = path.ToRelativePath();
                UnityEditor.AssetDatabase.AddObjectToAsset(objectToAdd, relativePath);
            }
        }

        /// <summary>
        ///     <para>Adds objectToAdd to an existing asset identified by assetObject.</para>
        /// </summary>
        /// <param name="objectToAdd"></param>
        /// <param name="assetObject"></param>
        public static void AddObjectToAsset(Object objectToAdd, Object assetObject)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_AddObjectToAsset.Auto())
            {
                UnityEditor.AssetDatabase.AddObjectToAsset(objectToAdd, assetObject);
            }
        }

        /// <summary>
        ///     <para>Decrements an internal counter which Unity uses to determine whether to allow automatic AssetDatabase refreshing behavior.</para>
        /// </summary>
        public static void AllowAutoRefresh()
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_AllowAutoRefresh.Auto())
            {
                UnityEditor.AssetDatabase.AllowAutoRefresh();
            }
        }

        /// <summary>
        ///     <para>Get the GUID for the asset at path.</para>
        /// </summary>
        /// <param name="path">Filesystem path for the asset.</param>
        /// <param name="options">
        ///     Specifies whether this method should return a GUID for recently deleted assets. The default value is
        ///     AssetPathToGUIDOptions.IncludeRecentlyDeletedAssets.
        /// </param>
        /// <returns>
        ///     <para>GUID.</para>
        /// </returns>
        public static string AssetPathToGUID(string path)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_AssetPathToGUID.Auto())
            {
                var relativePath = path.ToRelativePath();
                return UnityEditor.AssetDatabase.AssetPathToGUID(relativePath);
            }
        }

        /// <summary>
        ///     <para>Get the GUID for the asset at path.</para>
        /// </summary>
        /// <param name="path">Filesystem path for the asset.</param>
        /// <param name="options">
        ///     Specifies whether this method should return a GUID for recently deleted assets. The default value is
        ///     AssetPathToGUIDOptions.IncludeRecentlyDeletedAssets.
        /// </param>
        /// <returns>
        ///     <para>GUID.</para>
        /// </returns>
        public static string AssetPathToGUID(string path, UnityEditor.AssetPathToGUIDOptions options)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_AssetPathToGUID.Auto())
            {
                var relativePath = path.ToRelativePath();
                return UnityEditor.AssetDatabase.AssetPathToGUID(relativePath, options);
            }
        }

        /// <summary>
        ///     <para>Checks the availability of the Cache Server.</para>
        /// </summary>
        /// <param name="ip">The IP address of the Cache Server.</param>
        /// <param name="port">The Port number of the Cache Server.</param>
        /// <returns>
        ///     <para>Returns true when Editor can connect to the Cache Server. Returns false otherwise.</para>
        /// </returns>
        public static bool CanConnectToCacheServer(string ip, ushort port)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_CanConnectToCacheServer.Auto())
            {
                return UnityEditor.AssetDatabase.CanConnectToCacheServer(ip, port);
            }
        }

        /// <summary>
        ///     <para>Checks if Unity can open an asset in the Editor.</para>
        /// </summary>
        /// <param name="instanceID">The instance ID of the asset.</param>
        /// <returns>
        ///     <para>Returns true if Unity can successfully open the asset in the Editor, otherwise returns false.</para>
        /// </returns>
        public static bool CanOpenAssetInEditor(int instanceID)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_CanOpenAssetInEditor.Auto())
            {
                return UnityEditor.AssetDatabase.CanOpenAssetInEditor(instanceID);
            }
        }

        /// <summary>
        ///     <para>
        ///         Query whether an Asset file can be opened for editing in version control and is not exclusively locked by another user or otherwise
        ///         unavailable.
        ///     </para>
        /// </summary>
        /// <param name="assetObject">Object representing the asset whose status you wish to query.</param>
        /// <param name="path">Path to the asset file or its .meta file on disk, relative to project folder.</param>
        /// <param name="message">Returns a reason for the asset not being available for edit.</param>
        /// <param name="statusOptions">
        ///     Options for how the version control system should be queried. These options can effect the speed and accuracy of the
        ///     query. Default is StatusQueryOptions.UseCachedIfPossible.
        /// </param>
        /// <returns>
        ///     <para>True if the asset is considered available for edit by the selected version control system.</para>
        /// </returns>
        public static bool CanOpenForEdit(Object assetObject)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_CanOpenForEdit.Auto())
            {
                return UnityEditor.AssetDatabase.CanOpenForEdit(assetObject);
            }
        }

        /// <summary>
        ///     <para>
        ///         Query whether an Asset file can be opened for editing in version control and is not exclusively locked by another user or otherwise
        ///         unavailable.
        ///     </para>
        /// </summary>
        /// <param name="assetObject">Object representing the asset whose status you wish to query.</param>
        /// <param name="path">Path to the asset file or its .meta file on disk, relative to project folder.</param>
        /// <param name="message">Returns a reason for the asset not being available for edit.</param>
        /// <param name="statusOptions">
        ///     Options for how the version control system should be queried. These options can effect the speed and accuracy of the
        ///     query. Default is StatusQueryOptions.UseCachedIfPossible.
        /// </param>
        /// <returns>
        ///     <para>True if the asset is considered available for edit by the selected version control system.</para>
        /// </returns>
        public static bool CanOpenForEdit(Object assetObject, UnityEditor.StatusQueryOptions statusOptions)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_CanOpenForEdit.Auto())
            {
                return UnityEditor.AssetDatabase.CanOpenForEdit(assetObject, statusOptions);
            }
        }

        /// <summary>
        ///     <para>
        ///         Query whether an Asset file can be opened for editing in version control and is not exclusively locked by another user or otherwise
        ///         unavailable.
        ///     </para>
        /// </summary>
        /// <param name="assetObject">Object representing the asset whose status you wish to query.</param>
        /// <param name="path">Path to the asset file or its .meta file on disk, relative to project folder.</param>
        /// <param name="message">Returns a reason for the asset not being available for edit.</param>
        /// <param name="statusOptions">
        ///     Options for how the version control system should be queried. These options can effect the speed and accuracy of the
        ///     query. Default is StatusQueryOptions.UseCachedIfPossible.
        /// </param>
        /// <returns>
        ///     <para>True if the asset is considered available for edit by the selected version control system.</para>
        /// </returns>
        public static bool CanOpenForEdit(string path)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_CanOpenForEdit.Auto())
            {
                var relativePath = path.ToRelativePath();
                return UnityEditor.AssetDatabase.CanOpenForEdit(relativePath);
            }
        }

        /// <summary>
        ///     <para>
        ///         Query whether an Asset file can be opened for editing in version control and is not exclusively locked by another user or otherwise
        ///         unavailable.
        ///     </para>
        /// </summary>
        /// <param name="assetObject">Object representing the asset whose status you wish to query.</param>
        /// <param name="path">Path to the asset file or its .meta file on disk, relative to project folder.</param>
        /// <param name="message">Returns a reason for the asset not being available for edit.</param>
        /// <param name="statusOptions">
        ///     Options for how the version control system should be queried. These options can effect the speed and accuracy of the
        ///     query. Default is StatusQueryOptions.UseCachedIfPossible.
        /// </param>
        /// <returns>
        ///     <para>True if the asset is considered available for edit by the selected version control system.</para>
        /// </returns>
        public static bool CanOpenForEdit(string path, UnityEditor.StatusQueryOptions statusOptions)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_CanOpenForEdit.Auto())
            {
                var relativePath = path.ToRelativePath();
                return UnityEditor.AssetDatabase.CanOpenForEdit(relativePath, statusOptions);
            }
        }

        public static bool CanOpenForEdit(Object assetObject, out string message)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_CanOpenForEdit.Auto())
            {
                return UnityEditor.AssetDatabase.CanOpenForEdit(assetObject, out message);
            }
        }

        public static bool CanOpenForEdit(
            Object assetObject,
            out string message,
            UnityEditor.StatusQueryOptions statusOptions)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_CanOpenForEdit.Auto())
            {
                return UnityEditor.AssetDatabase.CanOpenForEdit(assetObject, out message, statusOptions);
            }
        }

        public static bool CanOpenForEdit(string path, out string message)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_CanOpenForEdit.Auto())
            {
                var relativePath = path.ToRelativePath();
                return UnityEditor.AssetDatabase.CanOpenForEdit(relativePath, out message);
            }
        }

        public static bool CanOpenForEdit(
            string path,
            out string message,
            UnityEditor.StatusQueryOptions statusOptions)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_CanOpenForEdit.Auto())
            {
                var relativePath = path.ToRelativePath();
                return UnityEditor.AssetDatabase.CanOpenForEdit(relativePath, out message, statusOptions);
            }
        }

        public static void CanOpenForEdit(
            string[] assetOrMetaFilePaths,
            List<string> outNotEditablePaths,
            UnityEditor.StatusQueryOptions statusQueryOptions =
                UnityEditor.StatusQueryOptions.UseCachedIfPossible)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_CanOpenForEdit.Auto())
            {
                UnityEditor.AssetDatabase.CanOpenForEdit(
                    assetOrMetaFilePaths,
                    outNotEditablePaths,
                    statusQueryOptions
                );
            }
        }

        /// <summary>
        ///     <para>Clears the importer override for the asset.</para>
        /// </summary>
        /// <param name="path">Asset path.</param>
        public static void ClearImporterOverride(string path)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_ClearImporterOverride.Auto())
            {
                var relativePath = path.ToRelativePath();
                UnityEditor.AssetDatabase.ClearImporterOverride(relativePath);
            }
        }

        /// <summary>
        ///     <para>Removes all labels attached to an asset.</para>
        /// </summary>
        /// <param name="obj"></param>
        public static void ClearLabels(Object obj)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();

            using (_PRF_ClearLabels.Auto())
            {
                UnityEditor.AssetDatabase.ClearLabels(obj);
            }
        }

        /// <summary>
        ///     <para>Closes an active cache server connection. If no connection is active, then it does nothing.</para>
        /// </summary>
        public static void CloseCacheServerConnection()
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_CloseCacheServerConnection.Auto())
            {
                UnityEditor.AssetDatabase.CloseCacheServerConnection();
            }
        }

        /// <summary>
        ///     <para>Is object an asset?</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="instanceID"></param>
        public static bool Contains(Object obj)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_Contains.Auto())
            {
                return UnityEditor.AssetDatabase.Contains(obj);
            }
        }

        /// <summary>
        ///     <para>Is object an asset?</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="instanceID"></param>
        public static bool Contains(int instanceID)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_Contains.Auto())
            {
                return UnityEditor.AssetDatabase.Contains(instanceID);
            }
        }

        /// <summary>
        ///     <para>Duplicates the asset at path and stores it at newPath.</para>
        /// </summary>
        /// <param name="path">Filesystem path of the source asset.</param>
        /// <param name="newPath">Filesystem path of the new asset to create.</param>
        /// <returns>
        ///     <para>Returns true if the copy operation is successful or false if part of the process fails.</para>
        /// </returns>
        public static bool CopyAsset(string path, string newPath)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_CopyAsset.Auto())
            {
                var relativePath = path.ToRelativePath();
                var relativeNewPath = newPath.ToRelativePath();
                return UnityEditor.AssetDatabase.CopyAsset(relativePath, relativeNewPath);
            }
        }

        /// <summary>
        ///     <para>Creates a new native Unity asset.</para>
        /// </summary>
        /// <param name="asset">Object to use in creating the asset.</param>
        /// <param name="path">Filesystem path for the new asset.</param>
        public static void CreateAsset(Object asset, string path)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_CreateAsset.Auto())
            {
                var relativePath = path.ToRelativePath();
                var directory = AppaPath.GetDirectoryName(relativePath);

                CreateFolder(directory, true);

                UnityEditor.AssetDatabase.CreateAsset(asset, relativePath);
            }
        }

        /// <summary>
        ///     <para>
        ///         Creates a new folder, in the specified parent folder.
        ///         The parent folder string must start with the "Assets" folder, and all folders within the parent folder string must already exist. For
        ///         example, when specifying "AssetsParentFolder1Parentfolder2/", the new folder will be created in "ParentFolder2" only if ParentFolder1 and
        ///         ParentFolder2 already exist.
        ///     </para>
        /// </summary>
        /// <param name="parentFolder">The path to the parent folder. Must start with "Assets/".</param>
        /// <param name="newFolderName">The name of the new folder.</param>
        /// <returns>
        ///     <para>The GUID of the newly created folder, if the folder was created successfully. Otherwise returns an empty string.</para>
        /// </returns>
        public static void CreateFolder(
            string parentFolder,
            string newFolderName,
            bool createStructure = true)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_CreateFolder.Auto())
            {
                parentFolder = parentFolder.CleanFullPath();

                if (createStructure)
                {
                    var di = new AppaDirectoryInfo(parentFolder);

                    if (!di.Exists)
                    {
                        di.Create();
                        ImportAsset(parentFolder);
                    }
                }

                var completeFolder = AppaPath.Combine(parentFolder, newFolderName);

                if (AppaDirectory.Exists(completeFolder))
                {
                    return;
                }

                if (UnityEditor.AssetDatabase.IsValidFolder(completeFolder))
                {
                    return;
                }

                AppaDirectory.CreateDirectory(completeFolder);
                UnityEditor.AssetDatabase.ImportAsset(completeFolder);
            }
        }

        public static void CreateFolder(string folderPath, bool createStructure)
        {
            folderPath = folderPath.CleanFullPath();

            var splits = folderPath.Split('/');

            var lastPart = splits[splits.Length - 1];

            var basePath = folderPath.Replace(lastPart, string.Empty);

            CreateFolder(basePath, lastPart, createStructure);
        }

        /// <summary>
        ///     <para>Deletes the specified asset or folder.</para>
        /// </summary>
        /// <param name="path">Project relative path of the asset or folder to be deleted.</param>
        /// <returns>
        ///     <para>Returns true if the asset has been successfully removed, false if it doesn't exist or couldn't be removed.</para>
        /// </returns>
        public static bool DeleteAsset(string path)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_DeleteAsset.Auto())
            {
                return UnityEditor.AssetDatabase.DeleteAsset(path);
            }
        }

        public static bool DeleteAssets(string[] paths, List<string> outFailedPaths)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_DeleteAssets.Auto())
            {
                return UnityEditor.AssetDatabase.DeleteAssets(paths, outFailedPaths);
            }
        }

        /// <summary>
        ///     <para>Increments an internal counter which Unity uses to determine whether to allow automatic AssetDatabase refreshing behavior.</para>
        /// </summary>
        public static void DisallowAutoRefresh()
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_DisallowAutoRefresh.Auto())
            {
                UnityEditor.AssetDatabase.DisallowAutoRefresh();
            }
        }

        /// <summary>
        ///     <para>Exports the assets identified by assetPathNames to a unitypackage file in fileName.</para>
        /// </summary>
        /// <param name="paths"></param>
        /// <param name="fileName"></param>
        /// <param name="flags"></param>
        /// <param name="path"></param>
        public static void ExportPackage(string path, string fileName)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_ExportPackage.Auto())
            {
                var relativePath = path.ToRelativePath();
                UnityEditor.AssetDatabase.ExportPackage(relativePath, fileName);
            }
        }

        /// <summary>
        ///     <para>Exports the assets identified by assetPathNames to a unitypackage file in fileName.</para>
        /// </summary>
        /// <param name="paths"></param>
        /// <param name="fileName"></param>
        /// <param name="flags"></param>
        /// <param name="path"></param>
        public static void ExportPackage(string path, string fileName, UnityEditor.ExportPackageOptions flags)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_ExportPackage.Auto())
            {
                var relativePath = path.ToRelativePath();
                UnityEditor.AssetDatabase.ExportPackage(relativePath, fileName, flags);
            }
        }

        /// <summary>
        ///     <para>Exports the assets identified by assetPathNames to a unitypackage file in fileName.</para>
        /// </summary>
        /// <param name="paths"></param>
        /// <param name="fileName"></param>
        /// <param name="flags"></param>
        /// <param name="path"></param>
        public static void ExportPackage(string[] paths, string fileName)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_ExportPackage.Auto())
            {
                var relativePaths = paths.ToRelativePaths();
                UnityEditor.AssetDatabase.ExportPackage(relativePaths, fileName);
            }
        }

        /// <summary>
        ///     <para>Exports the assets identified by assetPathNames to a unitypackage file in fileName.</para>
        /// </summary>
        /// <param name="paths"></param>
        /// <param name="fileName"></param>
        /// <param name="flags"></param>
        /// <param name="path"></param>
        public static void ExportPackage(
            string[] paths,
            string fileName,
            UnityEditor.ExportPackageOptions flags)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_ExportPackage.Auto())
            {
                var relativePaths = paths.ToRelativePaths();
                UnityEditor.AssetDatabase.ExportPackage(relativePaths, fileName, flags);
            }
        }

        /// <summary>
        ///     <para>Creates an external Asset from an object (such as a Material) by extracting it from within an imported asset (such as an FBX file).</para>
        /// </summary>
        /// <param name="asset">The sub-asset to extract.</param>
        /// <param name="newPath">The file path of the new Asset.</param>
        /// <returns>
        ///     <para>An empty string if Unity has successfully extracted the Asset, or an error message if not.</para>
        /// </returns>
        public static string ExtractAsset(Object asset, string newPath)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_ExtractAsset.Auto())
            {
                return UnityEditor.AssetDatabase.ExtractAsset(asset, newPath);
            }
        }

        /// <summary>
        ///     <para>Search the asset database using the search filter string.</para>
        /// </summary>
        /// <param name="filter">The filter string can contain search data.  See below for details about this string.</param>
        /// <param name="searchInFolders">The folders where the search will start.</param>
        /// <returns>
        ///     <para>Array of matching asset. Note that GUIDs will be returned. If no matching assets were found, returns empty array.</para>
        /// </returns>
        public static string[] FindAssets(string filter)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_FindAssets.Auto())
            {
                return UnityEditor.AssetDatabase.FindAssets(filter);
            }
        }

        /// <summary>
        ///     <para>Search the asset database using the search filter string.</para>
        /// </summary>
        /// <param name="filter">The filter string can contain search data.  See below for details about this string.</param>
        /// <param name="searchInFolders">The folders where the search will start.</param>
        /// <returns>
        ///     <para>Array of matching asset. Note that GUIDs will be returned. If no matching assets were found, returns empty array.</para>
        /// </returns>
        public static string[] FindAssets(string filter, string[] searchInFolders)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_FindAssets.Auto())
            {
                return UnityEditor.AssetDatabase.FindAssets(filter, searchInFolders);
            }
        }

        public static void ForceReserializeAssets(
            IEnumerable<string> assetPaths,
            UnityEditor.ForceReserializeAssetsOptions options =
                UnityEditor.ForceReserializeAssetsOptions.ReserializeAssetsAndMetadata)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_ForceReserializeAssets.Auto())
            {
                UnityEditor.AssetDatabase.ForceReserializeAssets(assetPaths, options);
            }
        }

        /// <summary>
        ///     <para>Forcibly load and re-serialize the given assets, flushing any outstanding data changes to disk.</para>
        /// </summary>
        /// <param name="assetPaths">The paths to the assets that should be reserialized. If omitted, will reserialize all assets in the project.</param>
        /// <param name="options">Specify whether you want to reserialize the assets themselves, their .meta files, or both. If omitted, defaults to both.</param>
        public static void ForceReserializeAssets()
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_ForceReserializeAssets.Auto())
            {
                UnityEditor.AssetDatabase.ForceReserializeAssets();
            }
        }

        /// <summary>
        ///     <para>
        ///         Forces the Editor to use the desired amount of worker processes. Unity will either spawn new worker processes or shut down idle worker
        ///         processes to reach the desired number.
        ///     </para>
        /// </summary>
        public static void ForceToDesiredWorkerCount()
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_ForceToDesiredWorkerCount.Auto())
            {
                UnityEditor.AssetDatabase.ForceToDesiredWorkerCount();
            }
        }

        /// <summary>
        ///     <para>Creates a new unique path for an asset.</para>
        /// </summary>
        /// <param name="path"></param>
        public static string GenerateUniqueAssetPath(string path)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_GenerateUniqueAssetPath.Auto())
            {
                return UnityEditor.AssetDatabase.GenerateUniqueAssetPath(path);
            }
        }

        /// <summary>
        ///     <para>Return all the AssetBundle names in the asset database.</para>
        /// </summary>
        /// <returns>
        ///     <para>Array of asset bundle names.</para>
        /// </returns>
        public static string[] GetAllAssetBundleNames()
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_GetAllAssetBundleNames.Auto())
            {
                return UnityEditor.AssetDatabase.GetAllAssetBundleNames();
            }
        }

        public static List<AssetPath> GetAllAssetPaths()
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_GetAllAssetPaths.Auto())
            {
                return AllAssetPaths;
            }
        }

        public static List<AssetPath> GetAllProjectPaths()
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_GetAllProjectPaths.Auto())
            {
                return AllProjectPaths;
            }
        }

        /// <summary>
        ///     <para>Given an assetBundleName, returns the list of AssetBundles that it depends on.</para>
        /// </summary>
        /// <param name="assetBundleName">The name of the AssetBundle for which dependencies are required.</param>
        /// <param name="recursive">
        ///     If false, returns only AssetBundles which are direct dependencies of the input; if true, includes all indirect dependencies
        ///     of the input.
        /// </param>
        /// <returns>
        ///     <para>The names of all AssetBundles that the input depends on.</para>
        /// </returns>
        public static string[] GetAssetBundleDependencies(string assetBundleName, bool recursive)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_GetAssetBundleDependencies.Auto())
            {
                return UnityEditor.AssetDatabase.GetAssetBundleDependencies(assetBundleName, recursive);
            }
        }

        /// <summary>
        ///     <para>Returns the hash of all the dependencies of an asset.</para>
        /// </summary>
        /// <param name="path">Path to the asset.</param>
        /// <param name="guid">GUID of the asset.</param>
        /// <returns>
        ///     <para>Aggregate hash.</para>
        /// </returns>
        public static Hash128 GetAssetDependencyHash(UnityEditor.GUID guid)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_GetAssetDependencyHash.Auto())
            {
                return UnityEditor.AssetDatabase.GetAssetDependencyHash(guid);
            }
        }

        /// <summary>
        ///     <para>Returns the hash of all the dependencies of an asset.</para>
        /// </summary>
        /// <param name="path">Path to the asset.</param>
        /// <param name="guid">GUID of the asset.</param>
        /// <returns>
        ///     <para>Aggregate hash.</para>
        /// </returns>
        public static Hash128 GetAssetDependencyHash(string path)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_GetAssetDependencyHash.Auto())
            {
                var relativePath = path.ToRelativePath();
                return UnityEditor.AssetDatabase.GetAssetDependencyHash(relativePath);
            }
        }

        /// <summary>
        ///     <para>Returns the path name relative to the project folder where the asset is stored.</para>
        /// </summary>
        /// <param name="assetObject"></param>
        public static string GetAssetOrScenePath(Object assetObject)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_GetAssetOrScenePath.Auto())
            {
                return UnityEditor.AssetDatabase.GetAssetOrScenePath(assetObject);
            }
        }

        /// <summary>
        ///     <para>Returns the path name relative to the project folder where the asset is stored.</para>
        /// </summary>
        /// <param name="instanceID">The instance ID of the asset.</param>
        /// <param name="assetObject">A reference to the asset.</param>
        /// <returns>
        ///     <para>The asset path name, or null, or an empty string if the asset does not exist.</para>
        /// </returns>
        public static string GetAssetPath(Object assetObject)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_GetAssetPath.Auto())
            {
                return UnityEditor.AssetDatabase.GetAssetPath(assetObject);
            }
        }

        /// <summary>
        ///     <para>Returns the path name relative to the project folder where the asset is stored.</para>
        /// </summary>
        /// <param name="instanceID">The instance ID of the asset.</param>
        /// <param name="assetObject">A reference to the asset.</param>
        /// <returns>
        ///     <para>The asset path name, or null, or an empty string if the asset does not exist.</para>
        /// </returns>
        public static string GetAssetPath(int instanceID)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_GetAssetPath.Auto())
            {
                return UnityEditor.AssetDatabase.GetAssetPath(instanceID);
            }
        }

        /// <summary>
        ///     <para>Gets the path to the asset file associated with a text .meta file.</para>
        /// </summary>
        /// <param name="path"></param>
        public static string GetAssetPathFromTextMetaFilePath(string path)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_GetAssetPathFromTextMetaFilePath.Auto())
            {
                var relativePath = path.ToRelativePath();
                return UnityEditor.AssetDatabase.GetAssetPathFromTextMetaFilePath(relativePath);
            }
        }

        /// <summary>
        ///     <para>Returns an array containing the paths of all assets marked with the specified Asset Bundle name.</para>
        /// </summary>
        /// <param name="assetBundleName"></param>
        public static string[] GetAssetPathsFromAssetBundle(string assetBundleName)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_GetAssetPathsFromAssetBundle.Auto())
            {
                return UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleName);
            }
        }

        /// <summary>
        ///     <para>
        ///         Get the Asset paths for all Assets tagged with assetBundleName and
        ///         named assetName.
        ///     </para>
        /// </summary>
        /// <param name="assetBundleName"></param>
        /// <param name="assetName"></param>
        public static string[] GetAssetPathsFromAssetBundleAndAssetName(
            string assetBundleName,
            string assetName)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_GetAssetPathsFromAssetBundleAndAssetName.Auto())
            {
                return UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(
                    assetBundleName,
                    assetName
                );
            }
        }

        /// <summary>
        ///     <para>Gets the importer types associated with a given Asset type.</para>
        /// </summary>
        /// <param name="path">The Asset path.</param>
        /// <returns>
        ///     <para>Returns an array of importer types that can handle the specified Asset.</para>
        /// </returns>
        public static Type[] GetAvailableImporterTypes(string path)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_GetAvailableImporterTypes.Auto())
            {
                var relativePath = path.ToRelativePath();
                return UnityEditor.AssetDatabase.GetAvailableImporterTypes(relativePath);
            }
        }

        public static Object GetBuiltinExtraResource(Type type, string path)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_GetBuiltinExtraResource.Auto())
            {
                var relativePath = path.ToRelativePath();
                return UnityEditor.AssetDatabase.GetBuiltinExtraResource(type, relativePath);
            }
        }

        public static T GetBuiltinExtraResource<T>(string path)
            where T : Object
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_GetBuiltinExtraResource.Auto())
            {
                var relativePath = path.ToRelativePath();
                return UnityEditor.AssetDatabase.GetBuiltinExtraResource<T>(relativePath);
            }
        }

        /// <summary>
        ///     <para>Retrieves an icon for the asset at the given asset path.</para>
        /// </summary>
        /// <param name="path"></param>
        public static Texture GetCachedIcon(string path)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_GetCachedIcon.Auto())
            {
                var relativePath = path.ToRelativePath();
                return UnityEditor.AssetDatabase.GetCachedIcon(relativePath);
            }
        }

        /// <summary>
        ///     <para>Gets the IP address of the Cache Server in Editor Settings.</para>
        /// </summary>
        /// <returns>
        ///     <para>Returns the IP address of the Cache Server in Editor Settings. Returns empty string if IP address is not set in Editor settings.</para>
        /// </returns>
        public static string GetCacheServerAddress()
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_GetCacheServerAddress.Auto())
            {
                return UnityEditor.AssetDatabase.GetCacheServerAddress();
            }
        }

        /// <summary>
        ///     <para>Gets the Cache Server Download option from Editor Settings.</para>
        /// </summary>
        /// <returns>
        ///     <para>Returns true when Download from the Cache Server is enabled. Returns false otherwise.</para>
        /// </returns>
        public static bool GetCacheServerEnableDownload()
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_GetCacheServerEnableDownload.Auto())
            {
                return UnityEditor.AssetDatabase.GetCacheServerEnableDownload();
            }
        }

        /// <summary>
        ///     <para>Gets the Cache Server Upload option from Editor Settings.</para>
        /// </summary>
        /// <returns>
        ///     <para>Returns true when Upload to the Cache Server is enabled. Returns false otherwise.</para>
        /// </returns>
        public static bool GetCacheServerEnableUpload()
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_GetCacheServerEnableUpload.Auto())
            {
                return UnityEditor.AssetDatabase.GetCacheServerEnableUpload();
            }
        }

        /// <summary>
        ///     <para>Gets the Cache Server Namespace prefix set in Editor Settings.</para>
        /// </summary>
        /// <returns>
        ///     <para>Returns the Namespace prefix for the Cache Server.</para>
        /// </returns>
        public static string GetCacheServerNamespacePrefix()
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_GetCacheServerNamespacePrefix.Auto())
            {
                return UnityEditor.AssetDatabase.GetCacheServerNamespacePrefix();
            }
        }

        /// <summary>
        ///     <para>Gets the Port number of the Cache Server in Editor Settings.</para>
        /// </summary>
        /// <returns>
        ///     <para>Returns the Port number of the Cache Server in Editor Settings. Returns 0 if Port number is not set in Editor Settings.</para>
        /// </returns>
        public static ushort GetCacheServerPort()
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_GetCacheServerPort.Auto())
            {
                return UnityEditor.AssetDatabase.GetCacheServerPort();
            }
        }

        /// <summary>
        ///     <para>Gets the IP address of the Cache Server currently in use by the Editor.</para>
        /// </summary>
        /// <returns>
        ///     <para>Returns a string representation of the current Cache Server IP address.</para>
        /// </returns>
        public static string GetCurrentCacheServerIp()
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_GetCurrentCacheServerIp.Auto())
            {
                return UnityEditor.AssetDatabase.GetCurrentCacheServerIp();
            }
        }

        /// <summary>
        ///     <para>
        ///         Returns an array of all the assets that are dependencies of the asset at the specified pathName.
        ///         Note: GetDependencies() gets the Assets that are referenced by other Assets. For example, a Scene could contain many GameObjects with a
        ///         Material attached to them. In this case,  GetDependencies() will return the path to the Material Assets, but not the GameObjects as those are
        ///         not Assets on your disk.
        ///     </para>
        /// </summary>
        /// <param name="path">The path to the asset for which dependencies are required.</param>
        /// <param name="recursive">
        ///     Controls whether this method recursively checks and returns all dependencies including indirect dependencies (when set to
        ///     true), or whether it only returns direct dependencies (when set to false).
        /// </param>
        /// <returns>
        ///     <para>The paths of all assets that the input depends on.</para>
        /// </returns>
        public static string[] GetDependencies(string path)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_GetDependencies.Auto())
            {
                var relativePath = path.ToRelativePath();
                return UnityEditor.AssetDatabase.GetDependencies(relativePath);
            }
        }

        /// <summary>
        ///     <para>
        ///         Returns an array of all the assets that are dependencies of the asset at the specified pathName.
        ///         Note: GetDependencies() gets the Assets that are referenced by other Assets. For example, a Scene could contain many GameObjects with a
        ///         Material attached to them. In this case,  GetDependencies() will return the path to the Material Assets, but not the GameObjects as those are
        ///         not Assets on your disk.
        ///     </para>
        /// </summary>
        /// <param name="path">The path to the asset for which dependencies are required.</param>
        /// <param name="recursive">
        ///     Controls whether this method recursively checks and returns all dependencies including indirect dependencies (when set to
        ///     true), or whether it only returns direct dependencies (when set to false).
        /// </param>
        /// <returns>
        ///     <para>The paths of all assets that the input depends on.</para>
        /// </returns>
        public static string[] GetDependencies(string path, bool recursive)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_GetDependencies.Auto())
            {
                var relativePath = path.ToRelativePath();
                return UnityEditor.AssetDatabase.GetDependencies(relativePath, recursive);
            }
        }

        /// <summary>
        ///     <para>
        ///         Returns an array of the paths of assets that are dependencies of all the assets in the list of pathNames that you provide.
        ///         Note: GetDependencies() gets the Assets that are referenced by other Assets. For example, a Scene could contain many GameObjects with a
        ///         Material attached to them. In this case,  GetDependencies() will return the path to the Material Assets, but not the GameObjects as those are
        ///         not Assets on your disk.
        ///     </para>
        /// </summary>
        /// <param name="paths">The path to the assets for which dependencies are required.</param>
        /// <param name="recursive">
        ///     Controls whether this method recursively checks and returns all dependencies including indirect dependencies (when set to
        ///     true), or whether it only returns direct dependencies (when set to false).
        /// </param>
        /// <returns>
        ///     <para>The paths of all assets that the input depends on.</para>
        /// </returns>
        public static string[] GetDependencies(string[] paths)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_GetDependencies.Auto())
            {
                var relativePaths = paths.ToRelativePaths();
                return UnityEditor.AssetDatabase.GetDependencies(relativePaths);
            }
        }

        /// <summary>
        ///     <para>
        ///         Returns an array of the paths of assets that are dependencies of all the assets in the list of pathNames that you provide.
        ///         Note: GetDependencies() gets the Assets that are referenced by other Assets. For example, a Scene could contain many GameObjects with a
        ///         Material attached to them. In this case,  GetDependencies() will return the path to the Material Assets, but not the GameObjects as those are
        ///         not Assets on your disk.
        ///     </para>
        /// </summary>
        /// <param name="paths">The path to the assets for which dependencies are required.</param>
        /// <param name="recursive">
        ///     Controls whether this method recursively checks and returns all dependencies including indirect dependencies (when set to
        ///     true), or whether it only returns direct dependencies (when set to false).
        /// </param>
        /// <returns>
        ///     <para>The paths of all assets that the input depends on.</para>
        /// </returns>
        public static string[] GetDependencies(string[] paths, bool recursive)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_GetDependencies.Auto())
            {
                var relativePaths = paths.ToRelativePaths();
                return UnityEditor.AssetDatabase.GetDependencies(relativePaths, recursive);
            }
        }

        /// <summary>
        ///     <para>Returns the name of the AssetBundle that a given asset belongs to.</para>
        /// </summary>
        /// <param name="path">The asset's path.</param>
        /// <returns>
        ///     <para>Returns the name of the AssetBundle that a given asset belongs to. See the method description for more details.</para>
        /// </returns>
        public static string GetImplicitAssetBundleName(string path)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_GetImplicitAssetBundleName.Auto())
            {
                var relativePath = path.ToRelativePath();
                return UnityEditor.AssetDatabase.GetImplicitAssetBundleName(relativePath);
            }
        }

        /// <summary>
        ///     <para>Returns the name of the AssetBundle Variant that a given asset belongs to.</para>
        /// </summary>
        /// <param name="path">The asset's path.</param>
        /// <returns>
        ///     <para>Returns the name of the AssetBundle Variant that a given asset belongs to. See the method description for more details.</para>
        /// </returns>
        public static string GetImplicitAssetBundleVariantName(string path)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_GetImplicitAssetBundleVariantName.Auto())
            {
                var relativePath = path.ToRelativePath();
                return UnityEditor.AssetDatabase.GetImplicitAssetBundleVariantName(relativePath);
            }
        }

        /// <summary>
        ///     <para>Returns the type of the override importer.</para>
        /// </summary>
        /// <param name="path">Asset path.</param>
        /// <returns>
        ///     <para>Importer type.</para>
        /// </returns>
        public static Type GetImporterOverride(string path)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_GetImporterOverride.Auto())
            {
                var relativePath = path.ToRelativePath();
                return UnityEditor.AssetDatabase.GetImporterOverride(relativePath);
            }
        }

        public static string[] GetLabels(UnityEditor.GUID guid)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_GetLabels.Auto())
            {
                return UnityEditor.AssetDatabase.GetLabels(guid);
            }
        }

        /// <summary>
        ///     <para>Returns all labels attached to a given asset.</para>
        /// </summary>
        /// <param name="obj"></param>
        public static string[] GetLabels(Object obj)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_GetLabels.Auto())
            {
                return UnityEditor.AssetDatabase.GetLabels(obj);
            }
        }

        /// <summary>
        ///     <para>Returns the type of the main asset object at assetPath.</para>
        /// </summary>
        /// <param name="path">Filesystem path of the asset to load.</param>
        public static Type GetMainAssetTypeAtPath(AssetPath path)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_GetMainAssetTypeAtPath.Auto())
            {
                var relativePath = path.RelativePath;
                var result = UnityEditor.AssetDatabase.GetMainAssetTypeAtPath(relativePath);

                if (result != null)
                {
                    return result;
                }

                var assetObjects = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(relativePath);

                if (assetObjects is { Length: > 0 })
                {
                    var first = assetObjects[0];

                    if (first == null)
                    {
                        return null;
                    }

                    result = first.GetType();
                }

                return result;
            }
        }

        /// <summary>
        ///     <para>Returns the type of the main asset object at assetPath.</para>
        /// </summary>
        /// <param name="path">Filesystem path of the asset to load.</param>
        public static Type GetMainAssetTypeAtPath(string path)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_GetMainAssetTypeAtPath.Auto())
            {
                var relativePath = path.ToRelativePath();
                var result = UnityEditor.AssetDatabase.GetMainAssetTypeAtPath(relativePath);

                if (result != null)
                {
                    return result;
                }

                var assetObjects = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(relativePath);

                if (assetObjects is { Length: > 0 })
                {
                    var first = assetObjects[0];

                    if (first == null)
                    {
                        return null;
                    }

                    result = first.GetType();
                }

                return result;
            }
        }

        /// <summary>
        ///     <para>
        ///         Given a path to a directory in the Assets folder, relative to the project folder, this method will return an array of all its
        ///         subdirectories.
        ///     </para>
        /// </summary>
        /// <param name="path"></param>
        public static string[] GetSubFolders(string path)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_GetSubFolders.Auto())
            {
                var relativePath = path.ToRelativePath();
                return UnityEditor.AssetDatabase.GetSubFolders(relativePath);
            }
        }

        /// <summary>
        ///     <para>Gets the path to the text .meta file associated with an asset.</para>
        /// </summary>
        /// <param name="path">The path to the asset.</param>
        /// <returns>
        ///     <para>The path to the .meta text file or empty string if the file does not exist.</para>
        /// </returns>
        public static string GetTextMetaDataPathFromAssetPath(string path)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_GetTextMetaDataPathFromAssetPath.Auto())
            {
                return UnityEditor.AssetDatabase.GetTextMetaFilePathFromAssetPath(path);
            }
        }

        /// <summary>
        ///     <para>Gets the path to the text .meta file associated with an asset.</para>
        /// </summary>
        /// <param name="path">The path to the asset.</param>
        /// <returns>
        ///     <para>The path to the .meta text file or an empty string if the file does not exist.</para>
        /// </returns>
        public static string GetTextMetaFilePathFromAssetPath(string path)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_GetTextMetaFilePathFromAssetPath.Auto())
            {
                var relativePath = path.ToRelativePath();
                return UnityEditor.AssetDatabase.GetTextMetaFilePathFromAssetPath(relativePath);
            }
        }

        /// <summary>
        ///     <para>Gets an object's type from an Asset path and a local file identifier.</para>
        /// </summary>
        /// <param name="path">The Asset's path.</param>
        /// <param name="localIdentifierInFile">The object's local file identifier.</param>
        /// <returns>
        ///     <para>The object's type.</para>
        /// </returns>
        public static Type GetTypeFromPathAndFileID(string path, long localIdentifierInFile)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_GetTypeFromPathAndFileID.Auto())
            {
                var relativePath = path.ToRelativePath();
                return UnityEditor.AssetDatabase.GetTypeFromPathAndFileID(
                    relativePath,
                    localIdentifierInFile
                );
            }
        }

        /// <summary>
        ///     <para>Return all the unused assetBundle names in the asset database.</para>
        /// </summary>
        public static string[] GetUnusedAssetBundleNames()
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_GetUnusedAssetBundleNames.Auto())
            {
                return UnityEditor.AssetDatabase.GetUnusedAssetBundleNames();
            }
        }

        /// <summary>
        ///     <para>Get the GUID for the asset at path.</para>
        /// </summary>
        /// <param name="path">Filesystem path for the asset. All paths are relative to the project folder.</param>
        /// <returns>
        ///     <para>The GUID of the asset. An all-zero GUID denotes an invalid asset path.</para>
        /// </returns>
        public static UnityEditor.GUID GUIDFromAssetPath(string path)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_GUIDFromAssetPath.Auto())
            {
                var relativePath = path.ToRelativePath();
                return UnityEditor.AssetDatabase.GUIDFromAssetPath(relativePath);
            }
        }

        /// <summary>
        ///     <para>Gets the corresponding asset path for the supplied GUID, or an empty string if the GUID can't be found.</para>
        /// </summary>
        /// <param name="guid">The GUID of an asset.</param>
        /// <returns>
        ///     <para>Path of the asset relative to the project folder.</para>
        /// </returns>
        public static AssetPath GUIDToAssetPath(string guid)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_GUIDToAssetPath.Auto())
            {
                var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                return AssetPath.FromRelativePath(path);
            }
        }

        /// <summary>
        ///     <para>Gets the corresponding asset path for the supplied GUID, or an empty string if the GUID can't be found.</para>
        /// </summary>
        /// <param name="guid">The GUID of an asset.</param>
        /// <returns>
        ///     <para>Path of the asset relative to the project folder.</para>
        /// </returns>
        public static AssetPath GUIDToAssetPath(UnityEditor.GUID guid)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_GUIDToAssetPath.Auto())
            {
                var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                return AssetPath.FromRelativePath(path);
            }
        }

        public static Object ImportAndLoadAssetAtPath(Type t, string path)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_ImportAndLoadAssetAtPath.Auto())
            {
                var relativePath = path.ToRelativePath();
                UnityEditor.AssetDatabase.ImportAsset(relativePath);

                Object result = null;

                var mainType = UnityEditor.AssetDatabase.GetMainAssetTypeAtPath(relativePath);

                if (mainType == null)
                {
                    UnityEditor.AssetDatabase.ImportAsset(relativePath);

                    Refresh();

                    mainType = UnityEditor.AssetDatabase.GetMainAssetTypeAtPath(relativePath);
                }

                if (mainType == null)
                {
                    var foundObject = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(relativePath)
                                                 .FirstOrDefault();

                    mainType = foundObject?.GetType();

                    if (mainType == t)
                    {
                        result = foundObject;
                    }
                }

                if (mainType == null)
                {
                    Context.Log.Error(
                        ZString.Format(
                            "Asset at [{0}] does not have a type.  It may be corrupted.",
                            relativePath
                        )
                    );

                    return null;
                }

                if (mainType != t)
                {
                    Context.Log.Warn(
                        ZString.Format(
                            "Attempting to load asset at [{0}] using a type [{1}].{2}] when the asset is type [{3}.{4}]",
                            relativePath,
                            t.Namespace,
                            t.Name,
                            mainType.Namespace,
                            mainType.Name
                        )
                    );
                }

                if (result == null)
                {
                    result = UnityEditor.AssetDatabase.LoadAssetAtPath(relativePath, t);
                }

                if (result == null)
                {
                    Context.Log.Error(
                        ZString.Format(
                            "Could not import and load asset of type [{0}] at [{1}].  Deleting it if it exists.",
                            t.Name,
                            relativePath
                        )
                    );

                    if (AppaFile.Exists(relativePath))
                    {
                        AppaFile.Delete(relativePath);
                    }
                }

                return result;
            }
        }

        public static T ImportAndLoadAssetAtPath<T>(string path)
            where T : Object
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_ImportAndLoadAssetAtPath.Auto())
            {
                return ImportAndLoadAssetAtPath(typeof(T), path) as T;
            }
        }

        /// <summary>
        ///     <para>Import asset at path.</para>
        /// </summary>
        /// <param name="path"></param>
        /// <param name="options"></param>
        public static void ImportAsset(string path)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_ImportAsset.Auto())
            {
                var relativePath = path.ToRelativePath();
                UnityEditor.AssetDatabase.ImportAsset(relativePath);
            }
        }

        /// <summary>
        ///     <para>Import asset at path.</para>
        /// </summary>
        /// <param name="path"></param>
        /// <param name="options"></param>
        public static void ImportAsset(string path, UnityEditor.ImportAssetOptions options)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_ImportAsset.Auto())
            {
                var relativePath = path.ToRelativePath();
                UnityEditor.AssetDatabase.ImportAsset(relativePath, options);
            }
        }

        /// <summary>
        ///     <para>Imports package at packagePath into the current project.</para>
        /// </summary>
        /// <param name="packagePath"></param>
        /// <param name="interactive"></param>
        public static void ImportPackage(string path, bool interactive)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_ImportPackage.Auto())
            {
                var relativePath = path.ToRelativePath();
                UnityEditor.AssetDatabase.ImportPackage(relativePath, interactive);
            }
        }

        public static bool IsAssetImportWorkerProcess()
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_IsAssetImportWorkerProcess.Auto())
            {
                return UnityEditor.AssetDatabase.IsAssetImportWorkerProcess();
            }
        }

        /// <summary>
        ///     <para>Checks whether the Cache Server is enabled in Project Settings.</para>
        /// </summary>
        /// <returns>
        ///     <para>Returns true when the Cache Server is enabled. Returns false otherwise.</para>
        /// </returns>
        public static bool IsCacheServerEnabled()
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_IsCacheServerEnabled.Auto())
            {
                return UnityEditor.AssetDatabase.IsCacheServerEnabled();
            }
        }

        /// <summary>
        ///     <para>Checks connection status of the Cache Server.</para>
        /// </summary>
        /// <returns>
        ///     <para>Returns true when Editor is connected to the Cache Server. Returns false otherwise.</para>
        /// </returns>
        public static bool IsConnectedToCacheServer()
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_IsConnectedToCacheServer.Auto())
            {
                return UnityEditor.AssetDatabase.IsConnectedToCacheServer();
            }
        }

        /// <summary>
        ///     <para>Reports whether Directory Monitoring is enabled.</para>
        /// </summary>
        /// <returns>
        ///     <para>Returns true when Directory Monitoring is enabled. Returns false otherwise.</para>
        /// </returns>
        public static bool IsDirectoryMonitoringEnabled()
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_IsDirectoryMonitoringEnabled.Auto())
            {
                return UnityEditor.AssetDatabase.IsDirectoryMonitoringEnabled();
            }
        }

        /// <summary>
        ///     <para>Determines whether the Asset is a foreign Asset.</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="instanceID"></param>
        public static bool IsForeignAsset(Object obj)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_IsForeignAsset.Auto())
            {
                return UnityEditor.AssetDatabase.IsForeignAsset(obj);
            }
        }

        /// <summary>
        ///     <para>Determines whether the Asset is a foreign Asset.</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="instanceID"></param>
        public static bool IsForeignAsset(int instanceID)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_IsForeignAsset.Auto())
            {
                return UnityEditor.AssetDatabase.IsForeignAsset(instanceID);
            }
        }

        /// <summary>
        ///     <para>Is asset a main asset in the project window?</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="instanceID"></param>
        public static bool IsMainAsset(Object obj)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_IsMainAsset.Auto())
            {
                return UnityEditor.AssetDatabase.IsMainAsset(obj);
            }
        }

        /// <summary>
        ///     <para>Is asset a main asset in the project window?</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="instanceID"></param>
        public static bool IsMainAsset(int instanceID)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_IsMainAsset.Auto())
            {
                return UnityEditor.AssetDatabase.IsMainAsset(instanceID);
            }
        }

        /// <summary>
        ///     <para>Returns true if the main asset object at assetPath is loaded in memory.</para>
        /// </summary>
        /// <param name="path">Filesystem path of the asset to load.</param>
        public static bool IsMainAssetAtPathLoaded(string path)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_IsMainAssetAtPathLoaded.Auto())
            {
                var relativePath = path.ToRelativePath();
                return UnityEditor.AssetDatabase.IsMainAssetAtPathLoaded(relativePath);
            }
        }

        /// <summary>
        ///     <para>Query whether an asset's metadata (.meta) file is open for edit in version control.</para>
        /// </summary>
        /// <param name="assetObject">Object representing the asset whose metadata status you wish to query.</param>
        /// <param name="message">Returns a reason for the asset metadata not being open for edit.</param>
        /// <param name="statusOptions">
        ///     Options for how the version control system should be queried. These options can effect the speed and accuracy of the
        ///     query. Default is StatusQueryOptions.UseCachedIfPossible.
        /// </param>
        /// <returns>
        ///     <para>True if the asset's metadata is considered open for edit by the selected version control system.</para>
        /// </returns>
        public static bool IsMetaFileOpenForEdit(Object assetObject)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_IsMetaFileOpenForEdit.Auto())
            {
                return UnityEditor.AssetDatabase.IsMetaFileOpenForEdit(assetObject);
            }
        }

        /// <summary>
        ///     <para>Query whether an asset's metadata (.meta) file is open for edit in version control.</para>
        /// </summary>
        /// <param name="assetObject">Object representing the asset whose metadata status you wish to query.</param>
        /// <param name="message">Returns a reason for the asset metadata not being open for edit.</param>
        /// <param name="statusOptions">
        ///     Options for how the version control system should be queried. These options can effect the speed and accuracy of the
        ///     query. Default is StatusQueryOptions.UseCachedIfPossible.
        /// </param>
        /// <returns>
        ///     <para>True if the asset's metadata is considered open for edit by the selected version control system.</para>
        /// </returns>
        public static bool IsMetaFileOpenForEdit(
            Object assetObject,
            UnityEditor.StatusQueryOptions statusOptions)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_IsMetaFileOpenForEdit.Auto())
            {
                return UnityEditor.AssetDatabase.IsMetaFileOpenForEdit(assetObject, statusOptions);
            }
        }

        public static bool IsMetaFileOpenForEdit(Object assetObject, out string message)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_IsMetaFileOpenForEdit.Auto())
            {
                return UnityEditor.AssetDatabase.IsMetaFileOpenForEdit(assetObject, out message);
            }
        }

        public static bool IsMetaFileOpenForEdit(
            Object assetObject,
            out string message,
            UnityEditor.StatusQueryOptions statusOptions)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_IsMetaFileOpenForEdit.Auto())
            {
                return UnityEditor.AssetDatabase.IsMetaFileOpenForEdit(
                    assetObject,
                    out message,
                    statusOptions
                );
            }
        }

        /// <summary>
        ///     <para>Determines whether the Asset is a native Asset.</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="instanceID"></param>
        public static bool IsNativeAsset(Object obj)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_IsNativeAsset.Auto())
            {
                return UnityEditor.AssetDatabase.IsNativeAsset(obj);
            }
        }

        /// <summary>
        ///     <para>Determines whether the Asset is a native Asset.</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="instanceID"></param>
        public static bool IsNativeAsset(int instanceID)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_IsNativeAsset.Auto())
            {
                return UnityEditor.AssetDatabase.IsNativeAsset(instanceID);
            }
        }

        /// <summary>
        ///     <para>Query whether an Asset file is open for editing in version control.</para>
        /// </summary>
        /// <param name="assetObject">Object representing the asset whose status you wish to query.</param>
        /// <param name="path">Path to the asset file or its .meta file on disk, relative to project folder.</param>
        /// <param name="message">Returns a reason for the asset not being open for edit.</param>
        /// <param name="statusOptions">
        ///     Options for how the version control system should be queried. These options can effect the speed and accuracy of the
        ///     query. Default is StatusQueryOptions.UseCachedIfPossible.
        /// </param>
        /// <returns>
        ///     <para>True if the asset is considered open for edit by the selected version control system.</para>
        /// </returns>
        public static bool IsOpenForEdit(Object assetObject)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_IsOpenForEdit.Auto())
            {
                return UnityEditor.AssetDatabase.IsOpenForEdit(assetObject);
            }
        }

        /// <summary>
        ///     <para>Query whether an Asset file is open for editing in version control.</para>
        /// </summary>
        /// <param name="assetObject">Object representing the asset whose status you wish to query.</param>
        /// <param name="path">Path to the asset file or its .meta file on disk, relative to project folder.</param>
        /// <param name="message">Returns a reason for the asset not being open for edit.</param>
        /// <param name="statusOptions">
        ///     Options for how the version control system should be queried. These options can effect the speed and accuracy of the
        ///     query. Default is StatusQueryOptions.UseCachedIfPossible.
        /// </param>
        /// <returns>
        ///     <para>True if the asset is considered open for edit by the selected version control system.</para>
        /// </returns>
        public static bool IsOpenForEdit(Object assetObject, UnityEditor.StatusQueryOptions statusOptions)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_IsOpenForEdit.Auto())
            {
                return UnityEditor.AssetDatabase.IsOpenForEdit(assetObject, statusOptions);
            }
        }

        /// <summary>
        ///     <para>Query whether an Asset file is open for editing in version control.</para>
        /// </summary>
        /// <param name="assetObject">Object representing the asset whose status you wish to query.</param>
        /// <param name="path">Path to the asset file or its .meta file on disk, relative to project folder.</param>
        /// <param name="message">Returns a reason for the asset not being open for edit.</param>
        /// <param name="statusOptions">
        ///     Options for how the version control system should be queried. These options can effect the speed and accuracy of the
        ///     query. Default is StatusQueryOptions.UseCachedIfPossible.
        /// </param>
        /// <returns>
        ///     <para>True if the asset is considered open for edit by the selected version control system.</para>
        /// </returns>
        public static bool IsOpenForEdit(string path)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_IsOpenForEdit.Auto())
            {
                var relativePath = path.ToRelativePath();
                return UnityEditor.AssetDatabase.IsOpenForEdit(relativePath);
            }
        }

        /// <summary>
        ///     <para>Query whether an Asset file is open for editing in version control.</para>
        /// </summary>
        /// <param name="assetObject">Object representing the asset whose status you wish to query.</param>
        /// <param name="path">Path to the asset file or its .meta file on disk, relative to project folder.</param>
        /// <param name="message">Returns a reason for the asset not being open for edit.</param>
        /// <param name="statusOptions">
        ///     Options for how the version control system should be queried. These options can effect the speed and accuracy of the
        ///     query. Default is StatusQueryOptions.UseCachedIfPossible.
        /// </param>
        /// <returns>
        ///     <para>True if the asset is considered open for edit by the selected version control system.</para>
        /// </returns>
        public static bool IsOpenForEdit(string path, UnityEditor.StatusQueryOptions statusOptions)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_IsOpenForEdit.Auto())
            {
                var relativePath = path.ToRelativePath();
                return UnityEditor.AssetDatabase.IsOpenForEdit(relativePath, statusOptions);
            }
        }

        public static bool IsOpenForEdit(Object assetObject, out string message)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_IsOpenForEdit.Auto())
            {
                return UnityEditor.AssetDatabase.IsOpenForEdit(assetObject, out message);
            }
        }

        public static bool IsOpenForEdit(
            Object assetObject,
            out string message,
            UnityEditor.StatusQueryOptions statusOptions)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_IsOpenForEdit.Auto())
            {
                return UnityEditor.AssetDatabase.IsOpenForEdit(assetObject, out message, statusOptions);
            }
        }

        public static bool IsOpenForEdit(string path, out string message)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_IsOpenForEdit.Auto())
            {
                var relativePath = path.ToRelativePath();
                return UnityEditor.AssetDatabase.IsOpenForEdit(relativePath, out message);
            }
        }

        public static bool IsOpenForEdit(
            string path,
            out string message,
            UnityEditor.StatusQueryOptions statusOptions)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_IsOpenForEdit.Auto())
            {
                var relativePath = path.ToRelativePath();
                return UnityEditor.AssetDatabase.IsOpenForEdit(relativePath, out message, statusOptions);
            }
        }

        public static void IsOpenForEdit(
            string[] assetOrMetaFilePaths,
            List<string> outNotEditablePaths,
            UnityEditor.StatusQueryOptions statusQueryOptions =
                UnityEditor.StatusQueryOptions.UseCachedIfPossible)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_IsOpenForEdit.Auto())
            {
                UnityEditor.AssetDatabase.IsOpenForEdit(
                    assetOrMetaFilePaths,
                    outNotEditablePaths,
                    statusQueryOptions
                );
            }
        }

        /// <summary>
        ///     <para>Does the asset form part of another asset?</para>
        /// </summary>
        /// <param name="obj">The asset Object to query.</param>
        /// <param name="instanceID">Instance ID of the asset Object to query.</param>
        public static bool IsSubAsset(Object obj)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_IsSubAsset.Auto())
            {
                return UnityEditor.AssetDatabase.IsSubAsset(obj);
            }
        }

        /// <summary>
        ///     <para>Does the asset form part of another asset?</para>
        /// </summary>
        /// <param name="obj">The asset Object to query.</param>
        /// <param name="instanceID">Instance ID of the asset Object to query.</param>
        public static bool IsSubAsset(int instanceID)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_IsSubAsset.Auto())
            {
                return UnityEditor.AssetDatabase.IsSubAsset(instanceID);
            }
        }

        /// <summary>
        ///     <para>Given a path to a folder, returns true if it exists, false otherwise.</para>
        /// </summary>
        /// <param name="path">The path to the folder.</param>
        /// <returns>
        ///     <para>Returns true if the folder exists.</para>
        /// </returns>
        public static bool IsValidFolder(AssetPath path)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_IsValidFolder.Auto())
            {
                var relativePath = path.RelativePath;
                return UnityEditor.AssetDatabase.IsValidFolder(relativePath);
            }
        }

        /// <summary>
        ///     <para>Given a path to a folder, returns true if it exists, false otherwise.</para>
        /// </summary>
        /// <param name="path">The path to the folder.</param>
        /// <returns>
        ///     <para>Returns true if the folder exists.</para>
        /// </returns>
        public static bool IsValidFolder(string path)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_IsValidFolder.Auto())
            {
                var relativePath = path.ToRelativePath();
                return UnityEditor.AssetDatabase.IsValidFolder(relativePath);
            }
        }

        /// <summary>
        ///     <para>Returns all sub Assets at assetPath.</para>
        /// </summary>
        /// <param name="path"></param>
        public static Object[] LoadAllAssetRepresentationsAtPath(AssetPath path)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_LoadAllAssetRepresentationsAtPath.Auto())
            {
                var relativePath = path.RelativePath;
                return UnityEditor.AssetDatabase.LoadAllAssetRepresentationsAtPath(relativePath);
            }
        }

        /// <summary>
        ///     <para>Returns all sub Assets at assetPath.</para>
        /// </summary>
        /// <param name="path"></param>
        public static Object[] LoadAllAssetRepresentationsAtPath(string path)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_LoadAllAssetRepresentationsAtPath.Auto())
            {
                var relativePath = path.ToRelativePath();
                return UnityEditor.AssetDatabase.LoadAllAssetRepresentationsAtPath(relativePath);
            }
        }

        /// <summary>
        ///     <para>Returns an array of all Assets at assetPath.</para>
        /// </summary>
        /// <param name="path">Filesystem path to the asset.</param>
        public static Object[] LoadAllAssetsAtPath(AssetPath path)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_LoadAllAssetsAtPath.Auto())
            {
                var relativePath = path.RelativePath;
                return UnityEditor.AssetDatabase.LoadAllAssetsAtPath(relativePath);
            }
        }

        /// <summary>
        ///     <para>Returns an array of all Assets at assetPath.</para>
        /// </summary>
        /// <param name="path">Filesystem path to the asset.</param>
        public static Object[] LoadAllAssetsAtPath(string path)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();

            if (path.EndsWith(".prefab"))
            {
                throw new NotSupportedException(
                    ZString.Format(
                        "If you intend to retrieve all objects from within a prefab, call {0} instead.",
                        nameof(LoadAllObjectsInPrefabAsset)
                    )
                );
            }

            using (_PRF_LoadAllAssetsAtPath.Auto())
            {
                var relativePath = path.ToRelativePath();
                return UnityEditor.AssetDatabase.LoadAllAssetsAtPath(relativePath);
            }
        }

        /// <summary>
        ///     <para>Returns an array of all Assets at assetPath.</para>
        /// </summary>
        /// <param name="path">Filesystem path to the asset.</param>
        public static Object[] LoadAllObjectsInPrefabAsset(string path)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_LoadAllObjectsInPrefabAsset.Auto())
            {
                var relativePath = path.ToRelativePath();
                return UnityEditor.AssetDatabase.LoadAllAssetsAtPath(relativePath);
            }
        }

        /// <summary>
        ///     <para>Returns the first asset object of type type at given path assetPath.</para>
        /// </summary>
        /// <param name="path">Path of the asset to load.</param>
        /// <param name="type">Data type of the asset.</param>
        /// <returns>
        ///     <para>The asset matching the parameters.</para>
        /// </returns>
        public static Object LoadAssetAtPath(AssetPath path, Type type)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_LoadAssetAtPath.Auto())
            {
                var relativePath = path.RelativePath;

                return UnityEditor.AssetDatabase.LoadAssetAtPath(relativePath, type);
            }
        }

        public static T LoadAssetAtPath<T>(AssetPath path)
            where T : Object
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_LoadAssetAtPath.Auto())
            {
                var relativePath = path.RelativePath;
                try
                {
                    return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(relativePath);
                }
                catch (Exception ex)
                {
                    Context.Log.Error(ZString.Format("Exception loading the asset at [{0}]", path), null, ex);

                    throw;
                }
            }
        }

        /// <summary>
        ///     <para>Returns the first asset object of type type at given path assetPath.</para>
        /// </summary>
        /// <param name="path">Path of the asset to load.</param>
        /// <param name="type">Data type of the asset.</param>
        /// <returns>
        ///     <para>The asset matching the parameters.</para>
        /// </returns>
        public static Object LoadAssetAtPath(string path, Type type)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_LoadAssetAtPath.Auto())
            {
                var relativePath = path.ToRelativePath();

                return UnityEditor.AssetDatabase.LoadAssetAtPath(relativePath, type);
            }
        }

        public static T LoadAssetAtPath<T>(string path)
            where T : Object
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();

            using (_PRF_LoadAssetAtPath.Auto())
            {
                var relativePath = path.ToRelativePath();
                try
                {
                    return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(relativePath);
                }
                catch (Exception ex)
                {
                    Context.Log.Error(ZString.Format("Exception loading the asset at [{0}]", path), null, ex);

                    throw;
                }
            }
        }

        /// <summary>
        ///     <para>
        ///         Returns the main asset object at assetPath.
        ///         The "main" Asset is the Asset at the root of a hierarchy (such as a Maya file which may contain multiples meshes and GameObjects).
        ///     </para>
        /// </summary>
        /// <param name="path">Filesystem path of the asset to load.</param>
        public static Object LoadMainAssetAtPath(AssetPath path)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_LoadMainAssetAtPath.Auto())
            {
                var relativePath = path.RelativePath;
                return UnityEditor.AssetDatabase.LoadMainAssetAtPath(relativePath);
            }
        }

        /// <summary>
        ///     <para>
        ///         Returns the main asset object at assetPath.
        ///         The "main" Asset is the Asset at the root of a hierarchy (such as a Maya file which may contain multiples meshes and GameObjects).
        ///     </para>
        /// </summary>
        /// <param name="path">Filesystem path of the asset to load.</param>
        public static Object LoadMainAssetAtPath(string path)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_LoadMainAssetAtPath.Auto())
            {
                var relativePath = path.ToRelativePath();

                return UnityEditor.AssetDatabase.LoadMainAssetAtPath(relativePath);
            }
        }

        /// <summary>
        ///     <para>Makes a file open for editing in version control.</para>
        /// </summary>
        /// <param name="path">Specifies the path to a file relative to the project root.</param>
        /// <returns>
        ///     <para>true if Unity successfully made the file editable in the version control system. Otherwise, returns false.</para>
        /// </returns>
        public static bool MakeEditable(string path)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_MakeEditable.Auto())
            {
                return UnityEditor.AssetDatabase.MakeEditable(path);
            }
        }

        public static bool MakeEditable(
            string[] paths,
            string prompt = null,
            List<string> outNotEditablePaths = null)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_MakeEditable.Auto())
            {
                return UnityEditor.AssetDatabase.MakeEditable(paths, prompt, outNotEditablePaths);
            }
        }

        /// <summary>
        ///     <para>Move an asset file (or folder) from one folder to another.</para>
        /// </summary>
        /// <param name="oldPath">The path where the asset currently resides.</param>
        /// <param name="newPath">The path which the asset should be moved to.</param>
        /// <returns>
        ///     <para>An empty string if the asset has been successfully moved, otherwise an error message.</para>
        /// </returns>
        public static string MoveAsset(string oldPath, string newPath)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_MoveAsset.Auto())
            {
                return UnityEditor.AssetDatabase.MoveAsset(oldPath, newPath);
            }
        }

        public static bool MoveAssetsToTrash(string[] paths, List<string> outFailedPaths)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_MoveAssetsToTrash.Auto())
            {
                return UnityEditor.AssetDatabase.MoveAssetsToTrash(paths, outFailedPaths);
            }
        }

        /// <summary>
        ///     <para>Moves the specified asset  or folder to the OS trash.</para>
        /// </summary>
        /// <param name="path">Project relative path of the asset or folder to be deleted.</param>
        /// <returns>
        ///     <para>Returns true if the asset has been successfully removed, false if it doesn't exist or couldn't be removed.</para>
        /// </returns>
        public static bool MoveAssetToTrash(string path)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_MoveAssetToTrash.Auto())
            {
                return UnityEditor.AssetDatabase.MoveAssetToTrash(path);
            }
        }

        /// <summary>
        ///     <para>Opens the asset with associated application.</para>
        /// </summary>
        /// <param name="instanceID"></param>
        /// <param name="lineNumber"></param>
        /// <param name="columnNumber"></param>
        /// <param name="target"></param>
        public static bool OpenAsset(int instanceID)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_OpenAsset.Auto())
            {
                return UnityEditor.AssetDatabase.OpenAsset(instanceID);
            }
        }

        /// <summary>
        ///     <para>Opens the asset with associated application.</para>
        /// </summary>
        /// <param name="instanceID"></param>
        /// <param name="lineNumber"></param>
        /// <param name="columnNumber"></param>
        /// <param name="target"></param>
        public static bool OpenAsset(int instanceID, int lineNumber)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_OpenAsset.Auto())
            {
                return UnityEditor.AssetDatabase.OpenAsset(instanceID, lineNumber);
            }
        }

        /// <summary>
        ///     <para>Opens the asset with associated application.</para>
        /// </summary>
        /// <param name="instanceID"></param>
        /// <param name="lineNumber"></param>
        /// <param name="columnNumber"></param>
        /// <param name="target"></param>
        public static bool OpenAsset(int instanceID, int lineNumber, int columnNumber)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_OpenAsset.Auto())
            {
                return UnityEditor.AssetDatabase.OpenAsset(instanceID, lineNumber, columnNumber);
            }
        }

        /// <summary>
        ///     <para>Opens the asset with associated application.</para>
        /// </summary>
        /// <param name="instanceID"></param>
        /// <param name="lineNumber"></param>
        /// <param name="columnNumber"></param>
        /// <param name="target"></param>
        public static bool OpenAsset(Object target)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_OpenAsset.Auto())
            {
                return UnityEditor.AssetDatabase.OpenAsset(target);
            }
        }

        /// <summary>
        ///     <para>Opens the asset with associated application.</para>
        /// </summary>
        /// <param name="instanceID"></param>
        /// <param name="lineNumber"></param>
        /// <param name="columnNumber"></param>
        /// <param name="target"></param>
        public static bool OpenAsset(Object target, int lineNumber)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_OpenAsset.Auto())
            {
                return UnityEditor.AssetDatabase.OpenAsset(target, lineNumber);
            }
        }

        /// <summary>
        ///     <para>Opens the asset with associated application.</para>
        /// </summary>
        /// <param name="instanceID"></param>
        /// <param name="lineNumber"></param>
        /// <param name="columnNumber"></param>
        /// <param name="target"></param>
        public static bool OpenAsset(Object target, int lineNumber, int columnNumber)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_OpenAsset.Auto())
            {
                return UnityEditor.AssetDatabase.OpenAsset(target, lineNumber, columnNumber);
            }
        }

        /// <summary>
        ///     <para>Opens the asset(s) with associated application(s).</para>
        /// </summary>
        /// <param name="objects"></param>
        public static bool OpenAsset(Object[] objects)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_OpenAsset.Auto())
            {
                return UnityEditor.AssetDatabase.OpenAsset(objects);
            }
        }

        /// <summary>
        ///     <para>Opens the asset with associated application.</para>
        /// </summary>
        /// <param name="instanceID"></param>
        /// <param name="lineNumber"></param>
        /// <param name="columnNumber"></param>
        /// <param name="target"></param>
        public static bool OpenAssetAtPath(string assetPath)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_OpenAssetAtPath.Auto())
            {
                var type = UnityEditor.AssetDatabase.GetMainAssetTypeAtPath(assetPath);
                var asset = UnityEditor.AssetDatabase.LoadAssetAtPath(assetPath, type);
                return UnityEditor.AssetDatabase.OpenAsset(asset);
            }
        }

        [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
        public static void Refresh(
            [CallerFilePath] string callerFilePath = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_Refresh.Auto())
            {
                Context.Log.Warn(
                    ZString.Format(
                        "{0} was called via {1} at {2}:{3}",
                        nameof(Refresh),
                        callerMemberName.FormatMethodForLogging(),
                        callerFilePath.FormatNameForLogging(),
                        callerLineNumber.FormatForLogging()
                    )
                );

                UnityEditor.AssetDatabase.Refresh();
            }
        }

        /// <summary>
        ///     <para>Import any changed assets.</para>
        /// </summary>
        /// <param name="options"></param>
        public static void Refresh(
            UnityEditor.ImportAssetOptions options,
            [CallerFilePath] string callerFilePath = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_Refresh.Auto())
            {
                Context.Log.Warn(
                    ZString.Format(
                        "{0} was called via {1} at {2}:{3}",
                        nameof(Refresh),
                        callerMemberName.FormatMethodForLogging(),
                        callerFilePath.FormatNameForLogging(),
                        callerLineNumber.FormatForLogging()
                    )
                );

                UnityEditor.AssetDatabase.Refresh(options);
            }
        }

        /// <summary>
        ///     <para>Apply pending Editor Settings changes to the Asset pipeline.</para>
        /// </summary>
        public static void RefreshSettings()
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_RefreshSettings.Auto())
            {
                UnityEditor.AssetDatabase.RefreshSettings();
            }
        }

        /// <summary>
        ///     <para>
        ///         Allows you to register a custom dependency that Assets can be dependent on. If you register a custom dependency, and specify that an Asset
        ///         is dependent on it, then the Asset will get re-imported if the custom dependency changes.
        ///     </para>
        /// </summary>
        /// <param name="dependency">
        ///     Name of dependency. You can use any name you like, but because these names are global across all your Assets, it can be
        ///     useful to use a naming convention (eg a path-based naming system) to avoid clashes with other custom dependency names.
        /// </param>
        /// <param name="hashOfValue">A Hash128 value of the dependency.</param>
        public static void RegisterCustomDependency(string dependency, Hash128 hashOfValue)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_RegisterCustomDependency.Auto())
            {
                UnityEditor.AssetDatabase.RegisterCustomDependency(dependency, hashOfValue);
            }
        }

        /// <summary>
        ///     <para>
        ///         Calling this function will release file handles internally cached by Unity. This allows modifying asset or meta files safely thus avoiding
        ///         potential file sharing IO errors.
        ///     </para>
        /// </summary>
        public static void ReleaseCachedFileHandles()
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_ReleaseCachedFileHandles.Auto())
            {
                UnityEditor.AssetDatabase.ReleaseCachedFileHandles();
            }
        }

        /// <summary>
        ///     <para>Remove the assetBundle name from the asset database. The forceRemove flag is used to indicate if you want to remove it even it's in use.</para>
        /// </summary>
        /// <param name="assetBundleName">The assetBundle name you want to remove.</param>
        /// <param name="forceRemove">Flag to indicate if you want to remove the assetBundle name even it's in use.</param>
        public static bool RemoveAssetBundleName(string assetBundleName, bool forceRemove)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_RemoveAssetBundleName.Auto())
            {
                return UnityEditor.AssetDatabase.RemoveAssetBundleName(assetBundleName, forceRemove);
            }
        }

        /// <summary>
        ///     <para>Removes object from its asset (See Also: UnityEditor.AssetDatabase.AddObjectToAsset).</para>
        /// </summary>
        /// <param name="objectToRemove"></param>
        public static void RemoveObjectFromAsset(Object objectToRemove)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_RemoveObjectFromAsset.Auto())
            {
                UnityEditor.AssetDatabase.RemoveObjectFromAsset(objectToRemove);
            }
        }

        /// <summary>
        ///     <para>Remove all the unused assetBundle names in the asset database.</para>
        /// </summary>
        public static void RemoveUnusedAssetBundleNames()
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_RemoveUnusedAssetBundleNames.Auto())
            {
                UnityEditor.AssetDatabase.RemoveUnusedAssetBundleNames();
            }
        }

        /// <summary>
        ///     <para>
        ///         Resets the internal cache server connection reconnect timer values. The default delay timer value is 1 second, and the max delay value is 5
        ///         minutes. Everytime a connection attempt fails it will double the delay timer value, until a maximum time of the max value.
        ///     </para>
        /// </summary>
        public static void ResetCacheServerReconnectTimer()
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_ResetCacheServerReconnectTimer.Auto())
            {
                UnityEditor.AssetDatabase.ResetCacheServerReconnectTimer();
            }
        }

        /// <summary>
        ///     <para>Writes all unsaved changes to the specified asset to disk.</para>
        /// </summary>
        /// <param name="obj">The asset object to be saved, if dirty.</param>
        /// <param name="guid">The guid of the asset to be saved, if dirty.</param>
        public static void SaveAssetIfDirty(UnityEditor.GUID guid)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_SaveAssetIfDirty.Auto())
            {
                UnityEditor.AssetDatabase.SaveAssetIfDirty(guid);
            }
        }

        /// <summary>
        ///     <para>Writes all unsaved changes to the specified asset to disk.</para>
        /// </summary>
        /// <param name="obj">The asset object to be saved, if dirty.</param>
        /// <param name="guid">The guid of the asset to be saved, if dirty.</param>
        public static void SaveAssetIfDirty(Object obj)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_SaveAssetIfDirty.Auto())
            {
                UnityEditor.AssetDatabase.SaveAssetIfDirty(obj);
            }
        }

        /// <summary>
        ///     <para>Writes all unsaved asset changes to disk.</para>
        /// </summary>
        public static void SaveAssets(
            [CallerFilePath] string callerFilePath = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_SaveAssets.Auto())
            {
                Context.Log.Warn(
                    ZString.Format(
                        "{0} was called via {1} at {2}:{3}",
                        nameof(SaveAssets),
                        callerMemberName.FormatMethodForLogging(),
                        callerFilePath.FormatNameForLogging(),
                        callerLineNumber.FormatForLogging()
                    )
                );

                UnityEditor.AssetDatabase.SaveAssets();
            }
        }

        public static void SetImporterOverride<T>(string path)
            where T : UnityEditor.AssetImporters.ScriptedImporter
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_SetImporterOverride.Auto())
            {
                var relativePath = path.ToRelativePath();
                UnityEditor.AssetDatabase.SetImporterOverride<T>(relativePath);
            }
        }

        /// <summary>
        ///     <para>Replaces that list of labels on an asset.</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="labels"></param>
        public static void SetLabels(Object obj, string[] labels)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_SetLabels.Auto())
            {
                UnityEditor.AssetDatabase.SetLabels(obj, labels);
            }
        }

        /// <summary>
        ///     <para>Specifies which object in the asset file should become the main object after the next import.</para>
        /// </summary>
        /// <param name="mainObject">The object to become the main object.</param>
        /// <param name="path">Path to the asset file.</param>
        public static void SetMainObject(Object mainObject, string path)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_SetMainObject.Auto())
            {
                var relativePath = path.ToRelativePath();
                UnityEditor.AssetDatabase.SetMainObject(mainObject, relativePath);
            }
        }

        /// <summary>
        ///     <para>
        ///         Starts importing Assets into the Asset Database. This lets you group several Asset imports together into one larger import.
        ///         Note:
        ///         Calling UnityEditor.AssetDatabase.StartAssetEditing() places the Asset Database in a state that will prevent imports until
        ///         UnityEditor.AssetDatabase.StopAssetEditing() is called.
        ///         This means that if an exception occurs between the two function calls, the AssetDatabase will be unresponsive.
        ///         Therefore, it is highly recommended that you place calls to UnityEditor.AssetDatabase.StartAssetEditing() and
        ///         UnityEditor.AssetDatabase.StopAssetEditing() inside
        ///         either a try..catch block, or a try..finally block as needed.
        ///     </para>
        /// </summary>
        public static void StartAssetEditing()
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_StartAssetEditing.Auto())
            {
                /*Context.Log.Trace("Asset database editing has begun and importing is suspended.");*/
                UnityEditor.AssetDatabase.StartAssetEditing();
            }
        }

        /// <summary>
        ///     <para>
        ///         Stops importing Assets into the Asset Database. This lets you group several Asset imports together into one larger import.
        ///         Note:
        ///         Calling UnityEditor.AssetDatabase.StartAssetEditing() places the Asset Database in a state that will prevent imports until
        ///         UnityEditor.AssetDatabase.StopAssetEditing() is called.
        ///         This means that if an exception occurs between the two function calls, the AssetDatabase will be unresponsive.
        ///         Therefore, it is highly recommended that you place calls to UnityEditor.AssetDatabase.StartAssetEditing() and
        ///         UnityEditor.AssetDatabase.StopAssetEditing() inside
        ///         either a try..catch block, or a try..finally block as needed.
        ///     </para>
        /// </summary>
        public static void StopAssetEditing()
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_StopAssetEditing.Auto())
            {
                /*Context.Log.Trace("Asset database editing has finished and importing will resume.");*/
                UnityEditor.AssetDatabase.StopAssetEditing();
            }
        }

        public static bool TryGetGUIDAndLocalFileIdentifier(Object obj, out string guid, out long localId)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_TryGetGUIDAndLocalFileIdentifier.Auto())
            {
                return UnityEditor.AssetDatabase.TryGetGUIDAndLocalFileIdentifier(obj, out guid, out localId);
            }
        }

        public static bool TryGetGUIDAndLocalFileIdentifier(int instanceID, out string guid, out long localId)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_TryGetGUIDAndLocalFileIdentifier.Auto())
            {
                return UnityEditor.AssetDatabase.TryGetGUIDAndLocalFileIdentifier(
                    instanceID,
                    out guid,
                    out localId
                );
            }
        }

        public static bool TryGetGUIDAndLocalFileIdentifier<T>(
            LazyLoadReference<T> assetRef,
            out string guid,
            out long localId)
            where T : Object
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_TryGetGUIDAndLocalFileIdentifier.Auto())
            {
                return UnityEditor.AssetDatabase.TryGetGUIDAndLocalFileIdentifier(
                    assetRef,
                    out guid,
                    out localId
                );
            }
        }

        /// <summary>
        ///     <para>Removes custom dependencies that match the prefixFilter.</para>
        /// </summary>
        /// <param name="prefixFilter">Prefix filter for the custom dependencies to unregister.</param>
        /// <returns>
        ///     <para>Number of custom dependencies removed.</para>
        /// </returns>
        public static uint UnregisterCustomDependencyPrefixFilter(string prefixFilter)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_UnregisterCustomDependencyPrefixFilter.Auto())
            {
                return UnityEditor.AssetDatabase.UnregisterCustomDependencyPrefixFilter(prefixFilter);
            }
        }

        /// <summary>
        ///     Renames an asset file.  The file extension is only required on <see cref="newName" /> to change the existing file extension.
        ///     The file will stay in the same directory.  To move the file to a different directory, call <see cref="MoveAsset" />.
        /// </summary>
        /// <param name="asset">The asset to rename.</param>
        /// <param name="newName">The new asset name.</param>
        public static void UpdateAssetName(Object asset, string newName)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();

            using (_PRF_UpdateAssetName.Auto())
            {
                var assetPath = GetAssetPath(asset);

                UpdateAssetName(assetPath, newName);
            }
        }

        /// <summary>
        ///     <para>Rename an asset file.</para>
        /// </summary>
        /// <param name="path">The path where the asset currently resides.</param>
        /// <param name="newName">The new name which should be given to the asset.</param>
        public static void UpdateAssetName(string path, string newName)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();

            using (_PRF_UpdateAssetName.Auto())
            {
                var oldExtension = AppaPath.GetExtension(path);

                var newBaseName = AppaPath.GetFileNameWithoutExtension(newName);
                var newExtension = AppaPath.GetExtension(newName);

                var newFileName = newExtension.IsNotNullOrWhiteSpace()
                    ? newName
                    : ZString.Concat(newBaseName, oldExtension);

                var relativePath = path.ToRelativePath();

                var assetObject = UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(path);
                assetObject.name = newBaseName;
                UnityEditor.EditorUtility.SetDirty(assetObject);

                var result = UnityEditor.AssetDatabase.RenameAsset(relativePath, newFileName);

                if (result.IsNotNullOrEmpty())
                {
                    throw new InvalidOperationException(result);
                }
            }
        }

        /// <summary>
        ///     <para>Checks if an asset file can be moved from one folder to another. (Without actually moving the file).</para>
        /// </summary>
        /// <param name="oldPath">The path where the asset currently resides.</param>
        /// <param name="newPath">The path which the asset should be moved to.</param>
        /// <returns>
        ///     <para>An empty string if the asset can be moved, otherwise an error message.</para>
        /// </returns>
        public static string ValidateMoveAsset(string oldPath, string newPath)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_ValidateMoveAsset.Auto())
            {
                return UnityEditor.AssetDatabase.ValidateMoveAsset(oldPath, newPath);
            }
        }

        /// <summary>
        ///     <para>Writes the import settings to disk.</para>
        /// </summary>
        /// <param name="path"></param>
        public static bool WriteImportSettingsIfDirty(string path)
        {
            ThrowIfInvalidState();
            using var scope = APPASERIALIZE.OnAssetEditing();
            using (_PRF_WriteImportSettingsIfDirty.Auto())
            {
                var relativePath = path.ToRelativePath();
                return UnityEditor.AssetDatabase.WriteImportSettingsIfDirty(relativePath);
            }
        }

        #region Profiling

        private static readonly ProfilerMarker _PRF_UpdateAssetName =
            new ProfilerMarker(_PRF_PFX + nameof(UpdateAssetName));

        private static readonly ProfilerMarker _PRF_LoadAllObjectsInPrefabAsset =
            new ProfilerMarker(_PRF_PFX + nameof(LoadAllObjectsInPrefabAsset));

        private static readonly ProfilerMarker _PRF_CanOpenForEdit = new(_PRF_PFX + nameof(CanOpenForEdit));
        private static readonly ProfilerMarker _PRF_IsOpenForEdit = new(_PRF_PFX + nameof(IsOpenForEdit));
        private static readonly ProfilerMarker _PRF_MakeEditable = new(_PRF_PFX + nameof(MakeEditable));

        private static readonly ProfilerMarker _PRF_GetTextMetaDataPathFromAssetPath =
            new(_PRF_PFX + nameof(GetTextMetaDataPathFromAssetPath));

        private static readonly ProfilerMarker _PRF_FindAssets = new(_PRF_PFX + nameof(FindAssets));
        private static readonly ProfilerMarker _PRF_Contains = new(_PRF_PFX + nameof(Contains));
        private static readonly ProfilerMarker _PRF_CreateFolder = new(_PRF_PFX + nameof(CreateFolder));
        private static readonly ProfilerMarker _PRF_IsMainAsset = new(_PRF_PFX + nameof(IsMainAsset));
        private static readonly ProfilerMarker _PRF_IsSubAsset = new(_PRF_PFX + nameof(IsSubAsset));
        private static readonly ProfilerMarker _PRF_IsForeignAsset = new(_PRF_PFX + nameof(IsForeignAsset));
        private static readonly ProfilerMarker _PRF_IsNativeAsset = new(_PRF_PFX + nameof(IsNativeAsset));

        private static readonly ProfilerMarker _PRF_GetCurrentCacheServerIp =
            new(_PRF_PFX + nameof(GetCurrentCacheServerIp));

        private static readonly ProfilerMarker _PRF_GenerateUniqueAssetPath =
            new(_PRF_PFX + nameof(GenerateUniqueAssetPath));

        private static readonly ProfilerMarker _PRF_StartAssetEditing =
            new(_PRF_PFX + nameof(StartAssetEditing));

        private static readonly ProfilerMarker _PRF_StopAssetEditing =
            new(_PRF_PFX + nameof(StopAssetEditing));

        private static readonly ProfilerMarker _PRF_ReleaseCachedFileHandles =
            new(_PRF_PFX + nameof(ReleaseCachedFileHandles));

        private static readonly ProfilerMarker _PRF_ValidateMoveAsset =
            new(_PRF_PFX + nameof(ValidateMoveAsset));

        private static readonly ProfilerMarker _PRF_MoveAsset = new(_PRF_PFX + nameof(MoveAsset));
        private static readonly ProfilerMarker _PRF_ExtractAsset = new(_PRF_PFX + nameof(ExtractAsset));
        private static readonly ProfilerMarker _PRF_RenameAsset = new(_PRF_PFX + nameof(UpdateAssetName));

        private static readonly ProfilerMarker _PRF_MoveAssetToTrash =
            new(_PRF_PFX + nameof(MoveAssetToTrash));

        private static readonly ProfilerMarker _PRF_MoveAssetsToTrash =
            new(_PRF_PFX + nameof(MoveAssetsToTrash));

        private static readonly ProfilerMarker _PRF_DeleteAsset = new(_PRF_PFX + nameof(DeleteAsset));
        private static readonly ProfilerMarker _PRF_DeleteAssets = new(_PRF_PFX + nameof(DeleteAssets));
        private static readonly ProfilerMarker _PRF_ImportAsset = new(_PRF_PFX + nameof(ImportAsset));
        private static readonly ProfilerMarker _PRF_CopyAsset = new(_PRF_PFX + nameof(CopyAsset));

        private static readonly ProfilerMarker _PRF_WriteImportSettingsIfDirty =
            new(_PRF_PFX + nameof(WriteImportSettingsIfDirty));

        private static readonly ProfilerMarker _PRF_GetSubFolders = new(_PRF_PFX + nameof(GetSubFolders));
        private static readonly ProfilerMarker _PRF_IsValidFolder = new(_PRF_PFX + nameof(IsValidFolder));
        private static readonly ProfilerMarker _PRF_CreateAsset = new(_PRF_PFX + nameof(CreateAsset));

        private static readonly ProfilerMarker _PRF_AddObjectToAsset =
            new(_PRF_PFX + nameof(AddObjectToAsset));

        private static readonly ProfilerMarker _PRF_SetMainObject = new(_PRF_PFX + nameof(SetMainObject));
        private static readonly ProfilerMarker _PRF_GetAssetPath = new(_PRF_PFX + nameof(GetAssetPath));

        private static readonly ProfilerMarker _PRF_GetAssetOrScenePath =
            new(_PRF_PFX + nameof(GetAssetOrScenePath));

        private static readonly ProfilerMarker _PRF_GetTextMetaFilePathFromAssetPath =
            new(_PRF_PFX + nameof(GetTextMetaFilePathFromAssetPath));

        private static readonly ProfilerMarker _PRF_GetAssetPathFromTextMetaFilePath =
            new(_PRF_PFX + nameof(GetAssetPathFromTextMetaFilePath));

        private static readonly ProfilerMarker _PRF_LoadAssetAtPath = new(_PRF_PFX + nameof(LoadAssetAtPath));

        private static readonly ProfilerMarker _PRF_LoadMainAssetAtPath =
            new(_PRF_PFX + nameof(LoadMainAssetAtPath));

        private static readonly ProfilerMarker _PRF_GetMainAssetTypeAtPath =
            new(_PRF_PFX + nameof(GetMainAssetTypeAtPath));

        private static readonly ProfilerMarker _PRF_GetTypeFromPathAndFileID =
            new(_PRF_PFX + nameof(GetTypeFromPathAndFileID));

        private static readonly ProfilerMarker _PRF_IsMainAssetAtPathLoaded =
            new(_PRF_PFX + nameof(IsMainAssetAtPathLoaded));

        private static readonly ProfilerMarker _PRF_LoadAllAssetRepresentationsAtPath =
            new(_PRF_PFX + nameof(LoadAllAssetRepresentationsAtPath));

        private static readonly ProfilerMarker _PRF_LoadAllAssetsAtPath =
            new(_PRF_PFX + nameof(LoadAllAssetsAtPath));

        private static readonly ProfilerMarker _PRF_Refresh = new(_PRF_PFX + nameof(Refresh));

        private static readonly ProfilerMarker _PRF_CanOpenAssetInEditor =
            new(_PRF_PFX + nameof(CanOpenAssetInEditor));

        private static readonly ProfilerMarker _PRF_OpenAsset = new(_PRF_PFX + nameof(OpenAsset));
        private static readonly ProfilerMarker _PRF_GUIDToAssetPath = new(_PRF_PFX + nameof(GUIDToAssetPath));

        private static readonly ProfilerMarker _PRF_GUIDFromAssetPath =
            new(_PRF_PFX + nameof(GUIDFromAssetPath));

        private static readonly ProfilerMarker _PRF_AssetPathToGUID = new(_PRF_PFX + nameof(AssetPathToGUID));

        private static readonly ProfilerMarker _PRF_GetAssetDependencyHash =
            new(_PRF_PFX + nameof(GetAssetDependencyHash));

        private static readonly ProfilerMarker _PRF_SaveAssets = new(_PRF_PFX + nameof(SaveAssets));

        private static readonly ProfilerMarker _PRF_SaveAssetIfDirty =
            new(_PRF_PFX + nameof(SaveAssetIfDirty));

        private static readonly ProfilerMarker _PRF_GetCachedIcon = new(_PRF_PFX + nameof(GetCachedIcon));
        private static readonly ProfilerMarker _PRF_SetLabels = new(_PRF_PFX + nameof(SetLabels));
        private static readonly ProfilerMarker _PRF_GetLabels = new(_PRF_PFX + nameof(GetLabels));
        private static readonly ProfilerMarker _PRF_ClearLabels = new(_PRF_PFX + nameof(ClearLabels));

        private static readonly ProfilerMarker _PRF_GetAllAssetBundleNames =
            new(_PRF_PFX + nameof(GetAllAssetBundleNames));

        private static readonly ProfilerMarker _PRF_GetUnusedAssetBundleNames =
            new(_PRF_PFX + nameof(GetUnusedAssetBundleNames));

        private static readonly ProfilerMarker _PRF_RemoveAssetBundleName =
            new(_PRF_PFX + nameof(RemoveAssetBundleName));

        private static readonly ProfilerMarker _PRF_RemoveUnusedAssetBundleNames =
            new(_PRF_PFX + nameof(RemoveUnusedAssetBundleNames));

        private static readonly ProfilerMarker _PRF_GetAssetPathsFromAssetBundleAndAssetName =
            new(_PRF_PFX + nameof(GetAssetPathsFromAssetBundleAndAssetName));

        private static readonly ProfilerMarker _PRF_GetAssetPathsFromAssetBundle =
            new(_PRF_PFX + nameof(GetAssetPathsFromAssetBundle));

        private static readonly ProfilerMarker _PRF_GetImplicitAssetBundleName =
            new(_PRF_PFX + nameof(GetImplicitAssetBundleName));

        private static readonly ProfilerMarker _PRF_GetImplicitAssetBundleVariantName =
            new(_PRF_PFX + nameof(GetImplicitAssetBundleVariantName));

        private static readonly ProfilerMarker _PRF_GetAssetBundleDependencies =
            new(_PRF_PFX + nameof(GetAssetBundleDependencies));

        private static readonly ProfilerMarker _PRF_GetDependencies = new(_PRF_PFX + nameof(GetDependencies));
        private static readonly ProfilerMarker _PRF_ExportPackage = new(_PRF_PFX + nameof(ExportPackage));

        private static readonly ProfilerMarker _PRF_IsMetaFileOpenForEdit =
            new(_PRF_PFX + nameof(IsMetaFileOpenForEdit));

        private static readonly ProfilerMarker _PRF_GetBuiltinExtraResource =
            new(_PRF_PFX + nameof(GetBuiltinExtraResource));

        private static readonly ProfilerMarker _PRF_ForceReserializeAssets =
            new(_PRF_PFX + nameof(ForceReserializeAssets));

        private static readonly ProfilerMarker _PRF_TryGetGUIDAndLocalFileIdentifier =
            new(_PRF_PFX + nameof(TryGetGUIDAndLocalFileIdentifier));

        private static readonly ProfilerMarker _PRF_RemoveObjectFromAsset =
            new(_PRF_PFX + nameof(RemoveObjectFromAsset));

        private static readonly ProfilerMarker _PRF_ImportPackage = new(_PRF_PFX + nameof(ImportPackage));

        private static readonly ProfilerMarker _PRF_DisallowAutoRefresh =
            new(_PRF_PFX + nameof(DisallowAutoRefresh));

        private static readonly ProfilerMarker _PRF_AllowAutoRefresh =
            new(_PRF_PFX + nameof(AllowAutoRefresh));

        private static readonly ProfilerMarker _PRF_ClearImporterOverride =
            new(_PRF_PFX + nameof(ClearImporterOverride));

        private static readonly ProfilerMarker _PRF_IsCacheServerEnabled =
            new(_PRF_PFX + nameof(IsCacheServerEnabled));

        private static readonly ProfilerMarker _PRF_SetImporterOverride =
            new(_PRF_PFX + nameof(SetImporterOverride));

        private static readonly ProfilerMarker _PRF_GetImporterOverride =
            new(_PRF_PFX + nameof(GetImporterOverride));

        private static readonly ProfilerMarker _PRF_GetAvailableImporterTypes =
            new(_PRF_PFX + nameof(GetAvailableImporterTypes));

        private static readonly ProfilerMarker _PRF_CanConnectToCacheServer =
            new(_PRF_PFX + nameof(CanConnectToCacheServer));

        private static readonly ProfilerMarker _PRF_RefreshSettings = new(_PRF_PFX + nameof(RefreshSettings));

        private static readonly ProfilerMarker _PRF_IsConnectedToCacheServer =
            new(_PRF_PFX + nameof(IsConnectedToCacheServer));

        private static readonly ProfilerMarker _PRF_ResetCacheServerReconnectTimer =
            new(_PRF_PFX + nameof(ResetCacheServerReconnectTimer));

        private static readonly ProfilerMarker _PRF_CloseCacheServerConnection =
            new(_PRF_PFX + nameof(CloseCacheServerConnection));

        private static readonly ProfilerMarker _PRF_GetCacheServerAddress =
            new(_PRF_PFX + nameof(GetCacheServerAddress));

        private static readonly ProfilerMarker _PRF_GetCacheServerPort =
            new(_PRF_PFX + nameof(GetCacheServerPort));

        private static readonly ProfilerMarker _PRF_GetCacheServerNamespacePrefix =
            new(_PRF_PFX + nameof(GetCacheServerNamespacePrefix));

        private static readonly ProfilerMarker _PRF_GetCacheServerEnableDownload =
            new(_PRF_PFX + nameof(GetCacheServerEnableDownload));

        private static readonly ProfilerMarker _PRF_GetCacheServerEnableUpload =
            new(_PRF_PFX + nameof(GetCacheServerEnableUpload));

        private static readonly ProfilerMarker _PRF_IsDirectoryMonitoringEnabled =
            new(_PRF_PFX + nameof(IsDirectoryMonitoringEnabled));

        private static readonly ProfilerMarker _PRF_RegisterCustomDependency =
            new(_PRF_PFX + nameof(RegisterCustomDependency));

        private static readonly ProfilerMarker _PRF_UnregisterCustomDependencyPrefixFilter =
            new(_PRF_PFX + nameof(UnregisterCustomDependencyPrefixFilter));

        private static readonly ProfilerMarker _PRF_IsAssetImportWorkerProcess =
            new(_PRF_PFX + nameof(IsAssetImportWorkerProcess));

        private static readonly ProfilerMarker _PRF_ForceToDesiredWorkerCount =
            new(_PRF_PFX + nameof(ForceToDesiredWorkerCount));

        private static readonly ProfilerMarker _PRF_GetAllProjectPaths =
            new(_PRF_PFX + nameof(GetAllProjectPaths));

        private static readonly ProfilerMarker _PRF_OpenAssetAtPath =
            new ProfilerMarker(_PRF_PFX + nameof(OpenAssetAtPath));

        private static readonly ProfilerMarker _PRF_ImportAndLoadAssetAtPath =
            new ProfilerMarker(_PRF_PFX + nameof(ImportAndLoadAssetAtPath));

        #endregion
    }
}

#endif
