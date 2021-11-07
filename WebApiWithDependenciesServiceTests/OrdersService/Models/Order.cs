using System;

namespace OrdersService.Models
{
    public record Order
    {
        public Guid Id { get; init; }
        public Guid AccountId { get; init; }
        public string[] Products { get; init; } = Array.Empty<string>();
        public OrderStatus Status { get; init; }
    }
}