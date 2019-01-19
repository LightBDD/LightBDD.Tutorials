using System.Net;
using System.Threading.Tasks;
using CustomerApi.ErrorHandling;
using CustomerApi.Models;
using LightBDD.Framework;
using LightBDD.Framework.Parameters;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;

namespace CustomerApi.ServiceTests.Features
{
    [FeatureDescription(
@"In order to manage customers database
As an Api client
I want to be able to add new customers")]
    public partial class Adding_customers : FeatureFixture
    {
        [Scenario]
        public async Task Creating_a_new_customer()
        {
            await Runner.RunScenarioAsync(
                _ => Given_a_valid_CreateCustomerRequest(),
                _ => When_I_request_customer_creation(),
                _ => Then_the_response_should_have_status_code(HttpStatusCode.Created),
                _ => Then_the_response_should_have_customer_content(),
                _ => Then_the_response_should_have_location_header(),
                _ => Then_the_created_customer_should_contain_specified_customer_data(),
                _ => Then_the_created_customer_should_contain_customer_Id());
        }

        [Scenario]
        public async Task Creating_customer_with_missing_details_is_not_allowed()
        {
            await Runner.RunScenarioAsync(
                _ => Given_a_CreateCustomerRequest_with_no_details(),
                _ => When_I_request_customer_creation(),
                _ => Then_the_response_should_have_status_code(HttpStatusCode.BadRequest),
                _ => Then_the_response_should_contain_errors(Table.ExpectData(
                    new Error(ErrorCodes.ValidationError, "The Email field is required."),
                    new Error(ErrorCodes.ValidationError, "The FirstName field is required."),
                    new Error(ErrorCodes.ValidationError, "The LastName field is required."))));
        }

        [Scenario]
        public async Task Creating_customer_with_already_used_email_is_not_allowed()
        {
            await Runner.RunScenarioAsync(
                _ => Given_an_existing_customer(),
                _ => Given_a_CreateCustomerRequest_with_the_same_email_as_existing_customer(),
                _ => When_I_request_customer_creation(),
                _ => Then_the_response_should_have_status_code(HttpStatusCode.BadRequest),
                _ => Then_the_response_should_contain_errors(Table.ExpectData(new Error(ErrorCodes.EmailInUse, "Email already in use."))));
        }
    }
}