using System;
using System.Text;
using Appalachia.Utility.Strings;
using Unity.Profiling;

namespace Appalachia.CI.Integration.Core
{
    public class SearchStringBuilder
    {
        public SearchStringBuilder()
        {
            using (_PRF_SearchStringBuilder.Auto())
            {
                Initialize();
            }
        }

        #region Static Fields and Autoproperties

        private static SearchStringBuilder _instance;

        #endregion

        #region Fields and Autoproperties

        private StringBuilder _stringBuilder;

        #endregion

        public static SearchStringBuilder Build => new();

        /// <inheritdoc />
        public override string ToString()
        {
            using (_PRF_ToString.Auto())
            {
                Initialize();
                return _stringBuilder.ToString();
            }
        }

        public SearchStringBuilder AddName(string s)
        {
            using (_PRF_AddName.Auto())
            {
                return AddTerm(s);
            }
        }

        public SearchStringBuilder AddTerm(string s)
        {
            using (_PRF_AddTerm.Auto())
            {
                Initialize();
                _stringBuilder.Append(GetTermPrefix());
                _stringBuilder.Append(s);

                return this;
            }
        }

        public SearchStringBuilder AddType<T>()
        {
            using (_PRF_AddType.Auto())
            {
                return AddType(typeof(T));
            }
        }

        public SearchStringBuilder AddType(Type t)
        {
            using (_PRF_AddType.Auto())
            {
                return AddTerm(ZString.Format("t:{0}", t.Name));
            }
        }

        public SearchStringBuilder AddTypeName(string t)
        {
            using (_PRF_AddType.Auto())
            {
                if (t == "FontAsset")
                {
                    return AddTerm("t:Font");
                }

                return AddTerm(ZString.Format("t:{0}", t));
            }
        }

        public string Finish()
        {
            using (_PRF_Finish.Auto())
            {
                Initialize();
                return ToString();
            }
        }

        public SearchStringBuilder Reset()
        {
            using (_PRF_Reset.Auto())
            {
                Initialize();
                _stringBuilder.Clear();

                return this;
            }
        }

        private string GetTermPrefix()
        {
            using (_PRF_GetTermPrefix.Auto())
            {
                Initialize();
                return _stringBuilder.Length > 0 ? " " : string.Empty;
            }
        }

        private void Initialize()
        {
            using (_PRF_Initialize.Auto())
            {
                if (_stringBuilder == null)
                {
                    _stringBuilder = new StringBuilder();
                }
            }
        }

        #region Profiling

        private const string _PRF_PFX = nameof(SearchStringBuilder) + ".";

        private static readonly ProfilerMarker _PRF_Initialize =
            new ProfilerMarker(_PRF_PFX + nameof(Initialize));

        private static readonly ProfilerMarker _PRF_SearchStringBuilder =
            new ProfilerMarker(_PRF_PFX + nameof(SearchStringBuilder));

        private static readonly ProfilerMarker
            _PRF_ToString = new ProfilerMarker(_PRF_PFX + nameof(ToString));

        private static readonly ProfilerMarker _PRF_AddName = new ProfilerMarker(_PRF_PFX + nameof(AddName));

        private static readonly ProfilerMarker _PRF_AddTerm = new ProfilerMarker(_PRF_PFX + nameof(AddTerm));

        private static readonly ProfilerMarker _PRF_AddType = new ProfilerMarker(_PRF_PFX + nameof(AddType));

        private static readonly ProfilerMarker _PRF_Finish = new ProfilerMarker(_PRF_PFX + nameof(Finish));

        private static readonly ProfilerMarker _PRF_Reset = new ProfilerMarker(_PRF_PFX + nameof(Reset));

        private static readonly ProfilerMarker _PRF_GetTermPrefix =
            new ProfilerMarker(_PRF_PFX + nameof(GetTermPrefix));

        #endregion
    }
}
