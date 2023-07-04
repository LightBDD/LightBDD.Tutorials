using System.Threading.Tasks;
using LightBDD.Core.Execution;
using OrderService.Messages;
using Rebus.Activation;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Persistence.FileSystem;
using Rebus.Routing.TypeBased;
using Rebus.Transport.FileSystem;

namespace OrderService.ServiceTests.Infrastructure
{
    internal class TestBus : IGlobalResourceSetUp
    {
        /// <summary>
        /// Message Bus to communicate with OrdersService.
        /// </summary>
        public IBus MessageBus { get; }

        /// <summary>
        /// Message dispatcher allowing to hook-up into events subscribed on Rebus file queues.
        /// </summary>
        public MessageDispatcher Dispatcher { get; } = new();

        public TestBus()
        {
            MessageBus = Configure.With(new BuiltinHandlerActivator().Register(() => Dispatcher))
                .Transport(t => t.UseFileSystem(".queues", "orders-test-queue"))
                .Subscriptions(t => t.UseJsonFile(".subscriptions.json"))
                .Routing(r => r.TypeBased()
                    .Map<OrderCreatedEvent>("orders-queue")
                    .Map<OrderStatusUpdatedEvent>("orders-queue")
                    .Map<OrderProductDispatchEvent>("orders-queue")
                    .Map<ApproveOrderCommand>("orders-queue")
                    .Map<RejectOrderCommand>("orders-queue")
                )
                .Start();
        }

        async Task IGlobalResourceSetUp.SetUpAsync()
        {
            await MessageBus.Subscribe<OrderCreatedEvent>();
            await MessageBus.Subscribe<OrderStatusUpdatedEvent>();
            await MessageBus.Subscribe<OrderProductDispatchEvent>();
        }

        Task IGlobalResourceSetUp.TearDownAsync() => Task.CompletedTask;
    }
}