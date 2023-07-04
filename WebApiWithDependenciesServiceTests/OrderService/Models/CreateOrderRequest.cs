using System;
using System.ComponentModel.DataAnnotations;

namespace OrderService.Models
{
    public record CreateOrderRequest
    {
        [Required]
        public Guid AccountId { get; init; }

        [Required] public string[] Products { get; init; } = Array.Empty<string>();
    }
}