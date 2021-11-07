using System;
using OrdersService.Models;

namespace OrdersService.Messages
{
    public record OrderStatusUpdatedEvent
    {
        public Guid OrderId { get; init; }
        public OrderStatus Status { get; init; }
    }
}