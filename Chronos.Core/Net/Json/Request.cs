using System;
using System.Reactive.Linq;
using Chronos.Core.Net.Json.Events;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Core.Net.Json
{
    public class Request<T> : AggregateBase where T : class
    {
        private string _url;
        private STATUS _status;
        
        public enum STATUS
        {
            OPEN,
            FAILED,
            TRACKING,
            COMPLETED
        }
        public Request() {}
        
        protected override void When(IEvent e)
        {
            When((dynamic) e);
        }

        public Request(Guid id, string url)
        {
            When(new JsonRequested<T>
            {
                RequestId = id,
                Url = url
            });
        }

        public void Complete()
        {
            When(new JsonRequestCompleted
            {
                RequestId = Id
            });
        }

        public void Fail()
        {
            When(new JsonRequestFailed
            {
                RequestId = Id
            });
        }

        public void Track(int updateInterval)
        {
            // if we rehydrated the request
            // it will already be in tracking mode
            //if (_status == STATUS.TRACKING)
            //    return;
            
            When(new JsonRequestTracked<T>
            {
                RequestId = Id,
                UpdateInterval = updateInterval
            });
        }

        public void When(JsonRequested<T> e)
        {
            Id = e.RequestId;
            _url = e.Url;
            _status = STATUS.OPEN;
            base.When(e);
        }

        public void When(JsonRequestCompleted e)
        {
            _status = STATUS.COMPLETED;
            base.When(e);
        }

        public void When(JsonRequestFailed e)
        {
            _status = STATUS.FAILED;
            base.When(e);
        }

        public void When(JsonRequestTracked<T> e)
        {
            _status = STATUS.TRACKING;
            base.When(e);
        }
    }
}