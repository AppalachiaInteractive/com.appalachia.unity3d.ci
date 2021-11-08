using System.Collections.Generic;
using System.IO;
using System.Linq;
using Appalachia.CI.Integration.Assets;
using Appalachia.CI.Integration.Core;
using Appalachia.CI.Integration.Core.Shell;
using Appalachia.CI.Integration.FileSystem;
using Appalachia.CI.Integration.Packages;
using Appalachia.CI.Integration.Repositories;
using Appalachia.Utility.Enums;
using Appalachia.Utility.Extensions;
using Unity.Profiling;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Appalachia.CI.Integration.Development
{
    public static class DevelopmentPackageSwapper
    {
        #region Profiling And Tracing Markers

        private const string _PRF_PFX = nameof(DevelopmentPackageSwapper) + ".";

        private static readonly ProfilerMarker _PRF_ConvertToPackage =
            new ProfilerMarker(_PRF_PFX + nameof(ConvertToPackage));

        private static readonly ProfilerMarker _PRF_Check = new ProfilerMarker(_PRF_PFX + nameof(Check));

        private static readonly ProfilerMarker _PRF_ExecuteToRepository =
            new ProfilerMarker(_PRF_PFX + nameof(ExecuteToRepository));

        private static readonly ProfilerMarker _PRF_ExecuteToPackages =
            new ProfilerMarker(_PRF_PFX + nameof(ExecuteToPackages));

        #endregion

        #region Constants and Static Readonly

        private const int MENU_APPA_PRIORITY = MENU_PRIORITY + 1;

        private const int MENU_PRIORITY = PKG.Menu.Appalachia.Packages.Priority;
        private const int MENU_THIRD_PRIORITY = MENU_PRIORITY + 3;
        private const int MENU_UNITY_PRIORITY = MENU_PRIORITY + 2;
        private const string MENU_APPA_PACK = PKG.Menu.Appalachia.Packages.Base + "Appalachia/To Packages";
        private const string MENU_APPA_REPO = PKG.Menu.Appalachia.Packages.Base + "Appalachia/To Repository";
        private const string MENU_ENAB = PKG.Menu.Appalachia.Packages.Base + "Enabled";
        private const string MENU_THIRD_PACK = PKG.Menu.Appalachia.Packages.Base + "Third Party/To Packages";

        private const string MENU_THIRD_REPO =
            PKG.Menu.Appalachia.Packages.Base + "Third Party/To Repository";

        private const string MENU_UNITY_PACK = PKG.Menu.Appalachia.Packages.Base + "Unity/To Packages";
        private const string MENU_UNITY_REPO = PKG.Menu.Appalachia.Packages.Base + "Unity/To Repository";

        #endregion

        //private static bool _dryRun;
        private static bool _enabled;
        private static bool _executing;

        private static DependencySubset _appalachiaSubset;
        private static DependencySubset _thirdPartySubset;
        private static DependencySubset _unitySubset;

        private static IEnumerable<DependencySubset> _subsets
        {
            get
            {
                yield return _appalachiaSubset;
                yield return _thirdPartySubset;
                yield return _unitySubset;
            }
        }

        #region Menu Items

        [MenuItem(MENU_APPA_PACK, false, priority = MENU_APPA_PRIORITY + 0)]
        private static void APPAToPackages()
        {
            ExecuteToPackages(_appalachiaSubset);
        }

        [MenuItem(MENU_APPA_PACK, true, priority = MENU_APPA_PRIORITY + 0)]
        private static bool APPAToPackagesValidate()
        {
            if (_executing)
            {
                return false;
            }

            Check();
            return _appalachiaSubset.anyRepositories;
        }

        [MenuItem(MENU_APPA_REPO, false, priority = MENU_APPA_PRIORITY + 1)]
        private static void APPAToRepository()
        {
            ExecuteToRepository(_appalachiaSubset);
        }

        [MenuItem(MENU_APPA_REPO, true, priority = MENU_APPA_PRIORITY + 1)]
        private static bool APPAToRepositoryValidate()
        {
            if (_executing)
            {
                return false;
            }

            Check();
            return _appalachiaSubset.anyPackages;
        }

        [MenuItem(MENU_THIRD_PACK, false, priority = MENU_THIRD_PRIORITY + 0)]
        private static void THIRDToPackages()
        {
            ExecuteToPackages(_thirdPartySubset);
        }

        [MenuItem(MENU_THIRD_PACK, true, priority = MENU_THIRD_PRIORITY + 0)]
        private static bool THIRDToPackagesValidate()
        {
            if (_executing)
            {
                return false;
            }

            Check();
            return _thirdPartySubset.anyRepositories;
        }

        [MenuItem(MENU_THIRD_REPO, false, priority = MENU_THIRD_PRIORITY + 1)]
        private static void THIRDToRepository()
        {
            ExecuteToRepository(_thirdPartySubset);
        }

        [MenuItem(MENU_THIRD_REPO, true, priority = MENU_THIRD_PRIORITY + 1)]
        private static bool THIRDToRepositoryValidate()
        {
            if (_executing)
            {
                return false;
            }

            Check();
            return _thirdPartySubset.anyPackages;
        }

        [MenuItem(MENU_ENAB, false, priority = MENU_PRIORITY - 1)]
        private static void ToggleEnabled()
        {
            _enabled = !_enabled;
        }

        [MenuItem(MENU_ENAB, true, priority = MENU_PRIORITY - 1)]
        private static bool ToggleEnabledValidated()
        {
            Menu.SetChecked(MENU_ENAB, _enabled);

            if (_executing)
            {
                return false;
            }

            return true;
        }

        [MenuItem(MENU_UNITY_PACK, false, priority = MENU_UNITY_PRIORITY + 0)]
        private static void UNITYToPackages()
        {
            ExecuteToPackages(_unitySubset);
        }

        [MenuItem(MENU_UNITY_PACK, true, priority = MENU_UNITY_PRIORITY + 0)]
        private static bool UNITYToPackagesValidate()
        {
            if (_executing)
            {
                return false;
            }

            Check();
            return _unitySubset.anyRepositories;
        }

        [MenuItem(MENU_UNITY_REPO, false, priority = MENU_UNITY_PRIORITY + 1)]
        private static void UNITYToRepository()
        {
            ExecuteToRepository(_unitySubset);
        }

        [MenuItem(MENU_UNITY_REPO, true, priority = MENU_UNITY_PRIORITY + 1)]
        private static bool UNITYToRepositoryValidate()
        {
            if (_executing)
            {
                return false;
            }

            Check();
            return _unitySubset.anyPackages;
        }

        #endregion

        public static void ConvertToPackage(
            RepositoryMetadata repo,
            PackageSwapOptions options = PackageSwapOptions.DryRun)
        {
            using (_PRF_ConvertToPackage.Auto())
            {
                var refreshAssets = options.HasFlag(PackageSwapOptions.RefreshAssets);
                var executeClient = options.HasFlag(PackageSwapOptions.ExecutePackageClient);
                var dryRun = options.HasFlag(PackageSwapOptions.DryRun);

                if (repo.IsPackage || repo.Path.EndsWith("~") || repo.Path.StartsWith("Package"))
                {
                    return;
                }

                if (!AppaDirectory.Exists(repo.Path))
                {
                    return;
                }

                Debug.Log(
                    $"Refreshing assets before converting [{repo.Name}] from a repository to a package."
                );

                if (refreshAssets)
                {
                    AssetDatabaseManager.Refresh();
                }

                Debug.Log($"Converting [{repo.Name}] from a repository to a package.");

                foreach (var dependency in repo.dependencies)
                {
                    if (dependency.repository.IsAppalachiaManaged)
                    {
                        var subOptions = options.UnsetFlag(PackageSwapOptions.RefreshAssets);

                        ConvertToPackage(dependency.repository, subOptions);
                    }
                }

                var root = repo.Path;
                var meta = root + ".meta";

                var newRoot = root + "~";

                if (dryRun)
                {
                    Debug.Log($"MOVEDIR: [{root}] to [{newRoot}]");
                    Debug.Log($"DELETE: [{meta}]");
                }
                else
                {
                    AppaDirectory.Move(root, newRoot);
                    AppaFile.Delete(meta);
                }

                if (executeClient)
                {
                    if (dryRun)
                    {
                        Debug.Log($"PKGMANAGER: Adding [{repo.NameAndVersion}]");
                    }
                    else
                    {
                        var addition = Client.Add(repo.NameAndVersion);

                        while (!addition.IsCompleted)
                        {
                        }
                    }
                }
            }
        }

        public static void ConvertToRepository(
            PackageMetadata package,
            PackageSwapOptions options = PackageSwapOptions.DryRun)
        {
            Debug.Log($"Converting [{package.Name}] from a package to a repository.");

            //var refreshAssets = options.HasFlag(PackageSwapOptions.RefreshAssets);
            var executeClient = options.HasFlag(PackageSwapOptions.ExecutePackageClient);
            var dryRun = options.HasFlag(PackageSwapOptions.DryRun);

            package.PopulateDependents();

            foreach (var dependent in package.dependents)
            {
                if (dependent.IsAppalachiaManaged)
                {
                    ConvertToRepository(dependent, options);
                }
            }

            var repo = package.packageInfo.repository;

            string directoryRoot;
            string directoryName;

            if (package.IsThirdParty || package.IsUnity)
            {
                directoryName = package.packageInfo.name.Split(".").Last();
                directoryRoot = "Assets/Third-Party/";
            }
            else
            {
                directoryName = package.Name;
                directoryRoot = "Assets/";
            }

            var existingDirectory = AppaDirectory.GetDirectories(
                                                      directoryRoot,
                                                      directoryName + "*",
                                                      SearchOption.TopDirectoryOnly
                                                  )
                                                 .Select(d => new AppaDirectoryInfo(d))
                                                 .FirstOrDefault();

            if (existingDirectory != null)
            {
                var existingDirectoryPath = existingDirectory.RelativePath;

                if (existingDirectoryPath.EndsWith("~"))
                {
                    var dir = new AppaDirectoryInfo(
                        existingDirectoryPath.Substring(0, existingDirectoryPath.Length - 1)
                    );

                    var hideDirectory = existingDirectory;

                    if (dryRun)
                    {
                        Debug.Log($"MOVEDIR: [{hideDirectory.RelativePath}] to [{dir.RelativePath}]");
                    }
                    else
                    {
                        AppaDirectory.Move(hideDirectory.RelativePath, dir.RelativePath);
                    }
                }
            }
            else
            {
                var result = new ShellResult();
                var directoryPath = AppaPath.Combine(directoryRoot, directoryName);
                var command = $"git clone \"{repo.url.Replace("git+", "")}\" \"{directoryPath}\"";
                var workingDirectory = Application.dataPath;

                if (dryRun)
                {
                    Debug.Log($"COMMAND: [{command}] WORKDIR: [{workingDirectory}]");
                }
                else
                {
                    var execution = SystemShell.Instance.Execute(command, workingDirectory, false, result);

                    while (execution.MoveNext())
                    {
                    }
                }
            }

            if (executeClient)
            {
                if (dryRun)
                {
                    Debug.Log($"PKGMANAGER: Removing [{package.Name}]");
                }
                else
                {
                    var removal = Client.Remove(package.Name);

                    while (!removal.IsCompleted)
                    {
                    }
                }
            }
        }

        private static void Check()
        {
            using (_PRF_Check.Auto())
            {
                _appalachiaSubset ??= new DependencySubset();
                _thirdPartySubset ??= new DependencySubset();
                _unitySubset ??= new DependencySubset();

                _appalachiaSubset.Reset();
                _thirdPartySubset.Reset();
                _unitySubset.Reset();

                var packages = PackageMetadata.FindAll();
                var repositories = RepositoryMetadata.FindAll();

                foreach (var package in packages)
                {
                    if (!package.IsAppalachiaManaged)
                    {
                        continue;
                    }

                    var subset = package.IsThirdParty
                        ? _thirdPartySubset
                        : package.IsUnity
                            ? _unitySubset
                            : _appalachiaSubset;

                    subset.anyPackages = true;
                    subset.dependencies.Add(package.Name, new DependencyMetadata(package));
                }

                foreach (var repository in repositories)
                {
                    if (!repository.IsAppalachiaManaged)
                    {
                        continue;
                    }

                    var subset = repository.IsThirdParty
                        ? _thirdPartySubset
                        : repository.IsUnity
                            ? _unitySubset
                            : _appalachiaSubset;

                    subset.anyRepositories = true;
                    subset.dependencies.AddOrExecute(
                        repository.Name,
                        () => new DependencyMetadata(repository),
                        d => { d.repository = repository; }
                    );
                }

                foreach (var subset in _subsets)
                {
                    foreach (var dependency in subset.dependencies)
                    {
                        dependency.Value.state = DependencyState.AsPackage;

                        if (dependency.Value.package == null)
                        {
                            dependency.Value.state = DependencyState.AsRepository;
                        }

                        //Debug.Log($"{dependency.Value.state}: {dependency.Key}");
                    }
                }

                Menu.SetChecked(MENU_APPA_PACK,  _appalachiaSubset.anyPackages);
                Menu.SetChecked(MENU_APPA_REPO,  _appalachiaSubset.anyRepositories);
                Menu.SetChecked(MENU_THIRD_PACK, _thirdPartySubset.anyPackages);
                Menu.SetChecked(MENU_THIRD_REPO, _thirdPartySubset.anyRepositories);
                Menu.SetChecked(MENU_UNITY_PACK, _unitySubset.anyPackages);
                Menu.SetChecked(MENU_UNITY_REPO, _unitySubset.anyRepositories);
            }
        }

        private static void ExecuteToPackages(DependencySubset subset)
        {
            using (_PRF_ExecuteToPackages.Auto())
            {
                try
                {
                    StartExecution();

                    var adds = new List<string>();

                    foreach (var dependencyPair in subset.dependencies)
                    {
                        var dependency = dependencyPair.Value;

                        if (dependency.state != DependencyState.AsRepository)
                        {
                            continue;
                        }

                        var repository = dependency.repository;

                        var options = _enabled ? PackageSwapOptions.None : PackageSwapOptions.DryRun;

                        ConvertToPackage(repository, options);

                        adds.Add(repository.NameAndVersion);
                    }

                    if (_enabled)
                    {
                        var request = Client.AddAndRemove(adds.ToArray());

                        using (_PRF_ExecuteToPackages.Suspend())
                        {
                            while (!request.IsCompleted)
                            {
                            }
                        }
                    }
                    else
                    {
                        foreach (var add in adds)
                        {
                            Debug.Log($"PKGMANAGER: Adding [{add}]");
                        }
                    }
                }
                finally
                {
                    FinishExecution();
                }
            }
        }

        private static void ExecuteToRepository(DependencySubset subset)
        {
            using (_PRF_ExecuteToRepository.Auto())
            {
                try
                {
                    StartExecution();

                    var removes = new List<string>();

                    foreach (var dependencyPair in subset.dependencies)
                    {
                        var dependency = dependencyPair.Value;

                        if (dependency.state != DependencyState.AsPackage)
                        {
                            continue;
                        }

                        var package = dependency.package;

                        var options = _enabled ? PackageSwapOptions.None : PackageSwapOptions.DryRun;

                        ConvertToRepository(package, options);

                        removes.Add(package.Name);
                    }

                    if (_enabled)
                    {
                        var request = Client.AddAndRemove(packagesToRemove: removes.ToArray());

                        using (_PRF_ExecuteToRepository.Suspend())
                        {
                            while (!request.IsCompleted)
                            {
                            }
                        }
                    }
                    else
                    {
                        foreach (var remove in removes)
                        {
                            Debug.Log($"PKGMANAGER: Removing [{remove}]");
                        }
                    }
                }
                finally
                {
                    FinishExecution();
                }
            }
        }

        private static void FinishExecution()
        {
            try
            {
                if (_enabled)
                {
                    AssetDatabaseManager.SaveAssets();
                    AssetDatabaseManager.Refresh();

                    IntegrationMetadata.ClearAll<PackageMetadata>();
                    IntegrationMetadata.ClearAll<RepositoryMetadata>();
                }
                else
                {
                    Debug.Log("[RESUME ASSET IMPORT]");
                    Debug.Log("[SAVE ASSETS]");
                    Debug.Log("[REFRESH ASSETS]");
                }
            }
            finally
            {
                _executing = false;
            }
        }

        private static void StartExecution()
        {
            _executing = true;

            try
            {
                Check();

                if (_enabled)
                {
                    AssetDatabaseManager.SetSelection("Assets");

                    if (SceneManager.GetActiveScene().name != "Untitled")
                    {
                        EditorSceneManager.SaveOpenScenes();

                        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                    }

                    AssetDatabaseManager.SaveAssets();
                    AssetDatabaseManager.Refresh();
                }
                else
                {
                    Debug.Log("[CLEAR SELECTION]");
                    Debug.Log("[SUSPEND ASSET IMPORT]");
                }
            }
            catch
            {
                _executing = false;
                throw;
            }
        }

        #region Nested Types

        private class DependencyMetadata
        {
            public DependencyMetadata(PackageMetadata package)
            {
                this.package = package;
            }

            public DependencyMetadata(RepositoryMetadata repository)
            {
                this.repository = repository;
            }

            public DependencyState state;

            public PackageMetadata package;
            public RepositoryMetadata repository;
        }

        private class DependencySubset
        {
            public bool anyPackages;
            public bool anyRepositories;

            public Dictionary<string, DependencyMetadata> dependencies;

            public void Reset()
            {
                anyPackages = false;
                anyRepositories = false;
                dependencies ??= new Dictionary<string, DependencyMetadata>();
                dependencies.Clear();
            }
        }

        #endregion

        private enum DependencyState
        {
            AsRepository,
            AsPackage,
        }
    }
}
