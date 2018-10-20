using System;
using CustomerApi.Models;

namespace CustomerApi.Repositories
{
    public interface ICustomerRepository
    {
        Customer CreateCustomer(Customer customer);
        Customer FindCustomerById(Guid id);
        bool DeleteCustomer(Guid id);
    }
}