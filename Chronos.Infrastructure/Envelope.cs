using System.Collections;
using System.Collections.Generic;
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

    public class BufferEnvelope
    {
        public BufferEnvelope(IList<IEvent> events, StreamDetails stream)
        {
            Events = events;
            Stream = stream;
        }

        public IList<IEvent> Events { get; }
        public StreamDetails Stream { get; }
        public bool SagaEvent => Stream.IsSaga;
        
        
    }
}