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
            new ProfilerMarker(_PRF_PFX + nameof(BeginConversionToPackage));

        private static readonly ProfilerMarker _PRF_ConvertToRepository_END =
            new ProfilerMarker(_PRF_PFX + nameof(EndConversionToRepository));

        private static readonly ProfilerMarker _PRF_ConvertToRepository_START =
            new ProfilerMarker(_PRF_PFX + nameof(BeginConversionToRepository));

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
                    BeginConversionToPackage,
                    EndConversionToPackage,
                    options
                );
                
                AssetDatabaseManager.Refresh();
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
                    BeginConversionToPackage,
                    EndConversionToPackage,
                    options
                );
                
                AssetDatabaseManager.Refresh();
            }
        }

        public static void ConvertToPackage(RepositoryMetadata target, PackageSwapOptions options)
        {
            using (_PRF_ConvertToPackage.Auto())
            {
                Convert(
                    new[] {target},
                    ref _convertToPackageEnd,
                    BeginConversionToPackage,
                    EndConversionToPackage,
                    options
                );
                
                AssetDatabaseManager.Refresh();
            }
        }

        public static void ConvertToRepository(PackageSwapOptions options, params PackageMetadata[] targets)
        {
            using (_PRF_ConvertToRepository.Auto())
            {
                Convert(
                    targets,
                    ref _convertToRepositoryEnd,
                    BeginConversionToRepository,
                    EndConversionToRepository,
                    options
                );
                
                AssetDatabaseManager.Refresh();
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
                    BeginConversionToRepository,
                    EndConversionToRepository,
                    options
                );
                
                AssetDatabaseManager.Refresh();
            }
        }

        public static void ConvertToRepository(PackageMetadata target, PackageSwapOptions options)
        {
            using (_PRF_ConvertToRepository.Auto())
            {
                Convert(
                    new[] {target},
                    ref _convertToRepositoryEnd,
                    BeginConversionToRepository,
                    EndConversionToRepository,
                    options
                );
                
                AssetDatabaseManager.Refresh();
            }
        }

        private static readonly ProfilerMarker _PRF_ConvertToPackageInternal = new ProfilerMarker(_PRF_PFX + nameof(ConvertToPackageInternal));
        private static void ConvertToPackageInternal(RepositoryMetadata target, PackageSwapOptions options)
        {
            using (_PRF_ConvertToPackageInternal.Auto())
            {
                Convert(
                    new[] {target},
                    ref _convertToPackageEnd,
                    BeginConversionToPackage,
                    EndConversionToPackage,
                    options
                );
            }
        }

        private static readonly ProfilerMarker _PRF_ConvertToRepositoryInternal = new ProfilerMarker(_PRF_PFX + nameof(ConvertToRepositoryInternal));
        private static void ConvertToRepositoryInternal(PackageMetadata target, PackageSwapOptions options)
        {
            using (_PRF_ConvertToRepositoryInternal.Auto())
            {
                Convert(
                    new[] {target},
                    ref _convertToRepositoryEnd,
                    BeginConversionToRepository,
                    EndConversionToRepository,
                    options
                );
            }
        }

        private static void BeginConversionToPackage(RepositoryMetadata target, PackageSwapOptions options)
        {
            using (_PRF_ConvertToPackage_START.Auto())
            {
                var dryRun = options.HasFlag(PackageSwapOptions.DryRun);
                var refreshAssetsAfterDirectoryMove =
                    options.HasFlag(PackageSwapOptions.RefreshAssetsAfterMove);

                if (target.IsPackage || target.Path.EndsWith("~") || target.Path.StartsWith("Package"))
                {
                    return;
                }

                if (!AppaDirectory.Exists(target.Path))
                {
                    return;
                }

                AppaLog.Info(
                    $"Refreshing assets before converting [{target.Name}] from a repository to a package."
                );

                AppaLog.Info($"Converting [{target.Name}] from a repository to a package.");

                foreach (var dependency in target.dependencies)
                {
                    if (dependency.repository.IsAppalachiaManaged)
                    {
                        var subOptions = GetSuboptions(options);

                        ConvertToPackageInternal(dependency.repository, subOptions);
                    }
                }

                var root = target.Path;
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

        private static void BeginConversionToRepository(PackageMetadata target, PackageSwapOptions options)
        {
            using (_PRF_ConvertToRepository_START.Auto())
            {
                AppaLog.Info($"Converting [{target.Name}] from a package to a repository.");

                var dryRun = options.HasFlag(PackageSwapOptions.DryRun);

                target.PopulateDependents();

                foreach (var dependent in target.dependents)
                {
                    AppaLog.Warning($"Need to convert dependent package {dependent.Name} to a repository first.");
                    
                    if (dependent.IsAppalachiaManaged)
                    {
                        var subOptions = GetSuboptions(options);

                        ConvertToRepositoryInternal(dependent, subOptions);
                    }
                }

                var executePackageClient = options.HasFlag(PackageSwapOptions.ExecutePackageClient);

                if (executePackageClient)
                {
                    if (dryRun)
                    {
                        AppaLog.Info($"Removing [{target.Name}]");
                    }
                    else
                    {
                        var removal = Client.Remove(target.Name);

                        while (!removal.IsCompleted)
                        {
                        }
                    }
                }
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

        private static void EndConversionToPackage(RepositoryMetadata target, PackageSwapOptions options)
        {
            using (_PRF_ConvertToPackage_END.Auto())
            {
                AppaLog.Info($"Finishing repository [{target.Name}] conversion to package.");

                var dryRun = options.HasFlag(PackageSwapOptions.DryRun);
                var executePackageClient = options.HasFlag(PackageSwapOptions.ExecutePackageClient);

                if (executePackageClient)
                {
                    if (dryRun)
                    {
                        AppaLog.Info($"Adding [{target.NameAndVersion}]");
                    }
                    else
                    {
                        var addition = Client.Add(target.NameAndVersion);

                        while (!addition.IsCompleted)
                        {
                        }
                    }
                }
            }
        }

        private static void EndConversionToRepository(PackageMetadata target, PackageSwapOptions options)
        {
            using (_PRF_ConvertToRepository_END.Auto())
            {
                AppaLog.Info($"Finishing package [{target.Name}] conversion to repository.");

                var dryRun = options.HasFlag(PackageSwapOptions.DryRun);
                var refreshAssetsAfterMove = options.HasFlag(PackageSwapOptions.RefreshAssetsAfterMove);

                string directoryRoot;
                string directoryName;

                if (target.IsThirdParty || target.IsUnity)
                {
                    directoryName = target.packageInfo.name.Split(".").Last();
                    directoryRoot = "Assets/Third-Party/";
                }
                else
                {
                    directoryName = target.Name;
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
                            AppaLog.Info($"MOVEDIR: [{hideDirectory.RelativePath}] to [{dir.RelativePath}]");
                        }
                        else
                        {
                            AppaDirectory.Move(hideDirectory.RelativePath, dir.RelativePath);

                            if (refreshAssetsAfterMove)
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
                PackageSwapOptions.RefreshAssetsAfterMove |
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
    }
}
