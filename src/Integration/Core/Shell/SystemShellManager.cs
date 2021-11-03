using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Appalachia.CI.Integration.Extensions;
using Appalachia.Utility.Extensions;
using Unity.EditorCoroutines.Editor;
using Unity.Profiling;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Appalachia.CI.Integration.Core.Shell
{
    public static partial class SystemShell
    {
        #region Nested Types

        [UnityEditor.InitializeOnLoad]
        private static class SystemShellManager
        {
            #region Profiling And Tracing Markers

            private const string _PRF_PFX = nameof(SystemShellManager) + ".";

            private static readonly ProfilerMarker _PRF_PrepareProcess =
                new ProfilerMarker(_PRF_PFX + nameof(PrepareProcess));

            private static readonly ProfilerMarker _PRF_EndProcess =
                new ProfilerMarker(_PRF_PFX + nameof(EndProcess));

            #endregion

            private static readonly Dictionary<string, Process> _commandProcessLookup;
            private const float BLINK_TIME = .2f;
            private const float SLEEP_TIME = 2f;
            private static readonly HashSet<string> _closedProcesses;
            private static readonly HashSet<string> _failedProcesses;
            private static readonly HashSet<string> _finishedProcesses;

            private const int MAX_THREADS = 4;
            private const int SLEEP_THRESHOLD = 100;
            private static readonly Queue<string> _waitingProcesses;

            static SystemShellManager()
            {
                _inProcessProcesses ??= new Queue<string>();
                _pendingProcessesTemp ??= new Queue<string>();
                _waitingProcesses ??= new Queue<string>();
                _failedProcesses ??= new HashSet<string>();
                _finishedProcesses ??= new HashSet<string>();
                _closedProcesses ??= new HashSet<string>();
                _needCloseProcesses ??= new HashSet<string>();
                _commandProcessLookup ??= new Dictionary<string, Process>();

                UnityEditor.EditorApplication.delayCall += ProcessPending;
            }

            private static bool _asleep;

            private static HashSet<string> _needCloseProcesses;
            private static int _sleepCount;
            private static Queue<string> _inProcessProcesses;
            private static Queue<string> _pendingProcessesTemp;

            public static bool DidProcessFail(string processKey)
            {
                return _failedProcesses.Contains(processKey);
            }

            public static bool IsAlreadyPreparedToRun(string processKey)
            {
                return _commandProcessLookup.ContainsKey(processKey);
            }

            public static bool ShouldWaitForProcess(string processKey)
            {
                return !(_finishedProcesses.Contains(processKey) || _failedProcesses.Contains(processKey));
            }

            public static void CloseProcess(string processKey)
            {
                if (_commandProcessLookup.ContainsKey(processKey))
                {
                    var process = _commandProcessLookup[processKey];
                    _commandProcessLookup.Remove(processKey);
                    process.Close();
                }

                _needCloseProcesses.Remove(processKey);
                _closedProcesses.Add(processKey);
            }

            public static void EndProcess(
                string processKey,
                ShellResult shellResult,
                Process process,
                StringBuilder errorBuilder,
                StringBuilder outBuilder)
            {
                using (_PRF_EndProcess.Auto())
                {
                    var failedToStart = DidProcessFail(processKey);

                    Console.WriteLine($"[{processKey}] [{nameof(EndProcess)}] saving results.");

                    if (shellResult != null)
                    {
                        try
                        {
                            if (failedToStart)
                            {
                                shellResult.exitCode = 1;
                                shellResult.error = $"Command [{processKey}] failed to start.";
                            }
                            else
                            {
                                shellResult.exitCode = process.ExitCode;
                                shellResult.error = errorBuilder.ToString();
                                shellResult.output = outBuilder.ToString();
                            }

                            CloseProcess(processKey);
                        }
                        catch (InvalidOperationException iox)
                        {
                            Console.WriteLine(
                                $"[{processKey}] [{nameof(EndProcess)}] -------------INVALID OPERATION: {iox.Message}."
                            );

                            Debug.LogError(
                                $"[{processKey}] [{nameof(EndProcess)}] Error while finalizing.\r\n----------\r\n{iox}"
                            );
                            throw;
                        }
                    }
                }
            }

            public static void ForgetProcess(string processKey)
            {
                _finishedProcesses.Remove(processKey);
                _failedProcesses.Remove(processKey);
                _commandProcessLookup?.Remove(processKey);
            }

            public static void SubmitProcess(string processKey, Process process)
            {
                _asleep = false;
                _sleepCount = 0;

                _waitingProcesses.Enqueue(processKey);
                _commandProcessLookup.Add(processKey, process);
            }

            internal static Process PrepareProcess(
                string command,
                string workingDir,
                DataReceivedEventHandler standardOutHandler,
                DataReceivedEventHandler standardErrorHandler,
                StringBuilder outBuilder,
                StringBuilder errorBuilder)
            {
                using (_PRF_PrepareProcess.Auto())
                {
                    const string bash = @"C:\Program Files\Git\bin\bash.exe";

                    var processStartInfo = new ProcessStartInfo(bash, "-c \" " + command + " \"")
                    {
                        WorkingDirectory = workingDir,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden,
                    };

                    var linuxWorkingDirectory = workingDir.ToAbsolutePath().WindowsToLinuxPath();
                    processStartInfo.EnvironmentVariables["APPA_PWD"] = linuxWorkingDirectory;
                    processStartInfo.EnvironmentVariables["APPA_FAST"] = "1";
                    processStartInfo.EnvironmentVariables["APPA_LOAD_BASHRC"] = "1";

                    var process = new Process {StartInfo = processStartInfo, EnableRaisingEvents = true};

                    process.OutputDataReceived += (sender, args) =>
                    {
                        if (args.Data == null)
                        {
                            return;
                        }

                        outBuilder.AppendLine(args.Data);
                        standardOutHandler?.Invoke(sender, args);
                    };

                    process.ErrorDataReceived += (sender, args) =>
                    {
                        if (args.Data == null)
                        {
                            return;
                        }

                        errorBuilder.AppendLine(args.Data);
                        standardErrorHandler?.Invoke(sender, args);
                    };

                    return process;
                }
            }

            private static IEnumerator ProcessPendingEnumerator()
            {
                while (!UnityEditor.EditorApplication.isCompiling)
                {
                    if ((_inProcessProcesses.Count == 0) && (_waitingProcesses.Count == 0))
                    {
                        _sleepCount += 1;

                        if (_sleepCount > SLEEP_THRESHOLD)
                        {
                            _asleep = true;
                        }

                        yield return new WaitForSeconds(BLINK_TIME);
                        continue;
                    }

                    if (_asleep)
                    {
                        yield return new WaitForSeconds(SLEEP_TIME);
                        continue;
                    }

                    _sleepCount = 0;

                    ExecuteProcessingLoop();

                    yield return new WaitForSeconds(BLINK_TIME);
                }

                FinalizeProcessingLoop();
            }

            private static void EndProcess(
                string processKey,
                bool failedToStart,
                ShellResult shellResult,
                Process process,
                StringBuilder errorBuilder,
                StringBuilder outBuilder)
            {
                using (_PRF_EndProcess.Auto())
                {
                    Console.WriteLine($"[{processKey}] [{nameof(EndProcess)}] saving results.");

                    if (shellResult != null)
                    {
                        try
                        {
                            if (failedToStart)
                            {
                                shellResult.exitCode = 1;
                                shellResult.error = $"Command [{processKey}] failed to start.";
                            }
                            else
                            {
                                shellResult.exitCode = process.ExitCode;
                                shellResult.error = errorBuilder.ToString();
                                shellResult.output = outBuilder.ToString();
                            }

                            CloseProcess(processKey);
                        }
                        catch (InvalidOperationException iox)
                        {
                            Console.WriteLine(
                                $"[{processKey}] [{nameof(EndProcess)}] -------------INVALID OPERATION: {iox.Message}."
                            );

                            Debug.LogError(
                                $"[{processKey}] [{nameof(EndProcess)}] Error while finalizing.\r\n----------\r\n{iox}"
                            );
                            throw;
                        }
                    }
                }
            }

            private static void ExecuteProcessingLoop()
            {
                _pendingProcessesTemp.Clear();

                while (_inProcessProcesses.Count > 0)
                {
                    var processKey = _inProcessProcesses.Dequeue();

                    Console.WriteLine($"[{processKey}] [{nameof(ExecuteProcessingLoop)}] checking exit.");

                    if (!_commandProcessLookup.ContainsKey(processKey))
                    {
                        continue;
                    }

                    var oldProcess = _commandProcessLookup[processKey];

                    if (!oldProcess.HasExited)
                    {
                        Console.WriteLine($"[{processKey}] [{nameof(ExecuteProcessingLoop)}] requeueing.");
                        Console.WriteLine($"[{processKey}] requeueing.");
                        _pendingProcessesTemp.Enqueue(processKey);
                    }
                    else
                    {
                        Console.WriteLine($"[{processKey}] [{nameof(ExecuteProcessingLoop)}] finishing.");

                        _finishedProcesses.Add(processKey);
                        _needCloseProcesses.Add(processKey);
                    }
                }

                var swap = _inProcessProcesses;
                _inProcessProcesses = _pendingProcessesTemp;
                _pendingProcessesTemp = swap;

                while ((_inProcessProcesses.Count < MAX_THREADS) && (_waitingProcesses.Count > 0))
                {
                    var processKey = _waitingProcesses.Dequeue();

                    if (!_commandProcessLookup.ContainsKey(processKey))
                    {
                        continue;
                    }

                    Console.WriteLine($"[{processKey}] [{nameof(ExecuteProcessingLoop)}] checking start.");

                    var newProcess = _commandProcessLookup[processKey];

                    try
                    {
                        Console.WriteLine($"[{processKey}] [{nameof(ExecuteProcessingLoop)}] starting.");
                        var startResult = newProcess.Start();

                        if (!startResult)
                        {
                            _failedProcesses.Add(processKey);
                            continue;
                        }

                        Console.WriteLine($"[{processKey}] [{nameof(ExecuteProcessingLoop)}] reading.");

                        newProcess.BeginOutputReadLine();
                        newProcess.BeginErrorReadLine();
                    }
                    catch (InvalidOperationException iox)
                    {
                        Console.WriteLine(
                            $"[{processKey}] [{nameof(ExecuteProcessingLoop)}] -------------INVALID OPERATION: {iox.Message}."
                        );

                        Debug.LogError(
                            $"[{processKey}] [{nameof(ExecuteProcessingLoop)}] Error while starting.\r\n----------\r\n{iox}"
                        );
                    }

                    Console.WriteLine($"[{processKey}] queued.");
                    _inProcessProcesses.Enqueue(processKey);
                }
            }

            private static void FinalizeProcessingLoop()
            {
                while (_inProcessProcesses.TryDequeue(out var processKey))
                {
                    Console.WriteLine($"[{processKey}] [{nameof(FinalizeProcessingLoop)}] clearing.");

                    if (!_commandProcessLookup.ContainsKey(processKey))
                    {
                        continue;
                    }

                    CloseProcess(processKey);
                }

                while (_waitingProcesses.TryDequeue(out var processKey))
                {
                    Console.WriteLine($"[{processKey}] [{nameof(FinalizeProcessingLoop)}] clearing.");

                    if (!_commandProcessLookup.ContainsKey(processKey))
                    {
                        continue;
                    }

                    _commandProcessLookup.Remove(processKey);
                }
            }

            private static void ProcessPending()
            {
                EditorCoroutineUtility.StartCoroutineOwnerless(ProcessPendingEnumerator());
            }
        }

        #endregion
    }
}
