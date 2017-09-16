using System;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Net.Json.Commands
{
    public class CreateRequestCommand<T> : CommandBase
    {
        public string Url { get; set; }
    }
}