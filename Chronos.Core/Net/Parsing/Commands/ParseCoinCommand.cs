using System;

namespace Chronos.Core.Net.Parsing.Commands
{
    public class ParseCoinCommand : RequestParsingCommand
    {
        public string Ticker { get; set; }

        public ParseCoinCommand(Guid assetId,string ticker, string json) : base(assetId, json)
        {
            Ticker = ticker;
        }
    }
}