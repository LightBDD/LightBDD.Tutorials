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

namespace OrdersService.ServiceTests
{
    internal static class TestServer
    {
        private static WebApplicationFactory<Startup> _testServer;

        /// <summary>
        /// HttpClient to the OrdersService under test.
        /// </summary>
        public static HttpClient Client { get; private set; }

        /// <summary>
        /// Message Bus to communicate with OrdersService under test.
        /// </summary>
        public static IBus MessageBus { get; private set; }

        /// <summary>
        /// Mock Account Service to control Account Service API calls
        /// </summary>
        public static MockAccountService MockAccountService { get; private set; }

        public static void Initialize()
        {
            _testServer = new WebApplicationFactory<Startup>();
            Client = _testServer.CreateDefaultClient();
            MessageBus = SetupTestBus().GetAwaiter().GetResult();
            MockAccountService = new MockAccountService();
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
            _testServer?.Dispose();
            MessageBus?.Dispose();
            MockAccountService?.Dispose();
        }
    }
}