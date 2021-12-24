using System;
using System.Runtime.CompilerServices;
using Appalachia.CI.Constants;
using Appalachia.CI.Integration.FileSystem;
using Appalachia.Utility.Extensions;
using Appalachia.Utility.Strings;
using Unity.Profiling;

namespace Appalachia.CI.Integration.Core
{
    public struct AppalachiaNameConfig
    {
        #region Constants and Static Readonly

        private const string CHECK_FINISHED_STRING = "Call [{0}()] before accessing the [{1}] property.";

        private const string DEFAULT_EXTENSION = ".asset";

        #endregion

        #region Fields and Autoproperties

        public bool addTime;
        public string extension;
        public Type appendType;
        public Type owner;
        public Type prependType;

        private bool _appendDate;

        private bool _finished;
        private string _name;
        private string _nameWithoutExtension;

        #endregion

        public string Name
        {
            get
            {
                CheckFinished();
                return _name;
            }
        }

        public string NameWithoutExtension
        {
            get
            {
                CheckFinished();
                return _nameWithoutExtension;
            }
        }

        public Type Owner => owner;

        public static AppalachiaNameConfig FromLegacyOptions(Type t, bool prependType, bool appendType)
        {
            using (_PRF_FromLegacyOptions.Auto())
            {
                var newInstance = new AppalachiaNameConfig();

                if (prependType)
                {
                    newInstance.PrependType(t);
                }

                if (appendType)
                {
                    newInstance.AppendType(t);
                }

                return newInstance;
            }
        }

        public AppalachiaNameConfig AddOwnerType(Type type)
        {
            using (_PRF_AddOwnerType.Auto())
            {
                owner = type;

                return this;
            }
        }

        public AppalachiaNameConfig AppendType(Type type)
        {
            using (_PRF_AppendType.Auto())
            {
                appendType = type;

                return this;
            }
        }

        public void Finish(string filePath)
        {
            using (_PRF_Finish.Auto())
            {
                var builder = new Utf16ValueStringBuilder(true);

                builder.Append(filePath);

                TrimBuilder(ref builder);

                var cleanRelativeFilePath = builder.ToString();

                builder.Clear();

                var cleanFileName = AppaPath.GetFileNameWithoutExtension(cleanRelativeFilePath);

                if (prependType != null)
                {
                    builder.Append(prependType.Name);
                    builder.Append(APPASTR.SYMBOL_HYPHEN);
                }

                builder.Append(cleanFileName);

                if (addTime)
                {
                    builder.Append(ZString.Format(APPASTR.Formats.FileTime, DateTime.Now));
                }

                if (appendType != null)
                {
                    builder.Append(APPASTR.SYMBOL_HYPHEN);
                    builder.Append(appendType.Name);
                }

                var realExtension = DEFAULT_EXTENSION;

                if (extension.IsNotNullOrWhiteSpace() && (extension != APPASTR.SYMBOL_PERIOD))
                {
                    realExtension = extension;
                }
                else
                {
                    var proposedExtensions = AppaPath.GetExtension(filePath);

                    if (proposedExtensions.IsNotNullOrWhiteSpace() &&
                        (proposedExtensions != APPASTR.SYMBOL_PERIOD))
                    {
                        realExtension = proposedExtensions;
                    }
                }

                TrimBuilder(ref builder);

                _nameWithoutExtension = builder.ToString();

                if (realExtension[0] != '.')
                {
                    builder.Append('.');
                }

                builder.Append(realExtension);

                builder.Replace(APPASTR.SYMBOL_PERIODPERIOD, APPASTR.SYMBOL_PERIOD);

                _name = builder.ToString();

                builder.Dispose();

                _finished = true;
            }
        }

        public AppalachiaNameConfig IncludeTime()
        {
            using (_PRF_IncludeTime.Auto())
            {
                addTime = true;
                return this;
            }
        }

        public AppalachiaNameConfig PrependType(Type type)
        {
            using (_PRF_PrependType.Auto())
            {
                prependType = type;

                return this;
            }
        }

        public AppalachiaNameConfig SetExtension(string ext)
        {
            using (_PRF_SetExtension.Auto())
            {
                extension = ext;

                return this;
            }
        }

        private static void TrimBuilder(ref Utf16ValueStringBuilder builder)
        {
            using (_PRF_TrimBuilder.Auto())
            {
                while ((builder[0] == '.') || (builder[0] == ' '))
                {
                    builder.Remove(0, 1);
                }

                while ((builder[builder.Length - 1] == '.') || (builder[builder.Length - 1] == ' '))
                {
                    builder.Remove(builder.Length - 1, 1);
                }
            }
        }

        private void CheckFinished([CallerMemberName] string caller = null)
        {
            using (_PRF_CheckFinished.Auto())
            {
                if (!_finished)
                {
                    throw new NotSupportedException(
                        ZString.Format(CHECK_FINISHED_STRING, nameof(Finish), caller)
                    );
                }
            }
        }

        #region Profiling

        private const string _PRF_PFX = nameof(AppalachiaNameConfig) + ".";

        private static readonly ProfilerMarker _PRF_CheckFinished =
            new ProfilerMarker(_PRF_PFX + nameof(CheckFinished));

        private static readonly ProfilerMarker _PRF_FromLegacyOptions =
            new ProfilerMarker(_PRF_PFX + nameof(FromLegacyOptions));

        private static readonly ProfilerMarker _PRF_IncludeTime =
            new ProfilerMarker(_PRF_PFX + nameof(IncludeTime));

        private static readonly ProfilerMarker _PRF_AppendType =
            new ProfilerMarker(_PRF_PFX + nameof(AppendType));

        private static readonly ProfilerMarker _PRF_SetExtension =
            new ProfilerMarker(_PRF_PFX + nameof(SetExtension));

        private static readonly ProfilerMarker _PRF_AddOwnerType =
            new ProfilerMarker(_PRF_PFX + nameof(AddOwnerType));

        private static readonly ProfilerMarker _PRF_PrependType =
            new ProfilerMarker(_PRF_PFX + nameof(PrependType));

        private static readonly ProfilerMarker _PRF_Finish = new ProfilerMarker(_PRF_PFX + nameof(Finish));

        private static readonly ProfilerMarker _PRF_TrimBuilder =
            new ProfilerMarker(_PRF_PFX + nameof(TrimBuilder));

        #endregion
    }
}
