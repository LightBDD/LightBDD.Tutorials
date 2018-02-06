using LightBDD.Framework.Scenarios.Basic;
using LightBDD.XUnit2;

[assembly: LightBddScope]

namespace CustomerManagement.Tests
{
    public partial class Customer_management_feature : FeatureFixture
    {
        [Scenario]
        public void Creating_new_customer()
        {
            Runner.RunScenario(

                Given_the_customer_service,
                Given_a_new_customer,
                When_I_create_that_customer_in_the_customer_service,
                Then_customer_service_should_return_customer_ID);
        }

        [Scenario]
        public void Retrieving_existing_customer()
        {
            Runner.RunScenario(

                Given_the_customer_service,
                Given_an_existing_customer_with_known_ID,
                When_I_request_customer_by_that_customer_ID,
                Then_customer_service_should_return_customer,
                Then_the_returned_customer_should_have_expected_details);
        }

        [Scenario]
        public void Retrieving_non_existent_customer()
        {
            Runner.RunScenario(

                Given_the_customer_service,
                Given_a_customer_ID_of_nonexistent_customer,
                When_I_request_customer_by_that_customer_ID,
                Then_customer_service_should_return_null_customer);
        }
    }
}
