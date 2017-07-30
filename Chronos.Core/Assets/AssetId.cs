using System;

namespace Chronos.Core.Assets
{
    public struct AssetId : IEquatable<AssetId>
    {
        public string Ticker { get; }

        public AssetId(string ticker)
        {
            Ticker = ticker;
        }

        public bool Equals(AssetId other)
        {
            return string.Equals(Ticker, other.Ticker);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is AssetId && Equals((AssetId) obj);
        }

        public override int GetHashCode()
        {
            return (Ticker != null ? Ticker.GetHashCode() : 0);
        }

        public static implicit operator AssetId(string ticker)
        {
            return new AssetId(ticker);
        }
    }
}