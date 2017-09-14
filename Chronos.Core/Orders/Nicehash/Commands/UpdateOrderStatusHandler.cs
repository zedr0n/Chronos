using System;
using Chronos.Core.Assets;
using Chronos.Core.Orders.NiceHash.Projections;
using Chronos.Core.Orders.NiceHash.Queries;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;
using Chronos.Infrastructure.Queries;

namespace Chronos.Core.Orders.NiceHash.Commands
{
	public class UpdateOrderStatusHandler : ICommandHandler<UpdateOrderStatusCommand>
	{
		private readonly IDomainRepository _domainRepository;
		private readonly IQueryHandler<OrderInfoQuery,OrderInfo> _orderInfoHandler;

		public UpdateOrderStatusHandler(IDomainRepository domainRepository, IQueryHandler<OrderInfoQuery, OrderInfo> orderInfoHandler)
		{
			_domainRepository = domainRepository;
			_orderInfoHandler = orderInfoHandler;
		}

		public void Handle(UpdateOrderStatusCommand command)
		{
			var order = _domainRepository.Find<Order>(command.TargetId);
			if(order == null)
				throw new InvalidOperationException("Order not found");

			var orderInfo = _orderInfoHandler.Handle(new OrderInfoQuery
			{
				OrderId = command.TargetId
			});
			
			order.UpdateStatus(new Amount(orderInfo.PriceAsset,command.Spent),command.Speed );
		
			_domainRepository.Save(order);
		}
	}    
}