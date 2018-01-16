## Summary

This tutorial shows how to convert regular unit tests to LightBDD scenarios.
It covers basics how to install and enable LightBDD, write steps and share state between them as well as execution progress and reporting.
It bases on xunit test framework, but it is applicable to any framework LightBDD integrates with.

## Converting unit tests to LightBDD

In this tutorial I would like to present how to convert regular tests into LightBDD scenarios.

For this purpose I have created a sample %%link{CustomerManagement}%% project for managing customers. It exposes interface %%interface{ICustomerService}%% that allows to create customers and retrieve them by ID and %%link{CustomerService|CustomerManagement/CustomerService.cs}%% class implementing it.

The %%link{CustomerManagement.Tests}%% project contains tests for this class written with xunit test framework that looks as follows: %%class{CustomerServiceTests}%%

### 1. Extract test steps

Lets take a closer look at first test here: %%method{CustomerServiceTests.CreatingCustomer}%%
The test is composed from *arrange*, *act* and *assert* blocks of code. Let's try to extract them to separate *Given*/*When*/*Then* methods and give them descriptive names.

The first step will look like this: %%method{CustomerServiceTests_with_extracted_steps.Given_the_customer_service}%%
It's name indicates now that we will be given a customer service that we can use later in scenario.
The service instance is shared now with other steps via class field.

*Note: Using test class fields in xunit and mstest is perfectly fine, however you have to remember that nunit shares test class instance between tests, so it is worth to clear their values explicitly in SetUp or TearDown methods*

Finally, to make it easier to read, I used the snake case for the method name.

Next, I'll extract step indicating that we are dealing with new customer: %%method{CustomerServiceTests_with_extracted_steps.Given_a_new_customer}%%

Similarly to previous step, this one is also a *Given* step and uses field to store our request instance.

Now it is the time to extract the *When* step: %%method{CustomerServiceTests_with_extracted_steps.When_I_create_that_customer_in_the_customer_service}%%
... and store the method result.

The last step will look like that: %%method{CustomerServiceTests_with_extracted_steps.Then_customer_service_should_return_customer_ID}%%
As this step contains assertions, it starts with *Then* prefix where name describes what *should* happen.

After refactoring, our test method looks now like that: %%method{CustomerServiceTests_with_extracted_steps.CreatingCustomer}%%

We can now re-run the tests and see that it still works as expected, however the test itself is written in behavioral style.

If we follow the same approach to refactor other tests, we will end up with code looking more or less like this: %%class{CustomerServiceTests_with_extracted_steps}%% **find better name!!**

### 2. Install LightBDD
