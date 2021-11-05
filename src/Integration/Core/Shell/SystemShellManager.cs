using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Appalachia.CI.Integration.Extensions;
using Appalachia.Utility.Execution;
using Appalachia.Utility.Extensions;
using Unity.EditorCoroutines.Editor;
using Unity.Profiling;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Appalachia.CI.Integration.Core.Shell
{
    internal class SystemShellManager : ScriptableObject //, ISerializationCallbackReceiver
    {
        #region Profiling And Tracing Markers

        private const string _PRF_PFX = nameof(SystemShellManager) + ".";

        private static readonly ProfilerMarker _PRF_PrepareProcess =
            new ProfilerMarker(_PRF_PFX + nameof(PrepareProcess));

        private static readonly ProfilerMarker _PRF_EndProcess =
            new ProfilerMarker(_PRF_PFX + nameof(EndProcess));

        #endregion

        #region Constants and Static Readonly

        internal const float BLINK_TIME = .2f;
        internal const float SLEEP_TIME = 2f;

        internal const int MAX_THREADS = 4;
        internal const int SLEEP_THRESHOLD = 100;

        #endregion

        private bool _asleep;

        [NonSerialized] private bool _isAwake;

        private Dictionary<string, Process> _commandProcessLookup;
        private HashSet<string> _failedProcesses;
        private HashSet<string> _finishedProcesses;
        [SerializeField] private int _sleepCount;
        [SerializeField] private int[] _commandProcessLookup_SERIALVALUE;
        private Queue<string> _inProcessProcesses;
        private Queue<string> _pendingProcessesTemp;
        private Queue<string> _waitingProcesses;

        [SerializeField] private string[] _commandProcessLookup_SERIALKEY;
        [SerializeField] private string[] _failedProcesses_SERIAL;
        [SerializeField] private string[] _finishedProcesses_SERIAL;
        [SerializeField] private string[] _inProcessProcesses_SERIAL;
        [SerializeField] private string[] _pendingProcessesTemp_SERIAL;
        [SerializeField] private string[] _waitingProcesses_SERIAL;

        #region Event Functions

        private void Awake()
        {
            EnsureInitialized();
        }

        private void OnEnable()
        {
            EnsureInitialized();
        }

        #endregion

        public bool DidProcessFail(string processKey)
        {
            return _failedProcesses.Contains(processKey);
        }

        public void EndProcess(
            string processKey,
            ShellResult shellResult,
            Process process,
            StringBuilder errorBuilder,
            StringBuilder outBuilder)
        {
            using (_PRF_EndProcess.Auto())
            {
                var failedToStart = DidProcessFail(processKey);

                ShellLogger.Log<SystemShellManager>(processKey, $"[{nameof(EndProcess)}] saving results.");

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
                        ShellLogger.Log<SystemShellManager>(
                            processKey,
                            $"[{nameof(EndProcess)}] -------------INVALID OPERATION: {iox.Message}."
                        );

                        Debug.LogError(
                            $"[{processKey}] [{nameof(EndProcess)}] Error while finalizing.\r\n----------\r\n{iox}"
                        );
                        throw;
                    }
                }
            }
        }

        public void EnsureInitialized()
        {
            if (_isAwake)
            {
                return;
            }

            _inProcessProcesses ??= new Queue<string>();
            _pendingProcessesTemp ??= new Queue<string>();
            _waitingProcesses ??= new Queue<string>();
            _failedProcesses ??= new HashSet<string>();
            _finishedProcesses ??= new HashSet<string>();
            _commandProcessLookup ??= new Dictionary<string, Process>();

            UnityEditor.EditorApplication.delayCall += ProcessPending;

            _isAwake = true;
        }

        public void ForgetProcess(string processKey)
        {
            _finishedProcesses.Remove(processKey);
            _failedProcesses.Remove(processKey);
            _commandProcessLookup?.Remove(processKey);
            _finishedProcesses.Remove(processKey);
        }

        public bool IsAlreadyPreparedToRun(string processKey)
        {
            return _commandProcessLookup.ContainsKey(processKey);
        }

        public void OnAfterDeserialize()
        {
            try
            {
                _inProcessProcesses = new Queue<string>(_inProcessProcesses_SERIAL);
                _pendingProcessesTemp = new Queue<string>(_pendingProcessesTemp_SERIAL);
                _waitingProcesses = new Queue<string>(_waitingProcesses_SERIAL);
                _failedProcesses = new HashSet<string>(_failedProcesses_SERIAL);
                _finishedProcesses = new HashSet<string>(_finishedProcesses_SERIAL);

                _commandProcessLookup = new Dictionary<string, Process>();

                for (var i = 0; i < _commandProcessLookup_SERIALKEY.Length; i++)
                {
                    var key = _commandProcessLookup_SERIALKEY[i];
                    var valueID = _commandProcessLookup_SERIALVALUE[i];

                    try
                    {
                        var process = Process.GetProcessById(valueID);
                        _commandProcessLookup.Add(key, process);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        public void OnBeforeSerialize()
        {
            try
            {
                _inProcessProcesses_SERIAL = _inProcessProcesses.ToArray();
                _pendingProcessesTemp_SERIAL = _pendingProcessesTemp.ToArray();
                _waitingProcesses_SERIAL = _waitingProcesses.ToArray();
                _failedProcesses_SERIAL = _failedProcesses.ToArray();
                _finishedProcesses_SERIAL = _finishedProcesses.ToArray();

                _commandProcessLookup_SERIALKEY = _commandProcessLookup.Keys.ToArray();
                var values = _commandProcessLookup.Values.ToArray();

                _commandProcessLookup_SERIALVALUE = values.Select(p => p.Id).ToArray();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        public Process PrepareAndSubmitProcess(
            string processKey,
            string command,
            string workingDir,
            bool elevated,
            DataReceivedEventHandler standardOutHandler,
            DataReceivedEventHandler standardErrorHandler,
            StringBuilder outBuilder,
            StringBuilder errorBuilder)
        {
            var process = PrepareProcess(
                processKey,
                command,
                workingDir,
                elevated,
                standardOutHandler,
                standardErrorHandler,
                outBuilder,
                errorBuilder
            );

            SubmitProcess(processKey, process);

            return process;
        }

        public bool ShouldWaitForProcess(string processKey)
        {
            if (_finishedProcesses.Contains(processKey))
            {
                return false;
            }

            if (_failedProcesses.Contains(processKey))
            {
                return false;
            }

            return true;
        }

        private void CloseProcess(string processKey)
        {
            if (_commandProcessLookup.ContainsKey(processKey))
            {
                var process = _commandProcessLookup[processKey];
                _commandProcessLookup.Remove(processKey);
                process.Close();
            }

            _finishedProcesses.Remove(processKey);
        }

        private void EndProcess(
            string processKey,
            bool failedToStart,
            ShellResult shellResult,
            Process process,
            StringBuilder errorBuilder,
            StringBuilder outBuilder)
        {
            using (_PRF_EndProcess.Auto())
            {
                ShellLogger.Log<SystemShellManager>(processKey, $"[{nameof(EndProcess)}] saving results.");

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
                        ShellLogger.Log<SystemShellManager>(
                            processKey,
                            $"[{nameof(EndProcess)}] -------------INVALID OPERATION: {iox.Message}."
                        );

                        Debug.LogError(
                            $"[{processKey}] [{nameof(EndProcess)}] Error while finalizing.\r\n----------\r\n{iox}"
                        );
                        throw;
                    }
                }
            }
        }

        private void ExecuteProcessingLoop()
        {
            _pendingProcessesTemp.Clear();

            while (_inProcessProcesses.Count > 0)
            {
                var processKey = _inProcessProcesses.Dequeue();

                if (!_commandProcessLookup.ContainsKey(processKey))
                {
                    continue;
                }

                var oldProcess = _commandProcessLookup[processKey];
                var elapsed = (DateTime.Now - oldProcess.StartTime).TotalSeconds;

                ShellLogger.Log<SystemShellManager>($"[{nameof(ExecuteProcessingLoop)}] checking exit.");

                if (!oldProcess.HasExited)
                {
                    ShellLogger.Log<SystemShellManager>(
                        processKey,
                        elapsed,
                        $"[{nameof(ExecuteProcessingLoop)}] requeueing."
                    );

                    _pendingProcessesTemp.Enqueue(processKey);
                }
                else
                {
                    ShellLogger.Log<SystemShellManager>(
                        processKey,
                        elapsed,
                        $"[{nameof(ExecuteProcessingLoop)}] finishing."
                    );

                    _finishedProcesses.Add(processKey);
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

                var newProcess = _commandProcessLookup[processKey];

                ShellLogger.Log<SystemShellManager>(
                    processKey,
                    $"[{nameof(ExecuteProcessingLoop)}] checking start."
                );

                try
                {
                    ShellLogger.Log<SystemShellManager>(
                        processKey,
                        $"[{nameof(ExecuteProcessingLoop)}] starting."
                    );

                    var startResult = newProcess.Start();

                    if (!startResult)
                    {
                        _failedProcesses.Add(processKey);
                        continue;
                    }

                    ShellLogger.Log<SystemShellManager>(
                        processKey,
                        $"[{processKey}] [{nameof(ExecuteProcessingLoop)}] reading."
                    );

                    newProcess.BeginOutputReadLine();
                    newProcess.BeginErrorReadLine();
                }
                catch (InvalidOperationException iox)
                {
                    ShellLogger.Log<SystemShellManager>(
                        processKey,
                        $"[{nameof(ExecuteProcessingLoop)}] -------------INVALID OPERATION: {iox.Message}."
                    );

                    Debug.LogError(
                        $"[{processKey}] [{nameof(ExecuteProcessingLoop)}] Error while starting.\r\n----------\r\n{iox}"
                    );
                }

                ShellLogger.Log<SystemShellManager>(processKey, "queued.");
                _inProcessProcesses.Enqueue(processKey);
            }
        }

        private void FinalizeProcessingLoop()
        {
            while (_inProcessProcesses.TryDequeue(out var processKey))
            {
                ShellLogger.Log<SystemShellManager>(
                    processKey,
                    $"[{nameof(FinalizeProcessingLoop)}] clearing."
                );

                if (!_commandProcessLookup.ContainsKey(processKey))
                {
                    continue;
                }

                CloseProcess(processKey);
            }

            while (_waitingProcesses.TryDequeue(out var processKey))
            {
                ShellLogger.Log<SystemShellManager>(
                    processKey,
                    $"[{nameof(FinalizeProcessingLoop)}] clearing."
                );

                if (!_commandProcessLookup.ContainsKey(processKey))
                {
                    continue;
                }

                _commandProcessLookup.Remove(processKey);
            }
        }

        private Process PrepareProcess(
            string processKey,
            string command,
            string workingDir,
            bool elevated,
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

                if (elevated)
                {
                    processStartInfo.Verb = "runas";
                }

                var process = new Process {StartInfo = processStartInfo, EnableRaisingEvents = true};

                process.OutputDataReceived += (sender, args) =>
                {
                    var data = args.Data;
                        
                    if (data == null)
                    {
                        return;
                    }

                    ShellLogger.Log<SystemShellManager>(
                        processKey,
                        $"[OutputDataReceived]: {data}"
                    );
                    
                    outBuilder.AppendLine();
                    standardOutHandler?.Invoke(sender, args);
                };

                process.ErrorDataReceived += (sender, args) =>
                {
                    var data = args.Data;
                    
                    if (data == null)
                    {
                        return;
                    }
                    
                    ShellLogger.Log<SystemShellManager>(
                        processKey,
                        $"[ErrorDataReceived]: {data}"
                    );
                    
                    errorBuilder.AppendLine(data);
                    standardErrorHandler?.Invoke(sender, args);
                };

                return process;
            }
        }

        private void ProcessPending()
        {
            var wrapper = ProcessPendingEnumerator().ToSafe("System Shell Manager Processing");
            wrapper.ExecuteAsEditorCoroutine();
        }

        private IEnumerator ProcessPendingEnumerator()
        {
            try
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

                        yield return new EditorWaitForSeconds(BLINK_TIME);
                        continue;
                    }

                    if (_asleep)
                    {
                        yield return new EditorWaitForSeconds(SLEEP_TIME);
                        continue;
                    }

                    _sleepCount = 0;

                    ExecuteProcessingLoop();

                    yield return new EditorWaitForSeconds(BLINK_TIME);
                }
            }
            finally
            {
                FinalizeProcessingLoop();
            }
        }

        private void SubmitProcess(string processKey, Process process)
        {
            _asleep = false;
            _sleepCount = 0;

            _waitingProcesses.Enqueue(processKey);
            _commandProcessLookup.Add(processKey, process);
        }
    }
}
