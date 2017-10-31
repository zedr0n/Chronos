using System;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Net.Parsing.Events
{
    public class ParsingCoinInfoFailed : EventBase
    {
        public ParsingCoinInfoFailed(Guid coinId)
        {
            CoinId = coinId;
        }

        public Guid CoinId { get; }
    }
}