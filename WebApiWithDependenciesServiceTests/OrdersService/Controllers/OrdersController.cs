using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using OrdersService.Clients;
using OrdersService.Messages;
using OrdersService.Models;
using OrdersService.Repositories;
using Rebus.Bus;

namespace OrdersService.Controllers
{
    /// <summary>
    /// Orders controller responsible for order creation and retrieval of order details.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly AccountServiceClient _accountServiceClient;
        private readonly IBus _bus;
        private readonly OrdersRepository _repository;

        public OrdersController(AccountServiceClient accountServiceClient, IBus bus, OrdersRepository repository)
        {
            _accountServiceClient = accountServiceClient;
            _bus = bus;
            _repository = repository;
        }

        /// <summary>
        /// Creates new order and publishes OrderCreatedEvent
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            if (!await _accountServiceClient.IsValidAccount(request.AccountId))
                return BadRequest("Invalid account");

            var order = new Order
            {
                Id = Guid.NewGuid(),
                AccountId = request.AccountId,
                Products = request.Products,
                Status = OrderStatus.Created
            };

            _repository.Upsert(order);

            await _bus.Publish(new OrderCreatedEvent { OrderId = order.Id });

            return CreatedAtAction("GetById", new { orderId = order.Id }, order);
        }

        /// <summary>
        /// Retrieves order details.
        /// </summary>
        [HttpGet("{orderId}")]
        public IActionResult GetById(Guid orderId)
        {
            var order = _repository.GetById(orderId);
            return order != null ? Ok(order) : NotFound();
        }
    }
}
