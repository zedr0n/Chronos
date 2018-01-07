using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Assets.Commands
{
    public class UpdateAssetChangeCommand : CommandBase 
    {
        public UpdateAssetChangeCommand(double hourChange, double dayChange, double weekChange)
        {
            DayChange = dayChange;
            WeekChange = weekChange;
            HourChange = hourChange;
        }

        public double DayChange { get; }
        public double WeekChange { get; }
        public double HourChange { get; }
    }
}