using System;

namespace OrdersService.Messages
{
    public record ApproveOrderCommand
    {
        public Guid OrderId { get; init; }
    }
}