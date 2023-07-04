using System;

namespace OrderService.Messages
{
    public record RejectOrderCommand
    {
        public Guid OrderId { get; init; }
    }
}