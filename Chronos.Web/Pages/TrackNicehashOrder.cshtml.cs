using System;
using System.Threading.Tasks;
using Chronos.Core.Orders.NiceHash.Commands;
using Chronos.Core.Orders.NiceHash.Projections;
using Chronos.Core.Orders.NiceHash.Queries;
using Chronos.Infrastructure.Commands;
using Chronos.Infrastructure.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static System.Double;

namespace Chronos.Web.Pages
{
    public class TrackNicehashOrderModel : PageModel
    {
        [BindProperty]
        public int OrderNumber { get; set; }
        
        public OrderStatus OrderStatus { get; set; } = new OrderStatus
        {
            Speed = NaN,
            Spent = NaN
        };
        
        private readonly ICommandBus _commandBus;
        private readonly IQueryProcessor _queryProcessor;
        
        public TrackNicehashOrderModel(ICommandBus commandBus, IQueryProcessor queryProcessor)
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

            var query = new OrderStatusQuery
            {
                OrderNumber = OrderNumber
            };
            
            OrderStatus = _queryProcessor.Process<OrderStatusQuery, OrderStatus>(query);

            var orderId = OrderStatus?.OrderId ?? Guid.NewGuid();
            
            if (OrderStatus == null)
            {
                await _commandBus.SendAsync(new CreateOrderCommand
                {
                    TargetId = orderId,
                    OrderNumber = OrderNumber
                });
                OrderStatus = _queryProcessor.Process<OrderStatusQuery, OrderStatus>(query); 
            }

            await _commandBus.SendAsync(new TrackOrderCommand
            {
                TargetId = orderId,
                UpdateInterval = 10
            });

            return Page();
        }
    }
}