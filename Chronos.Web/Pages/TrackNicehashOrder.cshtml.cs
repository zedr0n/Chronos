using System;
using System.Threading.Tasks;
using Chronos.Core.Orders.NiceHash.Commands;
using Chronos.Infrastructure.Commands;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chronos.Web.Pages
{
    public class TrackNicehashOrderModel : PageModel
    {
        [BindProperty]
        public CreateOrderCommand Command { get; set; } = new CreateOrderCommand();

        private readonly ICommandBus _commandBus;
        
        public TrackNicehashOrderModel(ICommandBus commandBus)
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