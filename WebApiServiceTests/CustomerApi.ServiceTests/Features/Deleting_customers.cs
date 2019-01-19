using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;
using System.Net;
using System.Threading.Tasks;

namespace CustomerApi.ServiceTests.Features
{
    [FeatureDescription(
@"In order to manage customers database
As an Api client
I want to be able to delete existing customers")]
    public partial class Deleting_customers
    {
        [Scenario]
        public async Task Deleting_customer()
        {
            await Runner.RunScenarioAsync(
                _ => Given_an_existing_customer_Id(),
                _ => When_I_request_to_delete_customer_by_this_Id(),
                _ => Then_the_response_should_have_status_code(HttpStatusCode.OK),
                _ => When_I_request_the_customer_by_this_Id(),
                _ => Then_the_response_should_have_status_code(HttpStatusCode.NotFound));
        }

        [Scenario]
        public async Task Deleting_nonexistent_customer()
        {
            await Runner.RunScenarioAsync(
                _ => Given_an_Id_of_nonexistent_customer(),
                _ => When_I_request_to_delete_customer_by_this_Id(),
                _ => Then_the_response_should_have_status_code(HttpStatusCode.NotFound));
        }
    }
}