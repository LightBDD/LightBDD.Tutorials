# Web Api Service Tests

**Technologies:** AspNetCore, Swashbuckle, LiteDB  
**LightBDD concepts:** partial classes, parallel execution, async steps, [State\<T>](https://github.com/LightBDD/LightBDD/wiki/Scenario-State-Management#ensuring-state-is-initialized-before-use), [composite steps](https://github.com/LightBDD/LightBDD/wiki/Composite-Steps-Definition), [tabular parameters](https://github.com/LightBDD/LightBDD/wiki/Advanced-Step-Parameters#verifiabledatatable)

This is a sample solution showing how to use LightBDD to service test Web Api project.

It consist of two projects:
* [CustomerApi](https://github.com/LightBDD/LightBDD.Tutorials/tree/master/WebApiServiceTests/CustomerApi) - AspNetCore WebApi project,
* [CustomerApi.ServiceTests](https://github.com/LightBDD/LightBDD.Tutorials/tree/master/WebApiServiceTests/CustomerApi.ServiceTests) - Service tests project based on xunit and LightBDD.

## Quick run

1. Open `cmd.exe` in solution directory
2. Run [run-tests.cmd](https://github.com/LightBDD/LightBDD.Tutorials/blob/master/WebApiServiceTests/run-tests.cmd).

The command will run the tests and open the `FeaturesReport.html` produced in `CustomerApi.ServiceTests\bin\Debug\netcoreapp2.1\Reports\` directory.

## CustomerApi

The CustomerApi is a sample AspNetCore WebApi project, generated with `dotnet new webapi` command and extended for the tutorial purpose with a sample stateful `/api/customers` endpoint.

The `/api/customers` endpoint is implemented in [CustomersController](https://github.com/LightBDD/LightBDD.Tutorials/blob/master/WebApiServiceTests/CustomerApi/Controllers/CustomersController.cs) and allows following operations:
* `POST /api/customers` - creates a new customer resource,
* `GET /api/customers/{id}` - retrieves customer resource by ID,
* `DELETE /api/customers/{id}` - deletes customer resource by ID.

The customer data is stored in [LiteDB](http://www.litedb.org/) database, managed by the [CustomerRepository](https://github.com/LightBDD/LightBDD.Tutorials/blob/master/WebApiServiceTests/CustomerApi/Repositories/CustomerRepository.cs) class.  
The repository does not allow to create multiple customers with the same email address.

**The Api has implemented error handling**:
* the [CreateCustomerRequest](https://github.com/LightBDD/LightBDD.Tutorials/blob/master/WebApiServiceTests/CustomerApi/Models/CreateCustomerRequest.cs) model is automatically validated by the controller, where the model validation errors are converted to [Errors](https://github.com/LightBDD/LightBDD.Tutorials/blob/master/WebApiServiceTests/CustomerApi/Models/Errors.cs) model by [ErrorHandler.FromInvalidModel()](https://github.com/LightBDD/LightBDD.Tutorials/blob/master/WebApiServiceTests/CustomerApi/ErrorHandling/ErrorHandler.cs#L21) method,
* the exceptions are automatically converted to either `400` or `500` HTTP Status Code and `Errors` model by [HandleExceptionsFilterAttribute](https://github.com/LightBDD/LightBDD.Tutorials/blob/master/WebApiServiceTests/CustomerApi/Filters/HandleExceptionsFilterAttribute.cs) method.
The error handling is configured in [Startup.ConfigureServices()](https://github.com/LightBDD/LightBDD.Tutorials/blob/master/WebApiServiceTests/CustomerApi/Startup.cs#L26).

Finally, the [Swashbuckle Swagger](https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-2.1) has been added to the Api, allowing to play with it by going to `https://localhost:5001/` when application is running.

## CustomerApi.ServiceTests

The CustomerApi.ServiceTests uses LightBDD to run behavioral tests against CustomerApi. All tests treats the Api as black box and uses only the Api endpoints to communicate. The `WebApplicationFactory<Startup>` is used from **Microsoft.AspNetCore.Mvc.Testing** package to spawn in-memory Api, following [Integration tests in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-2.1) Microsoft documentation.

### One Test Server instance

The `WebApplicationFactory<Startup>` is instantiated once for the whole test run. It is managed by [TestServer](https://github.com/LightBDD/LightBDD.Tutorials/blob/master/WebApiServiceTests/CustomerApi.ServiceTests/TestServer.cs) static class, that offers a `GetClient()` method to obtain the `HttpClient` used later by tests.

The instantiation and disposal of the `TestServer` is handled by the [ConfiguredLightBddScope](https://github.com/LightBDD/LightBDD.Tutorials/blob/master/WebApiServiceTests/CustomerApi.ServiceTests/ConfiguredLightBddScope.cs)  `OnSetUp()` and `OnTearDown()` methods, guaranteeing to execute once, before any and after all tests in the assembly.

**Why the one test server instance is important?**  
Well, the service tests treats the service as a black box, which means that when it is initialized, all potentially complex service startup has to be performed (including database initialization, cache population, service warming-up routines and anything else that the service may be doing during startup). Instantiating the `TestServer` per test or even per test class will introduce the unnecessary overhead that will affect the test execution time.

**Why this example does not use `IClassFixture<WebApplicationFactory<RazorPagesProject.Startup>>` pattern described on [Integration tests in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-2.1) documentation?**  
The [Shared Context between Tests](https://xunit.github.io/docs/shared-context.html) xunit documentation states that:
* using `IClassFixture<T>` makes shared instance within the tests belonging to that class, but not between test classes themselves, which means many initializations of the service code,
* using `ICollectionFixture<T>` will make one instance shared between all the tests and tests classes, but at the cost that none of the tests will run in parallel.

## Running all tests in parallel

The xunit allows to run all the test classes in parallel by default (as long as they do not use `ICollectionFixture<T>`).  
This project enables LightBDD specific test method level parallelization as well, with following code: `[assembly: ClassCollectionBehavior(AllowTestParallelization = true)]` (see [ConfiguredLightBddScope.cs](https://github.com/LightBDD/LightBDD.Tutorials/blob/master/WebApiServiceTests/CustomerApi.ServiceTests/ConfiguredLightBddScope.cs#L4)).

It means that all the test methods specified in that project can run in parallel (please remember that by default the number of tests run in parallel reflects number of CPU cores in xunit).

## Test features

The test features are located in [Features](https://github.com/LightBDD/LightBDD.Tutorials/tree/master/WebApiServiceTests/CustomerApi.ServiceTests/Features) directory.

We can find here three features:
* [Adding_customers](https://github.com/LightBDD/LightBDD.Tutorials/blob/master/WebApiServiceTests/CustomerApi.ServiceTests/Features/Adding_customers.cs) feature testing `POST /api/customers` operation,
* [Retrieving_customers](https://github.com/LightBDD/LightBDD.Tutorials/blob/master/WebApiServiceTests/CustomerApi.ServiceTests/Features/Retrieving_customers.cs) feature testing `GET /api/customers/{id}` operation,
* [Deleting_customers](https://github.com/LightBDD/LightBDD.Tutorials/blob/master/WebApiServiceTests/CustomerApi.ServiceTests/Features/Deleting_customers.cs) feature testing `DELETE /api/customers/{id}` operation.

Each feature class consists of two parts:
* part defining the scenarios, like [Adding_customers.cs](https://github.com/LightBDD/LightBDD.Tutorials/blob/master/WebApiServiceTests/CustomerApi.ServiceTests/Features/Adding_customers.cs),
* part defining the step methods and class state, like [Adding_customers.Steps.cs](https://github.com/LightBDD/LightBDD.Tutorials/blob/master/WebApiServiceTests/CustomerApi.ServiceTests/Features/Adding_customers.Steps.cs).

All the scenarios in this example are asynchronous and uses [extended syntax](https://github.com/LightBDD/LightBDD/wiki/Scenario-Steps-Definition#extended-scenarios).

All of the above scenarios are implemented in the similar way:
* they are independent from each other,
* they are written in a way that can be safely run in parallel,
* they use steps defined in the same class (for simplicity),
* they obtain `HttpClient` from the `TestServer` in the constructor like [here](https://github.com/LightBDD/LightBDD.Tutorials/blob/master/WebApiServiceTests/CustomerApi.ServiceTests/Features/Adding_customers.Steps.cs#L22).

## Sample scenario

Let's take a look at [Adding_customers.Creating_a_new_customer()](https://github.com/LightBDD/LightBDD.Tutorials/blob/master/WebApiServiceTests/CustomerApi.ServiceTests/Features/Adding_customers.cs#L19) scenario for example:
```c#
[Scenario]
public async Task Creating_a_new_customer()
{
    await Runner.RunScenarioAsync(
        _ => Given_a_valid_CreateCustomerRequest(),
        _ => When_I_request_customer_creation(),
        _ => Then_the_response_should_have_status_code(HttpStatusCode.Created),
        _ => Then_the_response_should_have_customer_content(),
        _ => Then_the_response_should_have_location_header(),
        _ => Then_the_created_customer_should_contain_specified_customer_data(),
        _ => Then_the_created_customer_should_contain_customer_Id());
}
```

It describes successful creation of the new customer, where:
* `Given_a_valid_CreateCustomerRequest()` step specifies that we are going to use a request with all the necessary information for creating the new customer resource,
* `When_I_request_customer_creation()` step specifies actual `POST /api/customers` operation call on the Api,
* `Then_...()` steps specifies a set of assertions performed on the response from the Api, including HTTP status code check, location headers as well as content of the response.

This scenario is asynchronous using `Runner.RunScenarioAsync(...)` method, which means that all the step methods have to have signature returning `Task` type.

## Sample feature class definition

Let's take a look now at the [Adding_customers.Steps.cs](https://github.com/LightBDD/LightBDD.Tutorials/blob/master/WebApiServiceTests/CustomerApi.ServiceTests/Features/Adding_customers.Steps.cs) part:

```c#
public partial class Adding_customers
{
    private readonly HttpClient _client;
    private State<CreateCustomerRequest> _createCustomerRequest;
    private State<HttpResponseMessage> _response;
    private State<Customer> _createdCustomer;

    public Adding_customers()
    {
        _client = TestServer.GetClient();
    }

    /* ... */
}
```

As mentioned above, the `Adding_customers` class contains constructor where `HttpClient` is obtained.

The class contains also a set of state fields that are used to share the scenario state between the steps. Please note that the fields uses `State<T>` struct that helps to ensure that fields are initialized before use. More about `State<T>` can be read on [Scenario State Management](https://github.com/LightBDD/LightBDD/wiki/Scenario-State-Management#ensuring-state-is-initialized-before-use) LightBDD wiki page.

### Setting up the scenario context with `given` steps

The `Given_a_valid_CreateCustomerRequest()` is defined as follows:
```c#
private async Task Given_a_valid_CreateCustomerRequest()
{
    _createCustomerRequest = new CreateCustomerRequest
    {
        Email = $"{Guid.NewGuid()}@mymail.com",
        FirstName = "John",
        LastName = "Smith"
    };
}
```

It is initializing the `_createCustomerRequest` field with a request allowing to create new customer.  
Please note that even though the method is not really async, it is declared with `async` modifier and returns `Task`. It is because all the steps in the asynchronous scenario have to return `Task` type and `async` modifier allows to avoid `return Task.CompletedTask` statement.

### Performing actions with `when` steps

The `When_I_request_customer_creation()` step is quite simple as well, but contains few interesting bits:
```c#
private async Task When_I_request_customer_creation()
{
    _response = await _client.CreateCustomer(_createCustomerRequest.GetValue());
}
```

The `_createCustomerRequest.GetValue()` obtains the value of the `State<CreateCustomerRequest> _createCustomerRequest` field, that was set in the `Given_a_valid_CreateCustomerRequest()` step. If the field value was not initialized before, the `GetValue()` will throw with message helping to identify this issue.

The `_client.CreateCustomer()` is an extension method defined in [CustomerApiExtensions](https://github.com/LightBDD/LightBDD.Tutorials/blob/master/WebApiServiceTests/CustomerApi.ServiceTests/CustomerApiExtensions.cs#L10) class. The `HttpClient` extensions pattern allows to extract the actual HTTP calls from the steps and reuse them in other steps and scenarios.

Finally, the `HttpResponseMessage` is obtained asynchronously and captured in the `_response` field.

### Verifying the scenario outcome with `then` steps

The `then` steps are very similar in structure to the previous ones. The difference is that they should focus on verification of the scenario outcome.

Let's take a look at few of those:
```c#
private async Task Then_the_response_should_have_status_code(HttpStatusCode code)
{
    Assert.Equal(code, _response.GetValue().StatusCode);
}

private async Task Then_the_response_should_have_location_header()
{
    Assert.NotNull(_response.GetValue().Headers.Location);
}

private async Task Then_the_response_should_have_customer_content()
{
    _createdCustomer = await _response.GetValue().DeserializeAsync<Customer>();
}

private async Task Then_the_created_customer_should_contain_specified_customer_data()
{
    var request = _createCustomerRequest.GetValue();
    var customer = _createdCustomer.GetValue();

    Assert.Equal(request.Email, customer.Email);
    Assert.Equal(request.FirstName, customer.FirstName);
    Assert.Equal(request.LastName, customer.LastName);
}
```

As presented above, most of them have `Assert.Xxx()` code inside. The `Then_the_response_should_have_customer_content()` step does not have explicit assert, but it deserializes the `HttpResponseMessage` content into model. The `DeserializeAsync<T>()` method is an extension method, defined in [JsonExtensions](https://github.com/LightBDD/LightBDD.Tutorials/blob/master/WebApiServiceTests/CustomerApi.ServiceTests/JsonExtensions.cs#L15).

## Using Composite Steps

The `Creating_a_new_customer()` scenario focuses on the customer creation process. Sometimes however, we need to write scenarios which focus on the behaviors after that stage. Let's take a look at this scenario:

```c#
[Scenario]
public async Task Creating_customer_with_already_used_email_is_not_allowed()
{
    await Runner.RunScenarioAsync(
        _ => Given_an_existing_customer(),
        _ => Given_a_CreateCustomerRequest_with_the_same_email_as_existing_customer(),
        _ => When_I_request_customer_creation(),
        _ => Then_the_response_should_have_status_code(HttpStatusCode.BadRequest),
        _ => Then_the_response_should_contain_errors(Table.ExpectData(
            new Error(ErrorCodes.EmailInUse, "Email already in use."))));
}
```

This scenario bases on the fact that we already have an existing customer. It focuses however on what should happen if we try to create another one with the same email address.

The `Given_an_existing_customer()` step is really an equivalent of following steps from `Creating_a_new_customer()` scenario:

```c#
_ => Given_a_valid_CreateCustomerRequest(),
_ => When_I_request_customer_creation(),
_ => Then_the_response_should_have_status_code(HttpStatusCode.Created),
```

We have few options here what to do:
1. we could just replace `Given_an_existing_customer()` with those three steps, however the scenario itself will become less readable and it will be more difficult to identify where is the focus here,
2. we could implement the `Given_an_existing_customer()` as a normal method with the same code as those three steps have, but it would introduce code duplications and will make it harder to maintain,
3. we could implement the `Given_an_existing_customer()` then to just call those 3 other steps. It will do the trick, but we will lose the visibility of what is exactly happening in the scenario,
4. we could implement the `Given_an_existing_customer()` as a composite step.

In this tutorial we went with last option, a composite step (described on [Composite Steps Definition](https://github.com/LightBDD/LightBDD/wiki/Composite-Steps-Definition) LightBDD wiki page):
```c#
private async Task<CompositeStep> Given_an_existing_customer()
{
    return CompositeStep.DefineNew()
        .AddAsyncSteps(
            _ => Given_a_valid_CreateCustomerRequest(),
            _ => When_I_request_customer_creation(),
            _ => Then_the_response_should_have_status_code(HttpStatusCode.Created))
        .Build();
}
```

This step behavior is very similar to *option 3* where we would just call those three steps from `Given_an_existing_customer()`, however it preserves the information on what is going on in the scenario.

If we run this test in Visual Studio or with [run-tests.cmd](https://github.com/LightBDD/LightBDD.Tutorials/blob/master/WebApiServiceTests/run-tests.cmd), we would see the difference on the console as well as in the `FeaturesReport.html`, where the sample output would be as follows:
```
SCENARIO: Creating customer with already used email is not allowed
  STEP 1/5: GIVEN an existing customer...
  STEP 1.1/1.3: GIVEN a valid CreateCustomerRequest...
  STEP 1.1/1.3: GIVEN a valid CreateCustomerRequest (Passed after 5ms)
  STEP 1.2/1.3: WHEN I request customer creation...
  STEP 1.2/1.3: WHEN I request customer creation (Passed after 722ms)
  STEP 1.3/1.3: THEN the response should have status code "Created"...
  STEP 1.3/1.3: THEN the response should have status code "Created" (Passed after 4ms)
  STEP 1/5: GIVEN an existing customer (Passed after 786ms)
  STEP 2/5: AND a CreateCustomerRequest with the same email as existing customer...
  STEP 2/5: AND a CreateCustomerRequest with the same email as existing customer (Passed after <1ms)
  STEP 3/5: WHEN I request customer creation...
  STEP 3/5: WHEN I request customer creation (Passed after 22ms)
  STEP 4/5: THEN the response should have status code "BadRequest"...
  STEP 4/5: THEN the response should have status code "BadRequest" (Passed after <1ms)
  ...
```

## Using tabular parameters

The last feature presented in this tutorial is the usage of the tabular parameters (described on [Advanced Step Parameters](https://github.com/LightBDD/LightBDD/wiki/Advanced-Step-Parameters#tabular-parameters) LightBDD wiki page).  
We could already see it in the `Creating_customer_with_already_used_email_is_not_allowed()` scenario, however the `Creating_customer_with_missing_details_is_not_allowed()` will be better one to describe it.

Let's take a look at it then:
```c#
[Scenario]
public async Task Creating_customer_with_missing_details_is_not_allowed()
{
    await Runner.RunScenarioAsync(
        _ => Given_a_CreateCustomerRequest_with_no_details(),
        _ => When_I_request_customer_creation(),
        _ => Then_the_response_should_have_status_code(HttpStatusCode.BadRequest),
        _ => Then_the_response_should_contain_errors(Table.ExpectData(
            new Error(ErrorCodes.ValidationError, "The Email field is required."),
            new Error(ErrorCodes.ValidationError, "The FirstName field is required."),
            new Error(ErrorCodes.ValidationError, "The LastName field is required."))));
}
```

This scenario focus on the `CreateCustomerRequest` data validation in `POST /api/customers` operation. If we do not provide any details in the `CreateCustomerRequest`, we should get a `400 BadRequest` with a list of validation error details. The tabular parameters helps with providing, validating and rendering them.

Let's see the implementation of `Then_the_response_should_contain_errors()` step:
```c#
private async Task Then_the_response_should_contain_errors(VerifiableDataTable<Error> errors)
{
    var actual = await _response.GetValue().DeserializeAsync<Errors>();
    errors.SetActual(actual.Items.OrderBy(x => x.Message));
}
```

The step method accepts `VerifiableDataTable<Error> errors` parameter. It represents an expected collection of `Error` items that should be verified in the step body against the actual collection. In this particular step, the actual collection is a list of `Errors` deserialized from the `HttpResponseMessage` content.

The expected-actual collection verification is performed by calling the `errors.SetActual(...)` method. 
The verification is performed on the item properties level, which means that in this case, both, the error code and error message values will be verified.  
Please note here that the items are ordered before passing to `SetActual()` method - it is because, by default, the verification is performed in the item index order. More information on that behavior can be found on [Advanced Step Parameters # Verifiable Data Table](https://github.com/LightBDD/LightBDD/wiki/Advanced-Step-Parameters#verifiabledatatable) wiki page.  
Another thing to note here is that `SetActual()` method will not throw if verification fails - the verification outcome will be checked after the step method return instead, which allows to have multiple tabular parameters in the same step.

Now, let's come back to the scenario itself and see how the parameter is passed:
```c#
_ => Then_the_response_should_contain_errors(Table.ExpectData(
    new Error(ErrorCodes.ValidationError, "The Email field is required."),
    new Error(ErrorCodes.ValidationError, "The FirstName field is required."),
    new Error(ErrorCodes.ValidationError, "The LastName field is required.")))
```

The list of expected `Error` values is being built with `Table.ExpectData()` method, that takes the provided list and converts to the `VerifiableDataTable<Error>` instance.

Running this scenario will produce a following example output on console:
```
SCENARIO: Creating customer with missing details is not allowed
  STEP 1/4: GIVEN a CreateCustomerRequest with no details...
  STEP 1/4: GIVEN a CreateCustomerRequest with no details (Passed after 20ms)
  STEP 2/4: WHEN I request customer creation...
  STEP 2/4: WHEN I request customer creation (Passed after 604ms)
  STEP 3/4: THEN the response should have status code "BadRequest"...
  STEP 3/4: THEN the response should have status code "BadRequest" (Passed after 4ms)
  STEP 4/4: AND the response should contain errors "<table>"...
  STEP 4/4: AND the response should contain errors "<table>" (Passed after 28ms)
    errors:
    +-+---------------+--------------------------------+
    |#|Code           |Message                         |
    +-+---------------+--------------------------------+
    |=|ValidationError|The Email field is required.    |
    |=|ValidationError|The FirstName field is required.|
    |=|ValidationError|The LastName field is required. |
    +-+---------------+--------------------------------+
  SCENARIO RESULT: Passed after 781ms
```
