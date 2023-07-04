using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using LightBDD.Framework;
using LightBDD.Framework.Messaging;
using LightBDD.Framework.Parameters;
using LightBDD.Framework.Scenarios;
using OrdersService.Messages;
using OrdersService.Models;
using OrdersService.ServiceTests.Infrastructure;
using Rebus.Bus;
using Shouldly;
using Xunit;

namespace OrdersService.ServiceTests.Contexts
{
    internal class OrderContext : IDisposable
    {
        private readonly MockAccountService _accountService;
        private readonly IBus _messageBus;
        private readonly MessageListener _listener;
        private readonly OrdersServiceClient _client;
        private readonly Guid _accountId = Guid.NewGuid();
        private HttpResponseMessage _response;
        private Order _order;

        // Uses DI container to resolve these dependencies
        public OrderContext(OrdersServiceClient client, MockAccountService accountService, TestBus testBus)
        {
            _client = client;
            _accountService = accountService;
            _messageBus = testBus.MessageBus;
            _listener = MessageListener.Start(testBus.Dispatcher);
        }

        public Task Given_a_valid_account()
        {
            _accountService.ConfigureGetAccount(_accountId, true);
            return Task.CompletedTask;
        }

        public Task Given_an_invalid_account()
        {
            _accountService.ConfigureGetAccount(_accountId, false);
            return Task.CompletedTask;
        }

        public async Task When_create_order_endpoint_is_called_for_products(params string[] products)
        {
            var request = new CreateOrderRequest { AccountId = _accountId, Products = products };
            _response = await _client.CreateOrder(request);
        }

        public Task Then_response_should_have_status(Verifiable<HttpStatusCode> status)
        {
            status.SetActual(_response.StatusCode);
            return Task.CompletedTask;
        }

        public async Task Then_response_should_contain_order()
        {
            _order = await _response.Content.ReadFromJsonAsync<Order>();
            Assert.NotEqual(Guid.Empty, _order?.Id);
        }

        public async Task Then_OrderCreatedEvent_should_be_published()
        {
            await _listener.EnsureReceived<OrderCreatedEvent>(x => x.OrderId == _order.Id);
        }

        public async Task Then_get_order_endpoint_should_return_order_with_status(Verifiable<OrderStatus> status)
        {
            var order = await _client.GetOrder(_order.Id);
            status.SetActual(order!.Status);
        }

        public Task<CompositeStep> Given_a_created_order()
        {
            return Task.FromResult(CompositeStep.DefineNew()
                .AddAsyncSteps(
                    _ => Given_a_valid_account(),
                    _ => When_create_order_endpoint_is_called_for_products("Product-A", "Product-B", "Product-C"),
                    _ => Then_response_should_have_status(HttpStatusCode.Created),
                    _ => Then_response_should_contain_order())
                .Build());
        }

        public async Task When_RejectOrderCommand_is_sent_for_this_order()
        {
            await _messageBus.Send(new RejectOrderCommand { OrderId = _order.Id });
        }

        public async Task When_ApproveOrderCommand_is_sent_for_this_order()
        {
            await _messageBus.Send(new ApproveOrderCommand { OrderId = _order.Id });
        }

        public async Task Then_OrderStatusUpdatedEvent_should_be_published_with_status(OrderStatus status)
        {
            await _listener.EnsureReceived<OrderStatusUpdatedEvent>(x => x.OrderId == _order.Id && x.Status == status);
        }

        public async Task Then_OrderProductDispatchEvent_should_be_published_for_each_product()
        {
            var messages = await _listener.EnsureReceivedMany<OrderProductDispatchEvent>(_order.Products.Length, x => x.OrderId == _order.Id);
            messages.Select(m => m.Product).ToArray()
                .ShouldBe(_order.Products, true);
        }

        public void Dispose()
        {
            _listener?.Dispose();
        }
    }
}