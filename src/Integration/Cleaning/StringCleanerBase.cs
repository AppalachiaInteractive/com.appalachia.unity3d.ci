using System.Collections.Generic;
using System.Text;
using Unity.Profiling;

namespace Appalachia.CI.Integration.Cleaning
{
    public abstract class StringCleanerBase<T>
        where T : StringCleanerBase<T>
    {
        public delegate string ExecuteClean(T instance, string input);

        #region Profiling And Tracing Markers

        private const string _PRF_PFX = nameof(StringCleanerBase<T>) + ".";
        
        private static readonly ProfilerMarker _PRF_Clean = new(_PRF_PFX + nameof(Clean));

        private static readonly ProfilerMarker _PRF_StringCleanerBase =
            new(_PRF_PFX + nameof(StringCleanerBase<T>));

        private static readonly ProfilerMarker _PRF_Clean_Action = new(_PRF_PFX + nameof(Clean) + ".Action");

        #endregion

        protected StringCleanerBase(ExecuteClean action, int capacity = 100)
        {
            using (_PRF_StringCleanerBase.Auto())
            {
                _builder = new StringBuilder(capacity);
                _lookup = new Dictionary<string, string>();
                _action = action;
            }
        }

        private Dictionary<string, string> _lookup;
        private StringBuilder _builder;
        private ExecuteClean _action;

        public string Clean(string input)
        {
            using (_PRF_Clean.Auto())
            {
                if (_lookup.ContainsKey(input))
                {
                    return _lookup[input];
                }

                string result;
                using (_PRF_Clean_Action.Auto())
                {
                    result = _action((T) this, input);
                }

                _lookup.Add(input, result);

                return result;
            }
        }
    }
}
