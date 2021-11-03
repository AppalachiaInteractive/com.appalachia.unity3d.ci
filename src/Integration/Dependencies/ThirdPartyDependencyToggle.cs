using System.Collections;
using System.Collections.Generic;
using Appalachia.CI.Integration.Assets;
using Appalachia.CI.Integration.Core;
using Appalachia.CI.Integration.Packages;
using Appalachia.CI.Integration.Repositories;
using Appalachia.Utility.Extensions;
using Unity.EditorCoroutines.Editor;
using Unity.Profiling;
using UnityEditor.PackageManager;
using UnityEngine;

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
        private const string MENU_DRYR = PKG.Menu.Appalachia.Packages.Base + "Dry Run";
        private const string MENU_THIRD_PACK = PKG.Menu.Appalachia.Packages.Base + "Third Party/To Packages";

        private const string MENU_THIRD_REPO =
            PKG.Menu.Appalachia.Packages.Base + "Third Party/To Repository";

        private const string MENU_UNITY_PACK = PKG.Menu.Appalachia.Packages.Base + "Unity/To Packages";
        private const string MENU_UNITY_REPO = PKG.Menu.Appalachia.Packages.Base + "Unity/To Repository";

        #endregion

        private static bool _dryRun;
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

        [UnityEditor.MenuItem(MENU_APPA_PACK, false, priority = MENU_PRIORITY + 0)]
        private static void APPAToPackages()
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(ExecuteToPackages(_dryRun, _appalachiaSubset));
        }

        [UnityEditor.MenuItem(MENU_APPA_PACK, true, priority = MENU_PRIORITY + 0)]
        private static bool APPAToPackagesValidate()
        {
            if (_executing)
            {
                return false;
            }

            Check();
            UnityEditor.Menu.SetChecked(MENU_APPA_PACK, _appalachiaSubset.anyPackages);
            return _appalachiaSubset.anyRepositories;
        }

        [UnityEditor.MenuItem(MENU_APPA_REPO, false, priority = MENU_PRIORITY + 1)]
        private static void APPAToRepository()
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(ExecuteToRepository(_dryRun, _appalachiaSubset));
        }

        [UnityEditor.MenuItem(MENU_APPA_REPO, true, priority = MENU_PRIORITY + 1)]
        private static bool APPAToRepositoryValidate()
        {
            if (_executing)
            {
                return false;
            }

            Check();
            UnityEditor.Menu.SetChecked(MENU_APPA_REPO, _appalachiaSubset.anyRepositories);
            return _appalachiaSubset.anyPackages;
        }

        [UnityEditor.MenuItem(MENU_THIRD_PACK, false, priority = MENU_PRIORITY + 0)]
        private static void THIRDToPackages()
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(ExecuteToPackages(_dryRun, _thirdPartySubset));
        }

        [UnityEditor.MenuItem(MENU_THIRD_PACK, true, priority = MENU_PRIORITY + 0)]
        private static bool THIRDToPackagesValidate()
        {
            if (_executing)
            {
                return false;
            }

            Check();
            UnityEditor.Menu.SetChecked(MENU_THIRD_PACK, _thirdPartySubset.anyPackages);
            return _thirdPartySubset.anyRepositories;
        }

        [UnityEditor.MenuItem(MENU_THIRD_REPO, false, priority = MENU_PRIORITY + 1)]
        private static void THIRDToRepository()
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(ExecuteToRepository(_dryRun, _thirdPartySubset));
        }

        [UnityEditor.MenuItem(MENU_THIRD_REPO, true, priority = MENU_PRIORITY + 1)]
        private static bool THIRDToRepositoryValidate()
        {
            if (_executing)
            {
                return false;
            }

            Check();
            UnityEditor.Menu.SetChecked(MENU_THIRD_REPO, _thirdPartySubset.anyRepositories);
            return _thirdPartySubset.anyPackages;
        }

        [UnityEditor.MenuItem(MENU_DRYR, false, priority = MENU_PRIORITY - 1)]
        private static void ToggleDryRun()
        {
            _dryRun = !_dryRun;
        }

        [UnityEditor.MenuItem(MENU_DRYR, true, priority = MENU_PRIORITY - 1)]
        private static bool ToggleDryRunValidate()
        {
            UnityEditor.Menu.SetChecked(MENU_DRYR, _dryRun);

            if (_executing)
            {
                return false;
            }

            return true;
        }

        [UnityEditor.MenuItem(MENU_UNITY_PACK, false, priority = MENU_PRIORITY + 0)]
        private static void UNITYToPackages()
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(ExecuteToPackages(_dryRun, _unitySubset));
        }

        [UnityEditor.MenuItem(MENU_UNITY_PACK, true, priority = MENU_PRIORITY + 0)]
        private static bool UNITYToPackagesValidate()
        {
            if (_executing)
            {
                return false;
            }

            Check();
            UnityEditor.Menu.SetChecked(MENU_UNITY_PACK, _unitySubset.anyPackages);
            return _unitySubset.anyRepositories;
        }

        [UnityEditor.MenuItem(MENU_UNITY_REPO, false, priority = MENU_PRIORITY + 1)]
        private static void UNITYToRepository()
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(ExecuteToRepository(_dryRun, _unitySubset));
        }

        [UnityEditor.MenuItem(MENU_UNITY_REPO, true, priority = MENU_PRIORITY + 1)]
        private static bool UNITYToRepositoryValidate()
        {
            if (_executing)
            {
                return false;
            }

            Check();
            UnityEditor.Menu.SetChecked(MENU_UNITY_REPO, _unitySubset.anyRepositories);
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

                        Debug.Log($"{dependency.Value.state}: {dependency.Key}");
                    }
                }
            }
        }

        private static IEnumerator ExecuteToPackages(bool dryRun, DependencySubset subset)
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

                        var subEnum = repository.ConvertToPackage(false, false, dryRun);

                        using (_PRF_ExecuteToPackages.Suspend())
                        {
                            while (subEnum.MoveNext())
                            {
                                yield return subEnum.Current;
                            }
                        }

                        adds.Add(repository.NameAndVersion);
                    }

                    if (dryRun)
                    {
                        foreach (var add in adds)
                        {
                            Debug.Log($"PKGMANAGER: Adding [{add}]");
                        }
                    }
                    else
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
                }
                finally
                {
                    FinishExecution();
                }
            }
        }

        private static IEnumerator ExecuteToRepository(bool dryRun, DependencySubset subset)
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

                        var subEnum = package.ConvertToRepository(false, false, dryRun);

                        using (_PRF_ExecuteToRepository.Suspend())
                        {
                            while (subEnum.MoveNext())
                            {
                                yield return subEnum.Current;
                            }
                        }

                        removes.Add(package.Name);
                    }

                    if (dryRun)
                    {
                        foreach (var remove in removes)
                        {
                            Debug.Log($"PKGMANAGER: Removing [{remove}]");
                        }
                    }
                    else
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
                if (!_dryRun)
                {
                    AssetDatabaseManager.StopAssetEditing();
                    AssetDatabaseManager.SaveAssets();
                    AssetDatabaseManager.Refresh();
                }

                IntegrationMetadata.ClearAll<PackageMetadata>();
                IntegrationMetadata.ClearAll<RepositoryMetadata>();
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
                if (!_dryRun)
                {
                    AssetDatabaseManager.StartAssetEditing();
                }
            }
            catch
            {
                _executing = false;
                throw;
            }
        }

        #region Nested Types

        private enum DependencyState
        {
            AsRepository,
            AsPackage,
        }

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
    }
}
