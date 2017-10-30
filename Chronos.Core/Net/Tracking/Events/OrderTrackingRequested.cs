namespace Chronos.Core.Net.Tracking.Events
{
    public class OrderTrackingRequested : AssetTrackingRequested
    {
        public int OrderNumber { get; set; }
    }
}