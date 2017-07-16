using NodaTime;

namespace Chronos.Infrastructure.Commands
{
    public class RequestTimeoutCommand : CommandBase
    {
        public Instant When { get; set; }
    }
}