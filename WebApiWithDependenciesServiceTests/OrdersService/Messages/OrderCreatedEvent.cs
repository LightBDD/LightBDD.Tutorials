using System;

namespace OrdersService.Messages
{
    public record OrderCreatedEvent
    {
        public Guid OrderId { get; init; }
    }
}