using System;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Net.Json.Commands
{
    public class TrackRequestCommand<T> : CommandBase
    {
        public int UpdateInterval { get; set; }
    }
}