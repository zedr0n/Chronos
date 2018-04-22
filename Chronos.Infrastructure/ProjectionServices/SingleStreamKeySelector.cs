using System;

namespace Chronos.Infrastructure.ProjectionServices
{
    public class SingleStreamKeySelector<T> : PersistentKeySelector<Guid,T> where T : class, IReadModel
    {
        public SingleStreamKeySelector(SingleStreamSelector<T> streamSelector) : base(streamSelector)
        {
        }

        protected override Guid Key(StreamDetails s)
        {
            return s.Key;
        }
    }
}