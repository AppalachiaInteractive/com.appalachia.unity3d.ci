using System.Collections;
using System.Collections.Generic;
using Appalachia.CI.Integration.Assets;
using Appalachia.CI.Integration.Core;
using Appalachia.CI.Integration.Packages;
using Appalachia.CI.Integration.Repositories;
using Appalachia.Utility.Execution;
using Appalachia.Utility.Extensions;
using Unity.Profiling;
using UnityEditor.PackageManager;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Appalachia.CI.Integration.Dependencies
{
    public static class ThirdPartyDependencyToggle
    {
        #region Profiling And Tracing Markers

        private const string _PRF_PFX = nameof(ThirdPartyDependencyToggle) + ".";
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

        [UnityEditor.MenuItem(MENU_APPA_PACK, false, priority = MENU_APPA_PRIORITY + 0)]
        private static void APPAToPackages()
        {
            ExecuteToPackages(_appalachiaSubset).AsSafe(nameof(APPAToPackages)).ExecuteAsEditorCoroutine();
        }

        [UnityEditor.MenuItem(MENU_APPA_PACK, true, priority = MENU_APPA_PRIORITY + 0)]
        private static bool APPAToPackagesValidate()
        {
            if (_executing)
            {
                return false;
            }

            Check();
            return _appalachiaSubset.anyRepositories;
        }

        [UnityEditor.MenuItem(MENU_APPA_REPO, false, priority = MENU_APPA_PRIORITY + 1)]
        private static void APPAToRepository()
        {
            ExecuteToRepository(_appalachiaSubset)
               .AsSafe(nameof(APPAToRepository))
               .ExecuteAsEditorCoroutine();
        }

        [UnityEditor.MenuItem(MENU_APPA_REPO, true, priority = MENU_APPA_PRIORITY + 1)]
        private static bool APPAToRepositoryValidate()
        {
            if (_executing)
            {
                return false;
            }

            Check();
            return _appalachiaSubset.anyPackages;
        }

        [UnityEditor.MenuItem(MENU_THIRD_PACK, false, priority = MENU_THIRD_PRIORITY + 0)]
        private static void THIRDToPackages()
        {
            ExecuteToPackages(_thirdPartySubset).AsSafe(nameof(THIRDToPackages)).ExecuteAsEditorCoroutine();
        }

        [UnityEditor.MenuItem(MENU_THIRD_PACK, true, priority = MENU_THIRD_PRIORITY + 0)]
        private static bool THIRDToPackagesValidate()
        {
            if (_executing)
            {
                return false;
            }

            Check();
            return _thirdPartySubset.anyRepositories;
        }

        [UnityEditor.MenuItem(MENU_THIRD_REPO, false, priority = MENU_THIRD_PRIORITY + 1)]
        private static void THIRDToRepository()
        {
            ExecuteToRepository(_thirdPartySubset)
               .AsSafe(nameof(THIRDToRepository))
               .ExecuteAsEditorCoroutine();
        }

        [UnityEditor.MenuItem(MENU_THIRD_REPO, true, priority = MENU_THIRD_PRIORITY + 1)]
        private static bool THIRDToRepositoryValidate()
        {
            if (_executing)
            {
                return false;
            }

            Check();
            return _thirdPartySubset.anyPackages;
        }

        [UnityEditor.MenuItem(MENU_ENAB, false, priority = MENU_PRIORITY - 1)]
        private static void ToggleEnabled()
        {
            _enabled = !_enabled;
        }

        [UnityEditor.MenuItem(MENU_ENAB, true, priority = MENU_PRIORITY - 1)]
        private static bool ToggleEnabledValidated()
        {
            UnityEditor.Menu.SetChecked(MENU_ENAB, _enabled);

            if (_executing)
            {
                return false;
            }

            return true;
        }

        [UnityEditor.MenuItem(MENU_UNITY_PACK, false, priority = MENU_UNITY_PRIORITY + 0)]
        private static void UNITYToPackages()
        {
            ExecuteToPackages(_unitySubset).AsSafe(nameof(UNITYToPackages)).ExecuteAsEditorCoroutine();
        }

        [UnityEditor.MenuItem(MENU_UNITY_PACK, true, priority = MENU_UNITY_PRIORITY + 0)]
        private static bool UNITYToPackagesValidate()
        {
            if (_executing)
            {
                return false;
            }

            Check();
            return _unitySubset.anyRepositories;
        }

        [UnityEditor.MenuItem(MENU_UNITY_REPO, false, priority = MENU_UNITY_PRIORITY + 1)]
        private static void UNITYToRepository()
        {
            ExecuteToRepository(_unitySubset).AsSafe(nameof(UNITYToRepository)).ExecuteAsEditorCoroutine();
        }

        [UnityEditor.MenuItem(MENU_UNITY_REPO, true, priority = MENU_UNITY_PRIORITY + 1)]
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

                UnityEditor.Menu.SetChecked(MENU_APPA_PACK,  _appalachiaSubset.anyPackages);
                UnityEditor.Menu.SetChecked(MENU_APPA_REPO,  _appalachiaSubset.anyRepositories);
                UnityEditor.Menu.SetChecked(MENU_THIRD_PACK, _thirdPartySubset.anyPackages);
                UnityEditor.Menu.SetChecked(MENU_THIRD_REPO, _thirdPartySubset.anyRepositories);
                UnityEditor.Menu.SetChecked(MENU_UNITY_PACK, _unitySubset.anyPackages);
                UnityEditor.Menu.SetChecked(MENU_UNITY_REPO, _unitySubset.anyRepositories);
            }
        }

        private static IEnumerator ExecuteToPackages(DependencySubset subset)
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

                        var subEnum = repository.ConvertToPackage(false, false, !_enabled);

                        using (_PRF_ExecuteToPackages.Suspend())
                        {
                            while (subEnum.MoveNext())
                            {
                                yield return subEnum.Current;
                            }
                        }

                        adds.Add(repository.NameAndVersion);
                    }

                    if (_enabled)
                    {
                        var request = Client.AddAndRemove(adds.ToArray());

                        using (_PRF_ExecuteToPackages.Suspend())
                        {
                            while (!request.IsCompleted)
                            {
                                yield return null;
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

        private static IEnumerator ExecuteToRepository(DependencySubset subset)
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

                        var subEnum = package.ConvertToRepository(false, !_enabled);

                        using (_PRF_ExecuteToRepository.Suspend())
                        {
                            while (subEnum.MoveNext())
                            {
                                yield return subEnum.Current;
                            }
                        }

                        removes.Add(package.Name);
                    }

                    if (_enabled)
                    {
                        var request = Client.AddAndRemove(packagesToRemove: removes.ToArray());

                        using (_PRF_ExecuteToRepository.Suspend())
                        {
                            while (!request.IsCompleted)
                            {
                                yield return null;
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

                    if (EditorSceneManager.GetActiveScene().name != "Untitled")
                    {
                        UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();

                        UnityEditor.SceneManagement.EditorSceneManager.NewScene(
                            NewSceneSetup.EmptyScene,
                            NewSceneMode.Single
                        );
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
