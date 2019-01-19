using CustomerApi.Models;
using CustomerApi.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CustomerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomersController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Customer))]
        [ProducesResponseType(404)]
        public ActionResult<Customer> GetCustomer(Guid id)
        {
            var customer = _customerRepository.FindCustomerById(id);
            if (customer == null)
                return NotFound();
            return customer;
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Customer))]
        public ActionResult<Customer> CreateCustomer([FromBody] CreateCustomerRequest request)
        {
            var customer = _customerRepository.CreateCustomer(request.ToCustomer());
            return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult DeleteCustomer(Guid id)
        {
            if (!_customerRepository.DeleteCustomer(id))
                return NotFound();
            return Ok();
        }
    }
}