using System;
using System.Collections.Generic;

namespace CustomerManagement
{
    public class CustomerService : ICustomerService
    {
        private readonly Dictionary<Guid, Customer> _customers = new Dictionary<Guid, Customer>();

        public Guid CreateCustomer(CustomerCreateRequest request)
        {
            var customer = new Customer(Guid.NewGuid(), request.FirstName, request.LastName, request.Address);
            _customers.Add(customer.Id, customer);
            return customer.Id;
        }

        public Customer GetById(Guid customerId)
        {
            return _customers.TryGetValue(customerId, out var customer)
                ? customer
                : null;
        }
    }
}