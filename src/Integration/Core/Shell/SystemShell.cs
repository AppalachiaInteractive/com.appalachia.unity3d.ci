using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using Appalachia.CI.Integration.Repositories;
using Appalachia.Utility.Extensions;
using Unity.EditorCoroutines.Editor;
using Unity.Profiling;
using UnityEngine;

namespace Appalachia.CI.Integration.Core.Shell
{
    public class SystemShell : ScriptableObject, ISerializationCallbackReceiver
    {
        #region Profiling And Tracing Markers

        private const string _PRF_PFX = nameof(SystemShell) + ".";
        private const string _TRACE_PFX = nameof(SystemShell) + ".";

        private static readonly ProfilerMarker _PRF_Execute = new ProfilerMarker(_PRF_PFX + nameof(Execute));

        #endregion

        private SystemShell()
        {
        }

        private static SystemShell _instance;
        [SerializeField] private SystemShell _instanceSERIAL;
        [SerializeField] private SystemShellManager _manager;

        public static SystemShell Instance
        {
            get
            {
                if (_instance == null)
                {
                    CreateInstance<SystemShell>();
                }

                return _instance;
            }
        }

        private SystemShellManager manager
        {
            get
            {
                _manager ??= CreateInstance<SystemShellManager>();

                return _manager;
            }
        }

        #region Event Functions

        private void Awake()
        {
            _instanceSERIAL ??= this;
            _instance ??= this;
        }

        private void OnEnable()
        {
            _instanceSERIAL ??= this;
            _instance ??= this;
        }

        #endregion

        public IEnumerator Execute(
            string command,
            RepositoryMetadata repository,
            bool elevated = false,
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
                    elevated,
                    shellResult,
                    standardOutHandler,
                    standardErrorHandler,
                    onComplete
                );
            }
        }

        public IEnumerator Execute(
            string command,
            string workingDir,
            bool elevated = false,
            ShellResult shellResult = null,
            DataReceivedEventHandler standardOutHandler = null,
            DataReceivedEventHandler standardErrorHandler = null,
            Action onComplete = null,
            int timeout = 60)
        {
            using (_PRF_Execute.Auto())
            {
                var processKey = workingDir + ": " + command;
                
                ShellLogger.Log<SystemShell>(processKey, $"[{nameof(Execute)}] requested.");

                manager.EnsureInitialized();
                
                try
                {
                    
                    if (manager.IsAlreadyPreparedToRun(processKey))
                    {
                        yield break;
                    }

                    var outBuilder = new StringBuilder();
                    var errorBuilder = new StringBuilder();

                    var start = Time.realtimeSinceStartup;

                    var process = manager.PrepareAndSubmitProcess(
                        processKey,
                        command,
                        workingDir,
                        elevated,
                        standardOutHandler,
                        standardErrorHandler,
                        outBuilder,
                        errorBuilder
                    );

                    using (_PRF_Execute.Suspend())
                    {
                        ShellLogger.Log<SystemShell>(processKey, $"[{nameof(Execute)}] entering wait loop.");

                        while (manager.ShouldWaitForProcess(processKey))
                        {
                            yield return new EditorWaitForSeconds(SystemShellManager.BLINK_TIME*.25F);

                            if (UnityEditor.EditorApplication.isCompiling)
                            {
                                yield break;
                            }
                        }

                        ShellLogger.Log<SystemShell>(processKey, $"[{nameof(Execute)}] exiting wait loop.");
                    }

                    manager.EndProcess(processKey, shellResult, process, errorBuilder, outBuilder);

                    onComplete?.Invoke();
                }
                finally
                {
                    manager.ForgetProcess(processKey);
                }
            }
        }

        public IEnumerator ExecuteHere(
            string command,
            bool elevated = false,
            ShellResult shellResult = null,
            DataReceivedEventHandler standardOutHandler = null,
            DataReceivedEventHandler standardErrorHandler = null,
            Action onComplete = null,
            int timeout = 60)
        {
            return Execute(
                command,
                Application.dataPath,
                elevated,
                shellResult,
                standardOutHandler,
                standardErrorHandler,
                onComplete,
                timeout
            );
        }

        public void OnAfterDeserialize()
        {
            _instance = _instanceSERIAL;
        }

        public void OnBeforeSerialize()
        {
            _instanceSERIAL = _instance;
        }
    }
}
