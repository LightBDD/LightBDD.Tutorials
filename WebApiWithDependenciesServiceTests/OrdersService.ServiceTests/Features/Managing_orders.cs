using System.Net;
using System.Threading.Tasks;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;
using OrdersService.Models;

namespace OrdersService.ServiceTests.Features
{
    public partial class Managing_orders
    {
        [Scenario]
        public async Task Creating_order()
        {
            await Runner.RunScenarioAsync(
                _ => Given_a_valid_account(),
                _ => When_create_order_endpoint_is_called_for_products("product-A"),
                _ => Then_response_should_have_status(HttpStatusCode.Created),
                _ => Then_response_should_contain_order(),
                _ => Then_OrderCreatedEvent_should_be_published(),
                _ => Then_get_order_endpoint_should_return_order_with_status(OrderStatus.Created));
        }

        [Scenario]
        public async Task Creating_order_for_invalid_account()
        {
            await Runner.RunScenarioAsync(
                _ => Given_an_invalid_account(),
                _ => When_create_order_endpoint_is_called_for_products("product-A"),
                _ => Then_response_should_have_status(HttpStatusCode.BadRequest));
        }

        [Scenario]
        public async Task Rejecting_order()
        {
            await Runner.RunScenarioAsync(
                _ => Given_a_created_order(),
                _ => When_RejectOrderCommand_is_sent_for_this_order(),
                _ => Then_OrderStatusUpdatedEvent_should_be_published_with_status(OrderStatus.Rejected),
                _ => Then_get_order_endpoint_should_return_order_with_status(OrderStatus.Rejected)
            );
        }

        [Scenario]
        public async Task Approving_order()
        {
            await Runner.RunScenarioAsync(
                _ => Given_a_created_order(),
                _ => When_ApproveOrderCommand_is_sent_for_this_order(),
                _ => Then_OrderStatusUpdatedEvent_should_be_published_with_status(OrderStatus.Complete),
                _ => Then_get_order_endpoint_should_return_order_with_status(OrderStatus.Complete)
            );
        }

        [Scenario]
        public async Task Dispatching_products_for_approved_order()
        {
            await Runner.RunScenarioAsync(
                _ => Given_a_created_order(),
                _ => When_ApproveOrderCommand_is_sent_for_this_order(),
                _ => Then_OrderStatusUpdatedEvent_should_be_published_with_status(OrderStatus.Complete),
                _ => Then_OrderProductDispatchEvent_should_be_published_for_each_product()
            );
        }
    }
}