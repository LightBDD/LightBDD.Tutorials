using System;
using Xunit;

namespace CustomerManagement.Tests
{
    public class CustomerServiceTests
    {
        [Fact]
        public void CreatingCustomer()
        {
            //arrange
            var service = new CustomerService();
            var createRequest = new CustomerCreateRequest
            {
                FirstName = "Bob",
                LastName = "Johnson",
                Address = "High St. 1, AB1 2CD, London"
            };

            //act
            var customerId = service.CreateCustomer(createRequest);

            //assert
            Assert.NotEqual(Guid.Empty, customerId);
        }

        [Fact]
        public void RetrievingCustomer()
        {
            //arrange
            var service = new CustomerService();
            var createRequest = new CustomerCreateRequest
            {
                FirstName = "Bob",
                LastName = "Johnson",
                Address = "High St. 1, AB1 2CD, London"
            };
            var customerId = service.CreateCustomer(createRequest);

            //act
            var retrievedCustomer = service.GetById(customerId);

            //assert
            Assert.NotNull(retrievedCustomer);
            Assert.Equal(customerId, retrievedCustomer.Id);
            Assert.Equal(createRequest.FirstName, retrievedCustomer.FirstName);
            Assert.Equal(createRequest.LastName, retrievedCustomer.LastName);
            Assert.Equal(createRequest.Address, retrievedCustomer.Address);
        }

        [Fact]
        public void RetrievingNonExistentCustomer()
        {
            //arrange
            var service = new CustomerService();
            var customerId = Guid.NewGuid();

            //act
            var retrievedCustomer = service.GetById(customerId);

            //assert
            Assert.Null(retrievedCustomer);
        }
    }
}
