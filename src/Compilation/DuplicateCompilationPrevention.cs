using System;
using Appalachia.Utility.Logging;
using UnityEngine;

namespace Appalachia.CI.Compilation
{
    #if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
    public static class DuplicateCompilationPrevention
    {
        static DuplicateCompilationPrevention()
        {
            UnityEditor.Compilation.CompilationPipeline.compilationStarted += OnCompilationStarted;
            UnityEditor.Compilation.CompilationPipeline.compilationFinished += OnCompilationEnded;
        }

        private static int _buildCalls;
        private static double _startTime;
        
        public static void OnCompilationStarted(object context)
        {
            _buildCalls += 1;
            AppaLog.Trace($"Build Calls: {_buildCalls}");

            if (_buildCalls > 1)
            {
                //UnityEditor.Scripting.ScriptCompilation.EditorCompilationInterface.Instance.StopCompilationTask();
            }
            else
            {
                _startTime = Time.realtimeSinceStartupAsDouble;
            }
        }

        public static void OnCompilationEnded(object context)
        {
            _buildCalls -= 1;
            var elapsed = Time.realtimeSinceStartupAsDouble - _startTime;
            AppaLog.Trace($"Build Time: {elapsed:F3}s");
        }
    }
    #endif
}
