using CustomerApi.ErrorHandling;
using CustomerApi.Models;
using LiteDB;
using System;

namespace CustomerApi.Repositories
{
    internal class CustomerRepository : ICustomerRepository
    {
        private readonly ILiteCollection<Customer> _collection;

        public CustomerRepository(LiteDatabase db)
        {
            _collection = db.GetCollection<Customer>();
            _collection.EnsureIndex(x => x.Email, true);
        }

        public Customer CreateCustomer(Customer customer)
        {
            try
            {
                customer.Id = _collection.Insert(customer).AsGuid;
            }
            catch (LiteException ex) when (ex.ErrorCode == LiteException.INDEX_DUPLICATE_KEY)
            {
                throw new EmailAlreadyInUseException();
            }

            return customer;
        }

        public Customer FindCustomerById(Guid id)
        {
            return _collection.FindById(id);
        }

        public bool DeleteCustomer(Guid id)
        {
            return _collection.Delete(id);
        }
    }
}