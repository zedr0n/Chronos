using System;
using System.Linq;

namespace Chronos.Infrastructure.ProjectionServices
{
    public class PersistentVersionProvider<TKey,T> : IVersionProvider<T> where T : class, IReadModel where TKey : IEquatable<TKey>
    {
        private readonly IReadRepository _readRepository;
        private readonly IKeySelector<TKey, T> _keySelector;

        public PersistentVersionProvider(IReadRepository readRepository, IKeySelector<TKey, T> keySelector)
        {
            _readRepository = readRepository;
            _keySelector = keySelector;
        }

        public virtual int Get(StreamDetails s)
        {
            var keys = _keySelector.Get(s).ToList();
            if (keys.Count > 1)
                return -1;
            
            var readModel = _readRepository.Find<TKey, T>(keys.Single());
            return readModel.Version;
        }
    }
    
    public class PersistentStreamVersionProvider<T> : PersistentVersionProvider<Guid,T> where T : class, IReadModel
    {
        public PersistentStreamVersionProvider(IReadRepository readRepository, SingleStreamKeySelector<T> keySelector) : base(readRepository, keySelector)
        {
        }
    }
}