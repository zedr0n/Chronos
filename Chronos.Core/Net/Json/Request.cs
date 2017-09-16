using System;
using Chronos.Core.Net.Json.Events;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Net.Json
{
    public class Request<T> : AggregateBase where T : class
    {
        private string _url;
        private STATUS _status;
        
        public enum STATUS
        {
            OPEN,
            TRACKING,
            COMPLETED
        }

        public Request(Guid id, string url)
        {
            When(new JsonRequested<T>
            {
                RequestId = id,
                Url = url
            });
        }

        public void Execute(IJsonConnector connector)
        {
            var result = connector.Get<T>(_url);
            connector.Save(Id,result);
            
            When(new JsonRequestCompleted
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

        public void When(JsonRequestTracked<T> e)
        {
            _status = STATUS.TRACKING;
            base.When(e);
        }
    }
}