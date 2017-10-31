using Chronos.Core.Assets.Projections;
using Chronos.Core.Assets.Queries;
using Chronos.Core.Projections;
using Chronos.Infrastructure.Queries;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chronos.Web.Pages
{
    public class StatsModel : PageModel
    {
        private readonly IQueryProcessor _queryProcessor;
        
        
        public StatsModel(IQueryProcessor queryProcessor)
        {
            _queryProcessor = queryProcessor;
        }
        
        public Stats Stats { get; set; }
        public CoinInfo CoinInfo { get; set; }
        
        public void OnGet()
        {
            Stats = _queryProcessor.Process<StatsQuery, Stats>(new StatsQuery());
            CoinInfo = _queryProcessor.Process<CoinInfoQuery, CoinInfo>(new CoinInfoQuery
            {
                Name = "Bitcoin"
            });
        }
    }
}