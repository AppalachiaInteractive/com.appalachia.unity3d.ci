using System;
using System.Diagnostics;
using Appalachia.Utility.Strings;
using Unity.Profiling;

namespace Appalachia.CI.Integration.Extensions
{
    [DebuggerDisplay("Input = {input} | Result - {Result}")]
    public class StringCleaningSet : IDisposable
    {
        public StringCleaningSet()
        {
            _builder = new Utf16ValueStringBuilder(false);
        }

        #region Fields and Autoproperties

        public string input;
        private bool _builderInitialized;
        private bool _finished;
        private string _result;

        private Utf16ValueStringBuilder _builder;

        #endregion

        public bool IsFinished => _finished;

        public int Length => _builder.Length;

        public string Result
        {
            get
            {
                if (!_finished)
                {
                    throw new NotSupportedException("Must finish before getting the result!");
                }

                return _result;
            }
        }

        public char this[int i]
        {
            get => _builder[i];
            set
            {
                _builder[i] = value;
                _finished = false;
                _result = null;
            }
        }

        public void Dispose()
        {
            _builder.Dispose();
        }

        public string Finish()
        {
            using (_PRF_Finish.Auto())
            {
                if (_finished)
                {
                    throw new NotSupportedException("Already has finished computation!");
                }

                _result = _builder.ToString();
                _finished = true;

                return _result;
            }
        }

        public void Load(string newValue)
        {
            using (_PRF_Load.Auto())
            {
                if (!_builderInitialized)
                {
                    _builderInitialized = true;
                    _builder = new Utf16ValueStringBuilder(false);
                }

                _builder.Clear();
                _builder.Append(newValue);

                input = newValue;
                _finished = false;
                _result = null;
            }
        }

        public string Peek()
        {
            using (_PRF_Peek.Auto())
            {
                if (_result != null)
                {
                    return _result;
                }

                return _builder.ToString();
            }
        }

        public void Remove(int start, int length)
        {
            using (_PRF_Remove.Auto())
            {
                _finished = false;
                _result = null;
                _builder.Remove(start, length);
            }
        }

        public void Replace(char oldValue, char newValue)
        {
            using (_PRF_Replace.Auto())
            {
                _finished = false;
                _result = null;
                _builder.Replace(oldValue, newValue);
            }
        }

        public void Replace(string oldValue, string newValue)
        {
            using (_PRF_Replace.Auto())
            {
                _finished = false;
                _result = null;
                _builder.Replace(oldValue, newValue);
            }
        }

        public void Reset()
        {
            using (_PRF_Reset.Auto())
            {
                _builder.Clear();
                input = null;
                _finished = false;
                _result = null;
            }
        }

        private static readonly ProfilerMarker _PRF_SetResult =
            new ProfilerMarker(_PRF_PFX + nameof(SetResult));

        private static readonly ProfilerMarker _PRF_Reset = new ProfilerMarker(_PRF_PFX + nameof(Reset));

        public void SetResult(string result)
        {
            using (_PRF_SetResult.Auto())
            {
                _finished = true;
                _result = result;
            }
        }

        #region Profiling

        private const string _PRF_PFX = nameof(StringCleaningSet) + ".";
        private static readonly ProfilerMarker _PRF_Peek = new ProfilerMarker(_PRF_PFX + nameof(Peek));
        private static readonly ProfilerMarker _PRF_Replace = new ProfilerMarker(_PRF_PFX + nameof(Replace));
        private static readonly ProfilerMarker _PRF_Remove = new ProfilerMarker(_PRF_PFX + nameof(Remove));
        private static readonly ProfilerMarker _PRF_Load = new ProfilerMarker(_PRF_PFX + nameof(Load));
        private static readonly ProfilerMarker _PRF_Finish = new ProfilerMarker(_PRF_PFX + nameof(Finish));

        #endregion
    }
}
