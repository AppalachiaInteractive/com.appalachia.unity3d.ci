using System;
using System.Collections.Generic;
using Unity.Profiling;

namespace Appalachia.CI.Integration.Core
{
    public static class IntegrationMetadataRegistry<T>
        where T : IntegrationMetadata
    {
        #region Profiling And Tracing Markers

        private const string _PRF_PFX = nameof(IntegrationMetadataRegistry<T>) + ".";
        private static Action _finalizing;
        private static Action _processing;
        private static bool _finalized;
        private static bool _processed;
        private static Func<IEnumerable<T>> _creation;
        private static List<T> _all;
        private static readonly ProfilerMarker _PRF_Clear = new(_PRF_PFX + nameof(Clear));
        private static readonly ProfilerMarker _PRF_FindAll = new(_PRF_PFX + nameof(FindAll));
        private static readonly ProfilerMarker _PRF_Refresh = new(_PRF_PFX + nameof(Refresh));
        private static readonly ProfilerMarker _PRF_Register = new(_PRF_PFX + nameof(Register));
        private static readonly ProfilerMarker _PRF_Initialize = new(_PRF_PFX + nameof(Initialize));
        private static readonly ProfilerMarker _PRF_Finalize = new(_PRF_PFX + nameof(OnFinalize));

        #endregion

        public static void Clear()
        {
            using (_PRF_Clear.Auto())
            {
                _all?.Clear();
            }
        }

        public static IReadOnlyList<T> FindAll()
        {
            using (_PRF_FindAll.Auto())
            {
                if (!IntegrationMetadataRegisterExecutor.HasExecuted)
                {
                    IntegrationMetadataRegisterExecutor.Execute();
                }

                Initialize();

                return _all;
            }
        }

        public static void Refresh()
        {
            using (_PRF_Refresh.Auto())
            {
                _processed = false;
                _finalized = false;
                _all?.Clear();
                IntegrationMetadataRegisterExecutor.Reset();
            }
        }

        public static void Register(
            int priority,
            Func<IEnumerable<T>> createAll,
            Action processing,
            Action finalizing)
        {
            using (_PRF_Register.Auto())
            {
                _creation = createAll;
                _processing = processing;
                _finalizing = finalizing;

                IntegrationMetadataRegisterExecutor.Register(priority, Initialize, OnFinalize);
            }
        }

        private static void OnFinalize()
        {
            using (_PRF_Finalize.Auto())
            {
                if (!_finalized)
                {
                    _finalized = true;
                    _finalizing();
                }
            }
        }

        private static void Initialize()
        {
            using (_PRF_Initialize.Auto())
            {
                if (_all == null)
                {
                    _all = new List<T>();
                }

                if (_all.Count == 0)
                {
                    if (_creation == null)
                    {
                        throw new NotSupportedException("CREATION MUST EXIST!");
                    }

                    var all = _creation();

                    foreach (var meta in all)
                    {
                        var message = meta.Name ?? meta.Id ?? meta.Path;
                        
                        if (meta.Name == null)
                        {
                            continue;
                            throw new NotSupportedException($"{typeof(T).Name}: Missing Name | {message}");
                        }
                        
                        if (meta.Id == null)
                        {
                            continue;
                            throw new NotSupportedException($"{typeof(T).Name}: Missing Id | {message}");
                        }
                        
                        if (meta.Path == null)
                        {
                            continue;
                            throw new NotSupportedException($"{typeof(T).Name}: Missing Path | {message}");
                        }

                        _all.Add(meta);
                    }
                }

                if (!_processed)
                {
                    _processed = true;
                    _processing();
                }
            }
        }
    }
}
