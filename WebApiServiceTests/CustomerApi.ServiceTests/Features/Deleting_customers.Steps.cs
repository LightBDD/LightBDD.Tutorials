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
    public partial class Deleting_customers : FeatureFixture
    {
        private readonly HttpClient _client;
        private State<Guid> _customerId;
        private State<HttpResponseMessage> _response;

        public Deleting_customers()
        {
            _client = TestServer.GetClient();
        }

        private async Task Given_an_existing_customer_Id()
        {
            var request = new CreateCustomerRequest
            {
                Email = $"{Guid.NewGuid()}@mail.com",
                FirstName = "Joe",
                LastName = "Kennedy"
            };
            var response = await _client.CreateCustomer(request);
            var customer = await response.EnsureSuccessStatusCode().DeserializeAsync<Customer>();
            _customerId = customer.Id;
        }

        private async Task Given_an_Id_of_nonexistent_customer()
        {
            _customerId = Guid.NewGuid();
        }

        private async Task When_I_request_to_delete_customer_by_this_Id()
        {
            _response = await _client.DeleteCustomerById(_customerId.GetValue());
        }

        private async Task When_I_request_the_customer_by_this_Id()
        {
            _response = await _client.GetCustomerById(_customerId);
        }

        private async Task Then_the_response_should_have_status_code(HttpStatusCode code)
        {
            Assert.Equal(code, _response.GetValue().StatusCode);
        }
    }
}