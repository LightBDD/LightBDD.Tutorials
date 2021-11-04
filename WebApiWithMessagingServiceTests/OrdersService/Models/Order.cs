using System;
using OrdersService.Controllers;

namespace OrdersService.Models
{
    public record Order
    {
        public Guid Id { get; init; }
        public Guid AccountId { get; init; }
        public string[] Products { get; init; }
        public OrderStatus Status { get; init; }
    }
}