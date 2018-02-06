## Content

This tutorial shows how to convert regular unit tests to LightBDD scenarios.  
It covers basics how to install and enable LightBDD, write steps and share state between them as well as execution progress and reporting.  
It bases on xunit test framework, but it is applicable to any framework LightBDD integrates with.  

## Converting unit tests to LightBDD

In this tutorial I would like to present how to convert regular tests into LightBDD scenarios.

For this purpose I have created a sample %%link{CustomerManagement}%% project for managing customers.  
It exposes interface that allows to create customers and retrieve them by ID: %%interface{ICustomerService}%%
It also provides sample implementation: %%link{CustomerService|CustomerManagement/CustomerService.cs}%%.

The %%link{CustomerManagement.Tests}%% project contains tests for this class, written with [xunit test framework](https://xunit.github.io/) and it looks as follows: %%class{CustomerServiceTests}%%

### 1. Extract test steps

Let's take a closer look at first test here: %%method{CustomerServiceTests.CreatingCustomer}%%
The test is composed from *arrange*, *act* and *assert* blocks of code. Let's try to extract them to separate *Given*/*When*/*Then* methods and give them descriptive names.

The first step will look like this: %%method{RefactoredCustomerServiceTests.Given_the_customer_service}%%
It's name indicates now that we will be given a customer service that we can use later in scenario.
The service instance is shared now with other steps via class field.

*Note: Using test class fields in *xunit* and *mstest* is perfectly fine, however you have to remember that *nunit* shares test class instance between tests, so it is worth to clear their values explicitly in `SetUp` or `TearDown` methods*

Finally, to make it easier to read, I used the snake case for the method name.

Next, I'll extract step indicating that we are dealing with new customer: %%method{RefactoredCustomerServiceTests.Given_a_new_customer}%%

Similarly to previous step, this one is also a *Given* step and uses field to store our request instance.

Now it is the time to extract the *When* step: %%method{RefactoredCustomerServiceTests.When_I_create_that_customer_in_the_customer_service}%%
...and store the method result.

The last step will look like this: %%method{RefactoredCustomerServiceTests.Then_customer_service_should_return_customer_ID}%%
As this step contains assertions, it starts with *Then* prefix where name describes what *should* happen.

After refactoring, our test method looks now as follows: %%method{RefactoredCustomerServiceTests.CreatingCustomer}%%

We can now re-run the tests and see that it still works like before, however the test itself is written in behavioral style.

If we follow the same approach to refactor other tests, we will end up with code looking more or less like this: %%class{RefactoredCustomerServiceTests}%%

### 2. Install LightBDD

Now, it is time to install LightBDD. As tests are written with *xunit*, I will install **LightBDD.XUnit2** package. In new csproj format, it is a matter of adding following line: %%text{|xml|CustomerManagement.Tests.csproj|<PackageReference Include="LightBDD.XUnit2" Version="[^"]+" />}%%
In old csprojs, it will be easier to use *Nuget Package Manager* UI to install **LightBDD.XUnit2**.

### 3. Convert tests to Scenarios

As tests are refactored now to the behavioral style, converting them to scenarios is simple and consists of following steps:
1. Making test class inheriting `FeatureFixture` base class,
2. Changing `[Fact]` or `[Theory]` attributes to `[Scenario]` attribute,
3. Wrapping all steps in one of `Runner.RunScenario*` method. For simplicity, I'll use basic scenario syntax, i.e. `Runner.RunScenario()` from `LightBDD.Framework.Scenarios.Basic` namespace.

Let's take a look on the sample test before and after conversion...

Before it looked like this: %%method{CustomerServiceTests.CreatingCustomer}%%
After change, it looks like this: %%method{Customer_management_feature.Creating_new_customer}%%

The whole test class looks like this: %%class{Customer_management_feature}%%

In fact I have made a few more alterations here:
* I renamed class and scenarios to use snake case and changed the name to describe better the feature that we are testing here,
* I moved all the step methods and fields to a %%link{CustomerManagement.Tests\Customer_management_feature.Steps.cs}%% file in order to make scenarios easy to read.

### 4. Run tests

Before running the tests, we have to do one more small thing - add LightBDD scope (that is necessary to enable LightBDD integration): %%text{|c#|Customer_management_feature.cs|\[assembly: LightBddScope\]}%%
If you forget to add it and try running tests, they will fail with a message like this:  
`LightBddScopeAttribute is not defined in the project. Please ensure that following attribute, or attribute extending it is defined at assembly level: [assembly:LightBddScopeAttribute]`.

Now we are ready to run the tests in exactly same way as before - we can use Visual Studio Test Explorer, Resharper or `dotnet xunit` command.

After test run we can notice few additional differences:

First of all, we will see all the steps printed in the test Output window or on console, including the step status and execution time.

When tests are run with `dotnet xunit` command, we can observe steps execution progress in real time - the sample output of this command looks like this:
```
%%content{test.log}%%
```
*Note: This tutorial bases on xunit, however other integrations also offers such functionality - see [LightBDD Test Framework Integrations](https://github.com/LightBDD/LightBDD/wiki/Test-Framework-Integrations) wiki page for more details.*

Other difference is that report files are created in directory where tests were executed (for this tutorial it will be in `CustomerManagement.Tests\bin\Debug\net46\Reports` directory).  
The sample `FeaturesReport.html` looks like this: %%directHtmlLink{CustomerManagement.Tests\bin\Debug\net46\Reports\FeaturesReport.html}%%

### Summary

As LightBDD integrates on top of the test frameworks, converting standard tests to LightBDD scenarios is rather a simple process - it is a matter of refactoring the tests to *given-when-then* step methods and wrapping them with LightBDD runner call.

As soon as the tests are converted, their execution progress can be tracked on the console and they will be included in the html report file.