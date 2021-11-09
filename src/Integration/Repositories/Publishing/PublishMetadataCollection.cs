using System;
using System.Collections.Generic;

namespace Appalachia.CI.Integration.Repositories.Publishing
{
    [Serializable]
    public class PublishMetadataCollection
    {
        public PublishMetadataCollection()
        {
            _recent = new List<PublishMetadata>();
        }

        // ReSharper disable once CollectionNeverQueried.Local
        private List<PublishMetadata> _recent;

        private PublishMetadata _current;

        public PublishStatus currentStatus => _current?.status ?? PublishStatus.None;

        public string currentMessage => _current?.errorOutput ?? _current?.output;

        public PublishMetadata NewAttempt()
        {
            _recent ??= new List<PublishMetadata>();

            if (_current != null)
            {
                _recent.Add(_current);
            }

            _current = new PublishMetadata();

            return _current;
        }
    }
}
