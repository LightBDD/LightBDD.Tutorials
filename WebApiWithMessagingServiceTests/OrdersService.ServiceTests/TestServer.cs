using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using OrdersService.Messages;
using Rebus.Activation;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Persistence.FileSystem;
using Rebus.Routing.TypeBased;
using Rebus.Transport.FileSystem;
using WireMock.Server;

namespace OrdersService.ServiceTests
{
    internal static class TestServer
    {
        private static WebApplicationFactory<Startup> Instance { get; set; }

        public static HttpClient GetClient() => Instance?.CreateDefaultClient()
                                                ?? throw new InvalidOperationException($"{nameof(TestServer)} not initialized.");

        public static IBus TestBus { get; private set; }
        public static WireMockServer MockApi { get; private set; }


        public static void Initialize()
        {
            Instance = new WebApplicationFactory<Startup>();
            // Actually the WebApplicationFactory has a problem that until CreateDefaultClient() is called, no underlying TestServer instance is created.
            // Also, the underlying TestServer creation has a race condition, calling it from tests running in parallel may cause multiple servers to be spawned.
            Instance.CreateDefaultClient();

            TestBus = SetupTestBus().GetAwaiter().GetResult();
            MockApi = WireMockServer.Start(5002, true);
        }

        private static async Task<IBus> SetupTestBus()
        {
            var testBus = Configure.With(new BuiltinHandlerActivator().Register(() => MessageDispatcher.Instance))
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
            await testBus.Subscribe<OrderCreatedEvent>();
            await testBus.Subscribe<OrderStatusUpdatedEvent>();
            await testBus.Subscribe<OrderProductDispatchEvent>();
            return testBus;
        }

        public static void Dispose()
        {
            Instance?.Dispose();
            TestBus?.Dispose();
        }
    }
}