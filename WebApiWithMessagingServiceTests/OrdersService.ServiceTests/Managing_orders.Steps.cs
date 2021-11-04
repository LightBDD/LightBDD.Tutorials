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
using LightBDD.XUnit2;
using OrdersService.Messages;
using OrdersService.Models;
using Shouldly;
using Xunit;

namespace OrdersService.ServiceTests
{
    public partial class Managing_orders : FeatureFixture, IDisposable
    {
        private readonly Guid _accountId = Guid.NewGuid();
        private readonly MessageListener _listener;
        private HttpResponseMessage _response;
        private Order _order;

        public Managing_orders()
        {
            _listener = MessageListener.Start(MessageDispatcher.Instance);
        }

        private Task Given_a_valid_account()
        {
            return Task.CompletedTask;
        }

        private async Task When_create_order_endpoint_is_called_for_products(params string[] products)
        {
            var request = new CreateOrderRequest { AccountId = _accountId, Products = products };
            _response = await TestServer.GetClient().PostAsJsonAsync("/orders", request);
        }

        private Task Then_response_should_have_status(Verifiable<HttpStatusCode> status)
        {
            status.SetActual(_response.StatusCode);
            return Task.CompletedTask;
        }

        private async Task Then_response_should_contain_order()
        {
            _order = await _response.Content.ReadFromJsonAsync<Order>();
            Assert.NotEqual(Guid.Empty, _order?.Id);
        }

        private async Task Then_OrderCreatedEvent_should_be_published()
        {
            await _listener.EnsureReceived<OrderCreatedEvent>(x => x.OrderId == _order.Id);
        }

        private async Task Then_get_order_endpoint_should_return_order_with_status(Verifiable<OrderStatus> status)
        {
            var order = await TestServer.GetClient().GetFromJsonAsync<Order>($"/orders/{_order.Id}");
            status.SetActual(order!.Status);
        }

        private Task<CompositeStep> Given_a_created_order()
        {
            return Task.FromResult(CompositeStep.DefineNew()
                .AddAsyncSteps(
                    _ => Given_a_valid_account(),
                    _ => When_create_order_endpoint_is_called_for_products("Product-A", "Product-B", "Product-C"),
                    _ => Then_response_should_have_status(HttpStatusCode.Created),
                    _ => Then_response_should_contain_order())
                .Build());
        }

        public void Dispose()
        {
            _listener?.Dispose();
        }

        private async Task When_RejectOrderCommand_is_sent_for_this_order()
        {
            await TestServer.TestBus.Send(new RejectOrderCommand { OrderId = _order.Id });
        }

        private async Task When_ApproveOrderCommand_is_sent_for_this_order()
        {
            await TestServer.TestBus.Send(new ApproveOrderCommand { OrderId = _order.Id });
        }

        private async Task Then_OrderStatusUpdatedEvent_should_be_published_with_status(OrderStatus status)
        {
            await _listener.EnsureReceived<OrderStatusUpdatedEvent>(x => x.OrderId == _order.Id && x.Status == status);
        }

        private async Task Then_OrderProductDispatchEvent_should_be_published_for_each_product()
        {
            var messages = await _listener.EnsureReceivedMany<OrderProductDispatchEvent>(_order.Products.Length, x => x.OrderId == _order.Id);
            messages.Select(m => m.Product).ToArray().ShouldBe(_order.Products, true);
        }
    }
}