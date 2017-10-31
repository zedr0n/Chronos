using System.Collections.Generic;
using System.Linq;
using Chronos.Core.Common.Events;
using Chronos.Core.Net.Parsing.Events;
using Chronos.Core.Net.Parsing.Json;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Net.Parsing.Commands
{
    public class ParseCoinHandler : ICommandHandler<ParseCoinCommand>
    {
        private readonly IEventBus _eventBus;
        private readonly IJsonParser _jsonParser;

        public ParseCoinHandler(IEventBus eventBus, IJsonParser jsonParser)
        {
            _eventBus = eventBus;
            _jsonParser = jsonParser;
        }

        public void Handle(ParseCoinCommand command)
        {
            var parsed = _jsonParser.Parse<List<CoinInfo>>(command.Json);
            var coinInfo = parsed.SingleOrDefault();
            
            if(coinInfo == null)
                _eventBus.Alert(new ParsingCoinInfoFailed(command.AssetId));
            else
                _eventBus.Alert(new CoinInfoParsed(command.AssetId,coinInfo.price_usd));
        }
    }
}