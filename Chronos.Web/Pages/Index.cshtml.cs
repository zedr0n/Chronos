using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chronos.Core.Accounts.Commands;
using Chronos.Infrastructure.Commands;
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

        public void OnGet()
        {
            var account = new CreateAccountCommand
            {
                Name = "Account",
                Currency = "GBP",
                TargetId = Guid.NewGuid()
            };
            
            _commandBus.Send(account);
        }
    }
}