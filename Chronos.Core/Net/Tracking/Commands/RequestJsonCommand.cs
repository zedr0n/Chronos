using System;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Net.Tracking.Commands
{
    public class RequestJsonCommand : CommandBase
    {
        public RequestJsonCommand(string url, Guid requestorId)
        {
            Url = url;
            RequestorId = requestorId;
        }

        public Guid RequestorId { get; }
        public string Url { get; }
    }
}