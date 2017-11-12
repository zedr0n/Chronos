using System;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Net.Tracking.Commands
{
    public class StopTrackingCommand : CommandBase
    {
        public override Guid TargetId
        {
            get => Tracker.TrackerId;
            set { }
        } 
        
        public Guid? AssetId { get; set; }
    }
}