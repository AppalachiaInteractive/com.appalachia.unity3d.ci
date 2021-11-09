using System;

namespace Appalachia.CI.Integration.Repositories.Publishing
{
    [Serializable]
    public class PublishMetadata
    {
        public DateTime attempt;
        public int exitCode;
        public PublishStatus status;
        public string errorOutput;
        public string output;
    }
}
