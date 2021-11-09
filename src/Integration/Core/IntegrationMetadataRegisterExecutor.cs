using System;
using System.Collections.Generic;
using Unity.Profiling;

namespace Appalachia.CI.Integration.Core
{
    public static class IntegrationMetadataRegisterExecutor
    {
        #region Profiling And Tracing Markers

        private const string _PRF_PFX = nameof(IntegrationMetadataRegisterExecutor) + ".";
        private static List<(int priority, Action action)> _finalizations;
        private static List<(int priority, Action action)> _initializations;
        private static readonly ProfilerMarker _PRF_Execute = new(_PRF_PFX + nameof(Execute));

        private static readonly ProfilerMarker _PRF_Register = new(_PRF_PFX + nameof(Register));

        private static readonly ProfilerMarker _PRF_Reset = new(_PRF_PFX + nameof(Reset));

        #endregion

        public static bool HasExecuted { get; private set; }

        public static void Execute()
        {
            using (_PRF_Execute.Auto())
            {
                if (HasExecuted)
                {
                    return;
                }

                HasExecuted = true;

                foreach (var initialization in _initializations)
                {
                    initialization.action();
                }

                foreach (var finalization in _finalizations)
                {
                    finalization.action();
                }
            }
        }

        public static void Register(int priority, Action initialization, Action finalization)
        {
            using (_PRF_Register.Auto())
            {
                if (_initializations == null)
                {
                    _initializations = new List<(int priority, Action action)>();
                }

                if (_finalizations == null)
                {
                    _finalizations = new List<(int priority, Action action)>();
                }

                _initializations.Add((priority, initialization));

                _initializations.Sort((a, b) => a.priority.CompareTo(b.priority));

                _finalizations.Add((priority, finalization));

                _finalizations.Sort((a, b) => a.priority.CompareTo(b.priority));
            }
        }

        public static void Reset()
        {
            using (_PRF_Reset.Auto())
            {
                HasExecuted = false;
            }
        }
    }
}
