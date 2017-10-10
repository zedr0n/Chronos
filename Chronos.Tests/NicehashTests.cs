using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using Chronos.Core.Net.Json.Projections;
using Chronos.Core.Net.Json.Queries;
using Chronos.Core.Nicehash.Commands;
using Chronos.Core.Nicehash.Json;
using Chronos.Core.Nicehash.Projections;
using Chronos.Core.Nicehash.Queries;
using Chronos.Infrastructure.Commands;
using Chronos.Infrastructure.Queries;
using Xunit;
using Xunit.Abstractions;

namespace Chronos.Tests
{
    public class NicehashTests : TestBase
    {
        public NicehashTests(ITestOutputHelper output) : base(output)
        {
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

            var orderId = Guid.NewGuid();
            var orderNumber = 4181102;
            
            var command = new CreateOrderCommand
            {
                TargetId = orderId,
                OrderNumber = orderNumber
            };
            
            commandBus.Send(command);

            var trackCommand = new TrackOrderCommand
            {
                TargetId = orderId,
                UpdateInterval = 1
            };
            
            commandBus.Send(trackCommand);
            
            var orderStatus = queryProcessor.Process<OrderStatusQuery, OrderStatus>(new OrderStatusQuery
            {
                OrderNumber = orderNumber
            });
            Assert.NotNull(orderStatus);
            Assert.Equal(orderId,orderStatus.OrderId);



            var obs = Observable.Create((IObserver<long> o) =>
            {
                var requestInfo = queryProcessor.Process<RequestInfoQuery<Orders>, RequestInfo<Orders>>(new RequestInfoQuery<Orders>
                {
                    RequestId = orderId
                });
                
                var observable = Observable.Interval(TimeSpan.FromSeconds(1))
                    .Do(x =>
                    {
                        orderStatus = queryProcessor.Process<OrderStatusQuery, OrderStatus>(new OrderStatusQuery
                        {
                            OrderNumber = orderNumber
                        });
                        
                        requestInfo = queryProcessor.Process<RequestInfoQuery<Orders>, RequestInfo<Orders>>(new RequestInfoQuery<Orders>
                        {
                            RequestId = orderId
                        });
                    })
                    .DistinctUntilChanged();

                var subscription = observable.Subscribe(x =>
                {
                    o.OnNext(x);
                    if(requestInfo.Completed || requestInfo.Failed)
                        o.OnCompleted();
                });
                
                return Disposable.Create(() => subscription.Dispose());
            });
            
            obs.Wait();
            
            var finalRequestInfo = queryProcessor.Process<RequestInfoQuery<Orders>, RequestInfo<Orders>>(new RequestInfoQuery<Orders>
            {
                RequestId = orderId
            });
            
            Assert.True(finalRequestInfo.Failed);
        }
         
        
    }
}