using System;
using Chronos.Core.Common.Commands;

namespace Chronos.Core.Net.Parsing.Commands
{
    public class ParseOrderCommand : RequestParsingCommand
    {
        public ParseOrderCommand(Guid assetId, string json) : base(assetId, json)
        {
        }
    }
}