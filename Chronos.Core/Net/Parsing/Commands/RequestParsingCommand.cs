using System;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Net.Parsing.Commands
{
    public abstract class RequestParsingCommand : CommandBase
    {
        protected RequestParsingCommand(Guid assetId, string json) 
        {
            Json = json;
            AssetId = assetId;
        }

        public string Json { get; }
        public Guid AssetId { get; }
    }
}