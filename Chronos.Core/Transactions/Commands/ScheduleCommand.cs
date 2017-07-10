using Chronos.Infrastructure.Commands;
using NodaTime;

namespace Chronos.Core.Transactions.Commands
{
    public class ScheduleCommand : CommandBase
    {
        public ICommand Command { get; set; }
        public Instant Date { get; set; }
    }
}