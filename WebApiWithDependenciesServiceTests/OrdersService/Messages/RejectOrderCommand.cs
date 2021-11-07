using System;

namespace OrdersService.Messages
{
    public record RejectOrderCommand
    {
        public Guid OrderId { get; init; }
    }
}