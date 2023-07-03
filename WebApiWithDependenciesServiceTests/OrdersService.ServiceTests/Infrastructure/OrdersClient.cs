using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using OrdersService.Models;

namespace OrdersService.ServiceTests.Infrastructure;

internal class OrdersClient
{
    private readonly HttpClient _client;

    public OrdersClient(TestServer server)
    {
        _client = server.Client;
    }

    public Task<HttpResponseMessage> CreateOrder(CreateOrderRequest request) => _client.PostAsJsonAsync("/orders", request);

    public Task<Order> GetOrder(Guid id) => _client.GetFromJsonAsync<Order>($"/orders/{id}");
}