using System;
using Chronos.Core.Common.Commands;

namespace Chronos.Core.Net.Parsing.Commands
{
    public class ParseOrderCommand : RequestParsingCommand
    {
        public int OrderNumber { get; set; }
        public ParseOrderCommand(Guid assetId, int orderNumber, string json) : base(assetId, json)
        {
            OrderNumber = orderNumber;
        }
    }
}