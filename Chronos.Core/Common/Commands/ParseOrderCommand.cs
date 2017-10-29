using System;

namespace Chronos.Core.Common.Commands
{
    public class ParseOrderCommand : RequestParsingCommand
    {
        public ParseOrderCommand(Guid assetId, string json) : base(assetId, json)
        {
        }
    }
}