using System;

namespace CustomerManagement
{
    public interface ICustomerService
    {
        Guid CreateCustomer(CustomerCreateRequest request);
        Customer GetById(Guid customerId);
    }
}