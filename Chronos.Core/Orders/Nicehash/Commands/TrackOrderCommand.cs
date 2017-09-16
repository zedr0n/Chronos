using System;
using System.Dynamic;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Orders.NiceHash.Commands
{
    public class TrackOrderCommand : CommandBase 
    {
        public int UpdateInterval { get; set; }
    }
}