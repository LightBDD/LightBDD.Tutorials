using CustomerApi.Models;
using LightBDD.Framework;
using LightBDD.Framework.Parameters;
using LightBDD.Framework.Scenarios;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace CustomerApi.ServiceTests.Features
{
    public partial class Adding_customers
    {
        private readonly HttpClient _client;
        private State<CreateCustomerRequest> _createCustomerRequest;
        private State<HttpResponseMessage> _response;
        private State<Customer> _createdCustomer;

        public Adding_customers()
        {
            _client = TestServer.GetClient();
        }

        private async Task Given_a_valid_CreateCustomerRequest()
        {
            _createCustomerRequest = new CreateCustomerRequest
            {
                Email = $"{Guid.NewGuid()}@mymail.com",
                FirstName = "John",
                LastName = "Smith"
            };
        }

        private async Task Given_a_CreateCustomerRequest_with_the_same_email_as_existing_customer()
        {
            _createCustomerRequest = new CreateCustomerRequest
            {
                Email = _createCustomerRequest.GetValue().Email,
                FirstName = "Bob",
                LastName = "Johnson"
            };
        }

        private async Task<CompositeStep> Given_an_existing_customer()
        {
            return CompositeStep.DefineNew()
                .AddAsyncSteps(
                    _ => Given_a_valid_CreateCustomerRequest(),
                    _ => When_I_request_customer_creation(),
                    _ => Then_the_response_should_have_status_code(HttpStatusCode.Created))
                .Build();
        }

        private async Task Given_a_CreateCustomerRequest_with_no_details()
        {
            _createCustomerRequest = new CreateCustomerRequest();
        }

        private async Task When_I_request_customer_creation()
        {
            _response = await _client.CreateCustomer(_createCustomerRequest.GetValue());
        }

        private async Task Then_the_created_customer_should_contain_customer_Id()
        {
            Assert.NotEqual(Guid.Empty, _createdCustomer.GetValue().Id);
        }

        private async Task Then_the_created_customer_should_contain_specified_customer_data()
        {
            var request = _createCustomerRequest.GetValue();
            var customer = _createdCustomer.GetValue();

            Assert.Equal(request.Email, customer.Email);
            Assert.Equal(request.FirstName, customer.FirstName);
            Assert.Equal(request.LastName, customer.LastName);
        }

        private async Task Then_the_response_should_have_status_code(HttpStatusCode code)
        {
            Assert.Equal(code, _response.GetValue().StatusCode);
        }

        private async Task Then_the_response_should_have_customer_content()
        {
            _createdCustomer = await _response.GetValue().DeserializeAsync<Customer>();
        }

        private async Task Then_the_response_should_contain_errors(VerifiableDataTable<Error> errors)
        {
            var actual = await _response.GetValue().DeserializeAsync<Errors>();
            errors.SetActual(actual.Items.OrderBy(x => x.Message));
        }

        private async Task Then_the_response_should_have_location_header()
        {
            Assert.NotNull(_response.GetValue().Headers.Location);
        }
    }
}