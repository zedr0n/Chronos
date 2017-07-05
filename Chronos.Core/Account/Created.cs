using Chronos.Infrastructure.Events;

namespace Chronos.Core.Account
{
    public class Created : EventBase
    {
        public string Name { get; set; }
        public string Currency { get; set; }
    }
}