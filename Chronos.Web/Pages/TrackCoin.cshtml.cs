using System.Threading.Tasks;
using Chronos.Core.Assets.Projections;
using Chronos.Core.Assets.Queries;
using Chronos.Core.Net.Tracking.Commands;
using Chronos.Infrastructure.Commands;
using Chronos.Infrastructure.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NodaTime;

namespace Chronos.Web.Pages
{
    public class TrackCoinModel : PageModel
    {
        private readonly ICommandBus _commandBus;

        private readonly IQueryProcessor _queryProcessor;
        //private readonly IQueryHandler<CoinInfoQuery,CoinInfo> _queryHandler;
        
        
        [BindProperty]
        public string Name { get; set; }
        [BindProperty]
        public int NumberOfSeconds { get; set; }
        
        public TrackCoinModel(ICommandBus commandBus, IQueryProcessor queryProcessor)
        {
            _commandBus = commandBus;
            _queryProcessor = queryProcessor;
        }
        
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var coinId = _queryProcessor.Process<CoinInfoQuery,CoinInfo>(new CoinInfoQuery
            {
                Name = Name
            })?.Key;

            if (coinId != null)
            {
                var command = //new TrackCoinCommand(coinId,Duration.FromMinutes(30))
                    new TrackCoinCommand(coinId.Value,Duration.FromSeconds(NumberOfSeconds))
                    {
                        Ticker = Name
                    }; 
                await _commandBus.SendAsync(command);
            }
            
            //Command.TargetId = Guid.NewGuid();
            //var pattern = LocalDatePattern.CreateWithInvariantCulture("MM/dd/yyyy");
            //var localDate = pattern.Parse(Date);
            //Command.At = new ZonedDateTime(localDate.Value.ToDateTimeUnspecified().ToLocalDateTime()
            //    ,DateTimeZone.Utc,Offset.Zero).ToInstant();

            return RedirectToPage("/Index");
        }
        
    }
}