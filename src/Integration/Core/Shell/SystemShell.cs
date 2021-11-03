using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using Appalachia.CI.Integration.Repositories;
using Appalachia.Utility.Extensions;
using Unity.Profiling;
using UnityEngine;

namespace Appalachia.CI.Integration.Core.Shell
{
    public static class SystemShell
    {
        #region Profiling And Tracing Markers

        private const string _PRF_PFX = nameof(SystemShell) + ".";
        private const string _TRACE_PFX = nameof(SystemShell) + ".";

        private static readonly ProfilerMarker _PRF_Execute = new ProfilerMarker(_PRF_PFX + nameof(Execute));

        #endregion

        public static IEnumerator Execute(
            string command,
            RepositoryMetadata repository,
            ShellResult shellResult = null,
            DataReceivedEventHandler standardOutHandler = null,
            DataReceivedEventHandler standardErrorHandler = null,
            Action onComplete = null)
        {
            using (_PRF_Execute.Auto())
            {
                return Execute(
                    command,
                    repository.RealPath,
                    shellResult,
                    standardOutHandler,
                    standardErrorHandler,
                    onComplete
                );
            }
        }

        public static IEnumerator Execute(
            string command,
            string workingDir,
            ShellResult shellResult = null,
            DataReceivedEventHandler standardOutHandler = null,
            DataReceivedEventHandler standardErrorHandler = null,
            Action onComplete = null,
            int timeout = 60)
        {
            using (_PRF_Execute.Auto())
            {
                var processKey = workingDir + ": " + command;

                Console.WriteLine($"[{processKey}] [{nameof(Execute)}] requested.");

                try
                {
                    if (SystemShellManager.IsAlreadyPreparedToRun(processKey))
                    {
                        yield break;
                    }

                    var outBuilder = new StringBuilder();
                    var errorBuilder = new StringBuilder();

                    var start = Time.time;

                    var process = SystemShellManager.PrepareProcess(
                        command,
                        workingDir,
                        standardOutHandler,
                        standardErrorHandler,
                        outBuilder,
                        errorBuilder
                    );

                    SystemShellManager.SubmitProcess(processKey, process);

                    using (_PRF_Execute.Suspend())
                    {
                        Console.WriteLine($"[{processKey}] [{nameof(Execute)}] entering wait loop.");

                        while (SystemShellManager.ShouldWaitForProcess(processKey))
                        {
                            yield return null;

                            IEnumeratorExtensions.CheckTimeout(processKey, start, Time.time, timeout);

                            if (UnityEditor.EditorApplication.isCompiling)
                            {
                                yield break;
                            }
                        }

                        Console.WriteLine($"[{processKey}] [{nameof(Execute)}] exiting wait loop.");
                    }

                    SystemShellManager.EndProcess(processKey, shellResult, process, errorBuilder, outBuilder);

                    onComplete?.Invoke();
                }
                finally
                {
                    SystemShellManager.ForgetProcess(processKey);
                }
            }
        }
    }
}
