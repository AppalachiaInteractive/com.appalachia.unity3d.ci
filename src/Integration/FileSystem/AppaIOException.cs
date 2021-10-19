using System;
using System.IO;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace Appalachia.CI.Integration.FileSystem
{
    public class AppaIOException : IOException
    {
        public AppaIOException()
        {
        }

        public AppaIOException(string message) : base(message)
        {
        }

        public AppaIOException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public AppaIOException(string message, int hresult) : base(message, hresult)
        {
        }

        protected AppaIOException([NotNull] SerializationInfo info, StreamingContext context) : base(
            info,
            context
        )
        {
        }
    }
}
