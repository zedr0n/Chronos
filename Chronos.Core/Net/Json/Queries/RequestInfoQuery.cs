using System;
using Chronos.Core.Net.Json.Projections;
using Chronos.Infrastructure.Queries;

namespace Chronos.Core.Net.Json.Queries
{
    public class RequestInfoQuery<T> : IQuery<RequestInfo<T>> where T : class
    {
        public Guid RequestId { get; set; }    
        public bool Completed { get; set; }
    }
}