using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace OrdersService.Clients
{
    public class AccountServiceClient
    {
        private readonly HttpClient _client;

        public AccountServiceClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<bool> IsValidAccount(Guid accountId) => await _client.GetFromJsonAsync<bool>($"/accounts/{accountId}/validate");
    }
}