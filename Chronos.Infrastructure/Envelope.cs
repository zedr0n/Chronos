using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure
{
    public class Envelope
    {       
        public IEvent Event { get; }
        public StreamDetails Stream { get; }
        public bool SagaEvent => Stream.IsSaga;
        
        public Envelope(IEvent @event, StreamDetails stream)
        {
            Event = @event;
            Stream = stream;
        }
    }
}