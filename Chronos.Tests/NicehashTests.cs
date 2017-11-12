using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Chronos.Core.Common.Commands;
using Chronos.Core.Common.Events;
using Chronos.Core.Net.Parsing.Events;
using Chronos.Core.Net.Tracking.Commands;
using Chronos.Core.Nicehash.Commands;
using Chronos.Core.Nicehash.Projections;
using Chronos.Core.Nicehash.Queries;
using Chronos.CrossCuttingConcerns.DependencyInjection;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;
using Chronos.Infrastructure.Logging;
using Chronos.Infrastructure.Queries;
using NodaTime;
using Xunit;
using Xunit.Abstractions;

namespace Chronos.Tests
{
    public class NicehashTests : TestBase
    {
        public NicehashTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override ICompositionRoot CreateRoot(string dbName)
        {
            return new CompositionRoot()
                .WriteWith().Database(dbName); 
        }
        
        [Fact]
        public void CanCreateOrder()
        {
            var container = CreateContainer(nameof(CanCreateOrder));
            var commandBus = container.GetInstance<CommandBus>();
            var queryProcessor = container.GetInstance<IQueryProcessor>();

            var orderId = Guid.NewGuid();
            var orderNumber = 4181102;
            
            var command = new CreateOrderCommand
            {
                TargetId = orderId,
                OrderNumber = orderNumber
            };
            
            commandBus.Send(command);

            var orderStatus = queryProcessor.Process<OrderStatusQuery, OrderStatus>(new OrderStatusQuery
            {
                OrderNumber = orderNumber
            });
            
            Assert.NotNull(orderStatus);
            Assert.Equal(orderId,orderStatus.OrderId);
        }
       
        [Fact]
        public void CanTrackOrder()
        {
            var container = CreateContainer(nameof(CanTrackOrder));
            var commandBus = container.GetInstance<CommandBus>();
            var queryProcessor = container.GetInstance<IQueryProcessor>();
            var eventStore = container.GetInstance<IEventStore>();
            var debugLog = container.GetInstance<IDebugLog>();

            var orderId = Guid.NewGuid();
            const int orderNumber = 4181102;
            
            var command = new CreateOrderCommand
            {
                TargetId = orderId,
                OrderNumber = orderNumber
            };
            
            commandBus.Send(command);

            var trackCommand = new TrackOrderCommand(orderId,Duration.FromSeconds(1)) 
            {
                OrderNumber = orderNumber
            };

            var alerts = eventStore.Alerts;//.Publish();
            //alerts.Connect();
            
            commandBus.Send(trackCommand);
            
            var orderStatus = queryProcessor.Process<OrderStatusQuery, OrderStatus>(new OrderStatusQuery
            {
                OrderNumber = orderNumber
            });
            Assert.NotNull(orderStatus);
            Assert.Equal(orderId,orderStatus.OrderId);

            var failAlerts = alerts.OfType<ParsingOrderStatusFailed>().Publish();
            var completed = false;
            
            failAlerts.Subscribe(e =>
            {
                completed = true;
                debugLog.WriteLine("Order parsing failed");
            });
            
            //var obs = Observable.Interval(TimeSpan.FromSeconds(1))
            //    .StartWith(-1).TakeUntil(failAlerts)
            //    .Timeout(DateTimeOffset.UtcNow.AddSeconds(10));

            var obs = Observable.Return(1).CompleteOn(completed);
            
            failAlerts.Connect();
            
            commandBus.Send(new StartTrackingCommand());
            
            obs.Wait();
        }
    }
}