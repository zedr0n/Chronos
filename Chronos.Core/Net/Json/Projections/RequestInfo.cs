using System;
using Chronos.Core.Net.Json.Events;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Net.Json.Projections
{
    public class RequestInfo<T> : ReadModelBase<Guid> where T : class
    {
        public string Url { get; private set; }
        public bool Completed { get; private set; }
        public bool Failed { get; private set; }

        public void When(StateReset e)
        {
            Url = null;
            Completed = false;
        }

        public void When(JsonRequested<T> e)
        {
            Url = e.Url;
            Key = e.RequestId;
        }

        public void When(JsonRequestCompleted e)
        {
            Completed = true;
        }

        public void When(JsonRequestFailed e)
        {
            Failed = true;
        }
    }
}