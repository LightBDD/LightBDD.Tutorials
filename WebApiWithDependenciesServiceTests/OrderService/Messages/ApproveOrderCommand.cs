using System;

namespace OrderService.Messages
{
    public record ApproveOrderCommand
    {
        public Guid OrderId { get; init; }
    }
}