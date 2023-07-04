using System;
using OrderService.Models;

namespace OrderService.Messages
{
    public record OrderStatusUpdatedEvent
    {
        public Guid OrderId { get; init; }
        public OrderStatus Status { get; init; }
    }
}