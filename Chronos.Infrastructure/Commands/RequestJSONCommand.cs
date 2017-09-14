using System;

namespace Chronos.Infrastructure.Commands
{
    public class RequestJsonCommand<T> : CommandBase
    {
        public Guid RequestId { get; set; }
        public string Url { get; set; }
    }
}