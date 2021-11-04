using System;
using System.Net.Http;
using System.Threading.Tasks;
using LightBDD.Framework.Messaging;
using Microsoft.AspNetCore.Mvc.Testing;
using OrdersService.Messages;
using Rebus.Activation;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Handlers;
using Rebus.Persistence.FileSystem;
using Rebus.Routing.TypeBased;
using Rebus.Transport.FileSystem;

namespace OrdersService.ServiceTests
{
    internal static class TestServer
    {
        private static WebApplicationFactory<Startup> Instance { get; set; }

        public static HttpClient GetClient() => Instance?.CreateDefaultClient()
                                                ?? throw new InvalidOperationException($"{nameof(TestServer)} not initialized.");

        public static IBus TestBus { get; private set; }

        public static void Initialize()
        {
            Instance = new WebApplicationFactory<Startup>();
            // Actually the WebApplicationFactory has a problem that until CreateDefaultClient() is called, no underlying TestServer instance is created.
            // Also, the underlying TestServer creation has a race condition, calling it from tests running in parallel may cause multiple servers to be spawned.
            Instance.CreateDefaultClient();

            TestBus = Configure.With(new BuiltinHandlerActivator().Register(()=>MessageDispatcher.Instance))
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

            TestBus.Subscribe<OrderCreatedEvent>().GetAwaiter().GetResult();
            TestBus.Subscribe<OrderStatusUpdatedEvent>().GetAwaiter().GetResult();
            TestBus.Subscribe<OrderProductDispatchEvent>().GetAwaiter().GetResult();
        }

        public static void Dispose()
        {
            Instance?.Dispose();
            TestBus?.Dispose();
        }
    }

    class MessageDispatcher : IMessageSource, IHandleMessages<object>
    {
        public static readonly MessageDispatcher Instance = new MessageDispatcher();
        public event Action<object> OnMessage;
        

        private MessageDispatcher() { }
        public Task Handle(object message)
        {
            OnMessage?.Invoke(message);
            return Task.CompletedTask;
        }
    }
}