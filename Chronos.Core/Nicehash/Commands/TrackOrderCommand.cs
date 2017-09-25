using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Nicehash.Commands
{
    public class TrackOrderCommand : CommandBase 
    {
        public int UpdateInterval { get; set; }
    }
}