using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure.Projections.New
{
    public class PersistentProjection<TKey,T> : Projection, IPersistentProjection<T>
        where T : class, IReadModel, new()
        where TKey : IEquatable<TKey>
    {
        protected Func<StreamDetails, TKey> KeyFunc { get; set; }
        private readonly IStateWriter _writer;
        private readonly IReadRepository _readRepository;

        internal PersistentProjection(IEventStore eventStore,IStateWriter writer, IReadRepository readRepository)
            : base(eventStore)
        {
            _writer = writer;
            _readRepository = readRepository;
        }

        public override void Start(bool reset = false)
        {
            if (typeof(T).GetTypeInfo().GetCustomAttributes<ResetAttribute>().Any())
                reset = true;
            
            base.Start(reset);
        }
        
        protected override void When(StreamDetails stream, IEvent e)
        {
            _writer.Write<TKey,T>(KeyFunc(stream),x =>
            {
                x.Timeline = Timeline;
                x.When(e);
            });
            base.When(stream, e);
        }

        protected override int GetVersion(StreamDetails stream)
        {
            var readModel = _readRepository.Find<TKey,T>(KeyFunc(stream));
            if (readModel == null)
                return -1;
            return readModel.Version;
        }
    }
}