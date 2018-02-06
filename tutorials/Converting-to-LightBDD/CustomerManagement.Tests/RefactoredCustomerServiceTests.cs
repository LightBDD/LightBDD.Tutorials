using System;
using Xunit;

namespace CustomerManagement.Tests
{
    public class RefactoredCustomerServiceTests
    {
        private ICustomerService _service;
        private CustomerCreateRequest _createRequest;
        private Guid _customerId;
        private Customer _retrievedCustomer;

        [Fact]
        public void CreatingCustomer()
        {
            //arrange
            Given_the_customer_service();
            Given_a_new_customer();

            //act
            When_I_create_that_customer_in_the_customer_service();

            //assert
            Then_customer_service_should_return_customer_ID();
        }

        [Fact]
        public void RetrievingCustomer()
        {
            //arrange
            Given_the_customer_service();
            Given_an_existing_customer_with_known_ID();

            //act
            When_I_request_customer_by_that_customer_ID();

            //assert
            Then_customer_service_should_return_customer();
            Then_the_returned_customer_should_have_expected_details();
        }

        [Fact]
        public void RetrievingNonExistentCustomer()
        {
            //arrange
            Given_the_customer_service();
            Given_an_customer_ID_of_nonexistent_customer();

            //act
            When_I_request_customer_by_that_customer_ID();

            //assert
            Then_customer_service_should_return_null_customer();
        }

        private void Given_the_customer_service()
        {
            _service = new CustomerService();
        }

        private void Given_a_new_customer()
        {
            _createRequest = new CustomerCreateRequest
            {
                FirstName = "Bob",
                LastName = "Johnson",
                Address = "High St. 1, AB1 2CD, London"
            };
        }

        private void Given_an_customer_ID_of_nonexistent_customer()
        {
            _customerId = Guid.NewGuid();
        }

        private void Given_an_existing_customer_with_known_ID()
        {
            Given_a_new_customer();
            When_I_create_that_customer_in_the_customer_service();
        }

        private void When_I_create_that_customer_in_the_customer_service()
        {
            _customerId = _service.CreateCustomer(_createRequest);
        }

        private void When_I_request_customer_by_that_customer_ID()
        {
            _retrievedCustomer = _service.GetById(_customerId);
        }

        private void Then_customer_service_should_return_customer_ID()
        {
            Assert.NotEqual(Guid.Empty, _customerId);
        }

        private void Then_customer_service_should_return_customer()
        {
            Assert.NotNull(_retrievedCustomer);
        }

        private void Then_customer_service_should_return_null_customer()
        {
            Assert.Null(_retrievedCustomer);
        }

        private void Then_the_returned_customer_should_have_expected_details()
        {
            Assert.Equal(_customerId, _retrievedCustomer.Id);
            Assert.Equal(_createRequest.FirstName, _retrievedCustomer.FirstName);
            Assert.Equal(_createRequest.LastName, _retrievedCustomer.LastName);
            Assert.Equal(_createRequest.Address, _retrievedCustomer.Address);
        }
    }
}
