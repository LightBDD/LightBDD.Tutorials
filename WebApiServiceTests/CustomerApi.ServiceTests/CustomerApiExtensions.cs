using CustomerApi.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace CustomerApi.ServiceTests
{
    internal static class CustomerApiExtensions
    {
        public static Task<HttpResponseMessage> CreateCustomer(this HttpClient client, CreateCustomerRequest createCustomerRequest)
        {
            return client.PostAsync("api/customers", createCustomerRequest.ToJsonContent());
        }

        public static Task<HttpResponseMessage> GetCustomerById(this HttpClient client, Guid id)
        {
            return client.GetAsync($"api/customers/{id}");
        }

        public static Task<HttpResponseMessage> DeleteCustomerById(this HttpClient client, Guid id)
        {
            return client.DeleteAsync($"api/customers/{id}");
        }
    }
}