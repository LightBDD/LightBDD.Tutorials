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
I want to be able to retrieve existing customers")]
    public partial class Retrieving_customers
    {
        [Scenario]
        public async Task Retrieving_customer_via_location_header()
        {
            await Runner.RunScenarioAsync(
                _ => Given_a_successful_customer_creation_response(),
                _ => When_I_follow_the_response_location_header(),
                _ => Then_the_response_should_have_status_code(HttpStatusCode.OK),
                _ => Then_the_response_should_contain_existing_customer_details());
        }

        [Scenario]
        public async Task Retrieving_customer_by_Id()
        {
            await Runner.RunScenarioAsync(
                _ => Given_a_successful_customer_creation_response(),
                _ => Given_an_Id_of_the_created_customer(),
                _ => When_I_request_the_customer_by_this_Id(),
                _ => Then_the_response_should_have_status_code(HttpStatusCode.OK),
                _ => Then_the_response_should_contain_existing_customer_details());
        }

        [Scenario]
        public async Task Retrieving_nonexistent_customer()
        {
            await Runner.RunScenarioAsync(
                _ => Given_an_Id_of_nonexistent_customer(),
                _ => When_I_request_the_customer_by_this_Id(),
                _ => Then_the_response_should_have_status_code(HttpStatusCode.NotFound));
        }
    }
}