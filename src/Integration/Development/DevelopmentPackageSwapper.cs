using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Appalachia.CI.Integration.Assets;
using Appalachia.CI.Integration.Core;
using Appalachia.CI.Integration.FileSystem;
using Appalachia.CI.Integration.Packages;
using Appalachia.CI.Integration.Repositories;
using Appalachia.Utility.Enums;
using Appalachia.Utility.Logging;
using Unity.Profiling;
using UnityEditor.Compilation;
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

        private static readonly ProfilerMarker _PRF_ConvertToPackage_END =
            new ProfilerMarker(_PRF_PFX + nameof(EndConversionToPackage));

        private static readonly ProfilerMarker _PRF_ConvertToPackage_START =
            new ProfilerMarker(_PRF_PFX + nameof(StartConversionToPackage));

        private static readonly ProfilerMarker _PRF_ConvertToRepository_END =
            new ProfilerMarker(_PRF_PFX + nameof(EndConversionToRepository));

        private static readonly ProfilerMarker _PRF_ConvertToRepository_START =
            new ProfilerMarker(_PRF_PFX + nameof(StartConversionToRepository));

        private static readonly ProfilerMarker _PRF_ConvertToRepository =
            new ProfilerMarker(_PRF_PFX + nameof(ConvertToRepository));

        private static Dictionary<string, Action<object>> _convertToPackageEnd;
        private static Dictionary<string, Action<object>> _convertToRepositoryEnd;
        private static readonly ProfilerMarker _PRF_Convert = new ProfilerMarker(_PRF_PFX + nameof(Convert));

        #endregion

        public static void ConvertToPackage(PackageSwapOptions options, params RepositoryMetadata[] targets)
        {
            using (_PRF_ConvertToPackage.Auto())
            {
                Convert(
                    targets,
                    ref _convertToPackageEnd,
                    StartConversionToPackage,
                    EndConversionToPackage,
                    options
                );
            }
        }

        public static void ConvertToPackage(
            IEnumerable<RepositoryMetadata> targets,
            PackageSwapOptions options)
        {
            using (_PRF_ConvertToPackage.Auto())
            {
                Convert(
                    targets,
                    ref _convertToPackageEnd,
                    StartConversionToPackage,
                    EndConversionToPackage,
                    options
                );
            }
        }

        public static void ConvertToPackage(RepositoryMetadata target, PackageSwapOptions options)
        {
            using (_PRF_ConvertToPackage.Auto())
            {
                Convert(
                    new[] {target},
                    ref _convertToPackageEnd,
                    StartConversionToPackage,
                    EndConversionToPackage,
                    options
                );
            }
        }

        public static void ConvertToRepository(PackageSwapOptions options, params PackageMetadata[] targets)
        {
            using (_PRF_ConvertToRepository.Auto())
            {
                Convert(
                    targets,
                    ref _convertToRepositoryEnd,
                    StartConversionToRepository,
                    EndConversionToRepository,
                    options
                );
            }
        }

        public static void ConvertToRepository(
            IEnumerable<PackageMetadata> targets,
            PackageSwapOptions options)
        {
            using (_PRF_ConvertToRepository.Auto())
            {
                Convert(
                    targets,
                    ref _convertToRepositoryEnd,
                    StartConversionToRepository,
                    EndConversionToRepository,
                    options
                );
            }
        }

        public static void ConvertToRepository(PackageMetadata target, PackageSwapOptions options)
        {
            using (_PRF_ConvertToRepository.Auto())
            {
                Convert(
                    new[] {target},
                    ref _convertToRepositoryEnd,
                    StartConversionToRepository,
                    EndConversionToRepository,
                    options
                );
            }
        }

        private static void Convert<T>(
            IEnumerable<T> targets,
            ref Dictionary<string, Action<object>> _convertEndCache,
            Action<T, PackageSwapOptions> convertStart,
            Action<T, PackageSwapOptions> convertEnd,
            PackageSwapOptions options)
            where T : IntegrationMetadata<T>
        {
            using (_PRF_Convert.Auto())
            {
                var dryRun = options.HasFlag(PackageSwapOptions.DryRun);
                var refreshAssetsAtEnd = options.HasFlag(PackageSwapOptions.RefreshAssetsAtEnd);

                _convertEndCache ??= new Dictionary<string, Action<object>>();

                if (!dryRun)
                {
                    InitializeExecution(options);
                }

                foreach (var target in targets)
                {
                    convertStart(target, options);

                    if (_convertEndCache.ContainsKey(target.Name))
                    {
                        var oldOnCompilationFinished = _convertEndCache[target.Name];
                        _convertEndCache.Remove(target.Name);

                        CompilationPipeline.compilationFinished -= oldOnCompilationFinished;
                    }

                    void OnCompilationFinished(object _)
                    {
                        AppaLog.Trace("OnCompilationFinished called for ");
                        convertEnd(target, options);
                    }

                    _convertEndCache.Add(target.Name, OnCompilationFinished);

                    CompilationPipeline.compilationFinished += OnCompilationFinished;
                }

                CompilationPipeline.compilationFinished -= FinalizeExecutionOnCompilationFinished;

                if (!dryRun && refreshAssetsAtEnd)
                {
                    CompilationPipeline.compilationFinished += FinalizeExecutionOnCompilationFinished;
                }
            }
        }

        private static void EndConversionToPackage(RepositoryMetadata repo, PackageSwapOptions options)
        {
            using (_PRF_ConvertToPackage_END.Auto())
            {
                var dryRun = options.HasFlag(PackageSwapOptions.DryRun);
                var executePackageClient = options.HasFlag(PackageSwapOptions.ExecutePackageClient);

                if (executePackageClient)
                {
                    if (dryRun)
                    {
                        AppaLog.Info($"Adding [{repo.NameAndVersion}]");
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

        private static void EndConversionToRepository(PackageMetadata package, PackageSwapOptions options)
        {
            using (_PRF_ConvertToRepository_END.Auto())
            {
                AppaLog.Trace($"Finishing package [{package.Name}]");
                
                var dryRun = options.HasFlag(PackageSwapOptions.DryRun);
                var executePackageClient = options.HasFlag(PackageSwapOptions.ExecutePackageClient);

                if (executePackageClient)
                {
                    if (dryRun)
                    {
                        AppaLog.Info($"Removing [{package.Name}]");
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
        }

        private static void FinalizeExecutionOnCompilationFinished(object obj)
        {
            AssetDatabaseManager.SaveAssets();
            AssetDatabaseManager.Refresh();

            IntegrationMetadata.ClearAll<PackageMetadata>();
            IntegrationMetadata.ClearAll<RepositoryMetadata>();
        }

        private static PackageSwapOptions GetSuboptions(PackageSwapOptions options)
        {
            var subOptions = options.UnsetFlag(
                PackageSwapOptions.RefreshAssetsAfterDirectoryMove |
                PackageSwapOptions.RefreshAssetsAtEnd |
                PackageSwapOptions.RefreshAssetsAtStart
            );

            return subOptions;
        }

        private static void InitializeExecution(PackageSwapOptions options)
        {
            var refreshAssetsAtStart = options.HasFlag(PackageSwapOptions.RefreshAssetsAtStart);

            AssetDatabaseManager.SetSelection("Assets");

            var activeScene = SceneManager.GetActiveScene();
            
            if (!string.IsNullOrWhiteSpace(activeScene.name) && (activeScene.name != "Untitled"))
            {
                EditorSceneManager.SaveOpenScenes();
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            }

            if (refreshAssetsAtStart)
            {
                AssetDatabaseManager.SaveAssets();
                AssetDatabaseManager.Refresh();
            }
        }

        private static void StartConversionToPackage(RepositoryMetadata repo, PackageSwapOptions options)
        {
            using (_PRF_ConvertToPackage_START.Auto())
            {
                var dryRun = options.HasFlag(PackageSwapOptions.DryRun);
                var refreshAssetsAfterDirectoryMove =
                    options.HasFlag(PackageSwapOptions.RefreshAssetsAfterDirectoryMove);

                if (repo.IsPackage || repo.Path.EndsWith("~") || repo.Path.StartsWith("Package"))
                {
                    return;
                }

                if (!AppaDirectory.Exists(repo.Path))
                {
                    return;
                }

                AppaLog.Info(
                    $"Refreshing assets before converting [{repo.Name}] from a repository to a package."
                );

                AppaLog.Info($"Converting [{repo.Name}] from a repository to a package.");

                foreach (var dependency in repo.dependencies)
                {
                    if (dependency.repository.IsAppalachiaManaged)
                    {
                        var subOptions = GetSuboptions(options);

                        ConvertToPackage(dependency.repository, subOptions);
                    }
                }

                var root = repo.Path;
                var meta = root + ".meta";

                var newRoot = root + "~";

                if (dryRun)
                {
                    AppaLog.Info($"MOVEDIR: [{root}] to [{newRoot}]");
                    AppaLog.Info($"DELETE: [{meta}]");
                }
                else
                {
                    AppaDirectory.Move(root, newRoot);
                    AppaFile.Delete(meta);
                }

                if (refreshAssetsAfterDirectoryMove)
                {
                    AssetDatabaseManager.Refresh();
                }
            }
        }

        private static void StartConversionToRepository(PackageMetadata package, PackageSwapOptions options)
        {
            using (_PRF_ConvertToRepository_START.Auto())
            {
                AppaLog.Info($"Converting [{package.Name}] from a package to a repository.");

                var dryRun = options.HasFlag(PackageSwapOptions.DryRun);
                var refreshAssetsAfterDirectoryMove =
                    options.HasFlag(PackageSwapOptions.RefreshAssetsAfterDirectoryMove);

                package.PopulateDependents();

                foreach (var dependent in package.dependents)
                {
                    if (dependent.IsAppalachiaManaged)
                    {
                        var subOptions = GetSuboptions(options);

                        ConvertToRepository(dependent, subOptions);
                    }
                }

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
                            AppaLog.Info(
                                $"MOVEDIR: [{hideDirectory.RelativePath}] to [{dir.RelativePath}]"
                            );
                        }
                        else
                        {
                            AppaDirectory.Move(hideDirectory.RelativePath, dir.RelativePath);

                            if (refreshAssetsAfterDirectoryMove)
                            {
                                AssetDatabaseManager.Refresh();
                            }
                        }
                    }
                }
                else
                {
                    throw new NotImplementedException("Not tested yet!");
                    /*var result = new ShellResult();
                    var directoryPath = AppaPath.Combine(directoryRoot, directoryName);
                    var command = $"git clone \"{repo.url.Replace("git+", "")}\" \"{directoryPath}\"";
                    var workingDirectory = Application.dataPath;

                    if (dryRun)
                    {
                        AppaLog.Info($"COMMAND: [{command}] WORKDIR: [{workingDirectory}]");
                    }
                    else
                    {
                        var execution = SystemShell.Instance.Execute(
                            command,
                            workingDirectory,
                            false,
                            result
                        );

                        while (execution.MoveNext())
                        {
                        }
                    }*/
                }
            }
        }
    }
}
