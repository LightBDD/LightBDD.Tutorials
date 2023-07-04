using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using OrderService.Models;

namespace OrderService.ServiceTests.Infrastructure;

/// <summary>
/// Order Service client allowing to interact with service REST Api
/// </summary>
internal class OrderServiceClient
{
    private readonly HttpClient _client;

    public OrderServiceClient(TestServer server)
    {
        _client = server.Client;
    }

    public Task<HttpResponseMessage> CreateOrder(CreateOrderRequest request) => _client.PostAsJsonAsync("/orders", request);

    public Task<Order> GetOrder(Guid id) => _client.GetFromJsonAsync<Order>($"/orders/{id}");
}