using System;
using System.Threading.Tasks;
using Chronos.Core.Assets.Commands;
using Chronos.Infrastructure.Commands;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chronos.Web.Pages
{
    public class CreateCoinModel : PageModel
    {
        private readonly ICommandBus _commandBus;
        
        [BindProperty]
        public CreateCoinCommand Command { get; set; }
        
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
            await _commandBus.SendAsync(Command);
            return RedirectToPage("/Index");
        }
        
    }
}