using CustomerApi.Models;
using LightBDD.Framework;
using LightBDD.XUnit2;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace CustomerApi.ServiceTests.Features
{
    public partial class Retrieving_customers : FeatureFixture
    {
        private readonly HttpClient _client;
        private State<HttpResponseMessage> _customerCreationResponse;
        private State<HttpResponseMessage> _response;
        private State<CreateCustomerRequest> _customerCreationRequest;
        private State<Guid> _customerId;

        public Retrieving_customers()
        {
            _client = TestServer.GetClient();
        }

        private async Task Given_a_successful_customer_creation_response()
        {
            _customerCreationRequest = new CreateCustomerRequest
            {
                Email = $"{Guid.NewGuid()}@mail.com",
                FirstName = "Joe",
                LastName = "Smith"
            };
            _customerCreationResponse = await _client.CreateCustomer(_customerCreationRequest);
            _customerCreationResponse.GetValue().EnsureSuccessStatusCode();
        }

        private async Task Given_an_Id_of_the_created_customer()
        {
            var customer = await _customerCreationResponse.GetValue().DeserializeAsync<Customer>();
            _customerId = customer.Id;
        }

        private async Task Given_an_Id_of_nonexistent_customer()
        {
            _customerId = Guid.NewGuid();
        }

        private async Task When_I_request_the_customer_by_this_Id()
        {
            _response = await _client.GetCustomerById(_customerId);
        }

        private async Task When_I_follow_the_response_location_header()
        {
            _response = await _client.GetAsync(_customerCreationResponse.GetValue().Headers.Location);
        }

        private async Task Then_the_response_should_have_status_code(HttpStatusCode code)
        {
            Assert.Equal(code, _response.GetValue().StatusCode);
        }

        private async Task Then_the_response_should_contain_existing_customer_details()
        {
            var actual = await _response.GetValue().DeserializeAsync<Customer>();
            var expected = _customerCreationRequest.GetValue();
            Assert.Equal(expected.Email, actual.Email);
            Assert.Equal(expected.FirstName, actual.FirstName);
            Assert.Equal(expected.LastName, actual.LastName);
        }
    }
}