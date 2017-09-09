using System;
using System.Threading.Tasks;
using Chronos.Core.Assets.Commands;
using Chronos.Infrastructure.Commands;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NodaTime;
using NodaTime.Extensions;
using NodaTime.Text;

namespace Chronos.Web.Pages
{
    public class CreateCoinModel : PageModel
    {
        private readonly ICommandBus _commandBus;
        
        [BindProperty]
        public HistoricalCommand<CreateCoinCommand> Command { get; set; }

        [BindProperty]
        public string Date { get; set; }
        
        public CreateCoinModel(ICommandBus commandBus)
        {
            _commandBus = commandBus;
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

            Command.TargetId = Guid.NewGuid();
            var pattern = LocalDatePattern.CreateWithInvariantCulture("MM/dd/yyyy");
            var localDate = pattern.Parse(Date);
            Command.At = new ZonedDateTime(localDate.Value.ToDateTimeUnspecified().ToLocalDateTime()
                ,DateTimeZone.Utc,Offset.Zero).ToInstant();
            await _commandBus.SendAsync(Command);
            return RedirectToPage("/Index");
        }
        
    }
}