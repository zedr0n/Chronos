using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chronos.Core.Accounts.Commands;
using Chronos.Infrastructure.Commands;
using Chronos.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chronos.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ICommandBus _commandBus;

        public IndexModel(ICommandBus commandBus)
        {
            _commandBus = commandBus;
        }
        
        [BindProperty]
        public CreateAccountCommand Command { get; set; } = new CreateAccountCommand();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Command.TargetId = Guid.NewGuid();
            await _commandBus.SendAsync(Command);
            return RedirectToPage("/Index");
        }

        public void OnPostClearDatabase()
        {
            _commandBus.Send(new ClearDatabaseCommand()); 
        }

        public IActionResult OnPostCreateCoin()
        {
            return RedirectToPage("/CreateCoin");
        }

        public IActionResult OnPostTrackCoin()
        {
            return RedirectToPage("/TrackCoin");
        }
        
        public void OnGet()
        {
        }
    }
}