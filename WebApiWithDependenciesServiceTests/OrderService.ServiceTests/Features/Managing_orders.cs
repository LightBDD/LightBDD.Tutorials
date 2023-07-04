using System.Net;
using System.Threading.Tasks;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;
using OrderService.Models;
using OrderService.ServiceTests.Contexts;

namespace OrderService.ServiceTests.Features
{
    public class Managing_orders : FeatureFixture
    {
        [Scenario]
        public async Task Creating_order()
        {
            await Runner
                .WithContext<OrderContext>()
                .RunScenarioAsync(
                    x => x.Given_a_valid_account(),
                    x => x.When_create_order_endpoint_is_called_for_products("product-A"),
                    x => x.Then_response_should_have_status(HttpStatusCode.Created),
                    x => x.Then_response_should_contain_order(),
                    x => x.Then_OrderCreatedEvent_should_be_published(),
                    x => x.Then_get_order_endpoint_should_return_order_with_status(OrderStatus.Created));
        }

        [Scenario]
        public async Task Creating_order_for_invalid_account()
        {
            await Runner
                .WithContext<OrderContext>()
                .RunScenarioAsync(
                    x => x.Given_an_invalid_account(),
                    x => x.When_create_order_endpoint_is_called_for_products("product-A"),
                    x => x.Then_response_should_have_status(HttpStatusCode.BadRequest));
        }

        [Scenario]
        public async Task Rejecting_order()
        {
            await Runner
                .WithContext<OrderContext>()
                .RunScenarioAsync(
                    x => x.Given_a_created_order(),
                    x => x.When_RejectOrderCommand_is_sent_for_this_order(),
                    x => x.Then_OrderStatusUpdatedEvent_should_be_published_with_status(OrderStatus.Rejected),
                    x => x.Then_get_order_endpoint_should_return_order_with_status(OrderStatus.Rejected));
        }

        [Scenario]
        public async Task Approving_order()
        {
            await Runner
                .WithContext<OrderContext>()
                .RunScenarioAsync(
                    x => x.Given_a_created_order(),
                    x => x.When_ApproveOrderCommand_is_sent_for_this_order(),
                    x => x.Then_OrderStatusUpdatedEvent_should_be_published_with_status(OrderStatus.Complete),
                    x => x.Then_get_order_endpoint_should_return_order_with_status(OrderStatus.Complete));
        }

        [Scenario]
        public async Task Dispatching_products_for_approved_order()
        {
            await Runner
                .WithContext<OrderContext>()
                .RunScenarioAsync(
                    x => x.Given_a_created_order(),
                    x => x.When_ApproveOrderCommand_is_sent_for_this_order(),
                    x => x.Then_OrderStatusUpdatedEvent_should_be_published_with_status(OrderStatus.Complete),
                    x => x.Then_OrderProductDispatchEvent_should_be_published_for_each_product());
        }
    }
}