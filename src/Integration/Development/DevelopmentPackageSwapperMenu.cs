using System.Collections.Generic;
using System.Linq;
using Appalachia.CI.Integration.Packages;
using Appalachia.CI.Integration.Repositories;
using Appalachia.Utility.Extensions;
using Unity.Profiling;
using UnityEditor;

namespace Appalachia.CI.Integration.Development
{
    public static class DevelopmentPackageSwapperMenu
    {
        #region Profiling And Tracing Markers

        private const string _PRF_PFX = nameof(DevelopmentPackageSwapperMenu) + ".";

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

                        //AppaLog.Info($"{dependency.Value.state}: {dependency.Key}");
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
                    if (_executing)
                    {
                        return;
                    }

                    _executing = true;

                    var targets = subset.dependencies
                                        .Where(p => p.Value.state == DependencyState.AsRepository)
                                        .Select(p => p.Value.repository);

                    DevelopmentPackageSwapper.ConvertToPackage(targets, PackageSwapOptions.SingleExecution);
                }
                finally
                {
                    _executing = false;
                }
            }
        }

        private static void ExecuteToRepository(DependencySubset subset)
        {
            using (_PRF_ExecuteToRepository.Auto())
            {
                try
                {
                    if (_executing)
                    {
                        return;
                    }

                    _executing = true;

                    var targets = subset.dependencies.Where(p => p.Value.state == DependencyState.AsPackage)
                                        .Select(p => p.Value.package);

                    DevelopmentPackageSwapper.ConvertToRepository(targets, PackageSwapOptions.SingleExecution);
                }
                finally
                {
                    _executing = false;
                }
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
