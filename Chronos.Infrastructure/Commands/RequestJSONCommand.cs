using System;

namespace Chronos.Infrastructure.Commands
{
    public class RequestJSONCommand<T> : CommandBase
    {
        public string Url { get; set; }
        public Action<T> Handler;
    }
}