namespace Chronos.Core.Net.Tracking.Events
{
    public class CoinTrackingRequested : AssetTrackingRequested
    {
        public string Ticker { get; set; }
    }
}