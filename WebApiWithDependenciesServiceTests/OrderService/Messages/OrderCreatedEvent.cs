using System;

namespace OrderService.Messages
{
    public record OrderCreatedEvent
    {
        public Guid OrderId { get; init; }
    }
}