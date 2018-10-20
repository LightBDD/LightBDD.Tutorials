using System;
using CustomerApi.ErrorHandling;
using CustomerApi.Models;
using LiteDB;

namespace CustomerApi.Repositories
{
    internal class CustomerRepository : ICustomerRepository
    {
        private readonly LiteCollection<Customer> _collection;

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

    internal class EmailAlreadyInUseException : ApiException
    {
        public EmailAlreadyInUseException() : base(ErrorCodes.EmailInUse,"Email already in use.")
        {
        }
    }

    internal class ApiException:InvalidOperationException
    {
        public string Code { get; }

        public ApiException(string code, string message):base(message)
        {
            Code = code;
        }
    }
}