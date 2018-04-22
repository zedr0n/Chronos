using System.Collections.Generic;

namespace Chronos.Infrastructure.ProjectionServices
{
    public interface IKeySelector<TKey,T> where T : IReadModel
    {
        IEnumerable<TKey> Get(StreamDetails s);
    }
}