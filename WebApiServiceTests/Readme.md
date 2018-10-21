# Web Api Service Tests

**Technologies:** AspNetCore, Swashbuckle, LiteDB  
**LightBDD concepts:** partial classes, parallel execution, async steps, [State\<T>](https://github.com/LightBDD/LightBDD/wiki/Scenario-State-Management#ensuring-state-is-initialized-before-use), [tabular parameters](https://github.com/LightBDD/LightBDD/wiki/Advanced-Step-Parameters#verifiabledatatable)

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

The CustomerApi.ServiceTests uses LightBDD to run behavioral tests against CustomerApi. All tests treats the Api as black box and uses only the Api endpoints to communicate. The `WebApplicationFactory<Startup>` is used from **Microsoft.AspNetCore.Mvc.Testing** package to spawn in-memory Api, following [Integration tests in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-2.1) Microsoft docs.

### One Test Server instance

The `WebApplicationFactory<Startup>` is instantiated once for the whole test run. It is managed by [TestServer](https://github.com/LightBDD/LightBDD.Tutorials/blob/master/WebApiServiceTests/CustomerApi.ServiceTests/TestServer.cs) static class, that offers a `GetClient()` method to obtain the `HttpClient` used later by tests.

The instantiation and disposal of the `TestServer` is handled by the [ConfiguredLightBddScope](https://github.com/LightBDD/LightBDD.Tutorials/blob/master/WebApiServiceTests/CustomerApi.ServiceTests/ConfiguredLightBddScope.cs)  `OnSetUp()` and `OnTearDown()` methods, guaranteeing to execute once, before any and after all tests in the assembly.

**Why the one test server instance is important?**  
Well, the service tests treats the service as a black box, which means that when it is initialized, all potentially complex service startup have to be performed (including database connection, cache population, service warming-up routines and anything else that the service may be doing). Instantiating the `TestServer` per test or even per test class will introduce the unecessary overhead that will affect the test execution time...

**Why this example does not use `IClassFixture<WebApplicationFactory<RazorPagesProject.Startup>>` pattern described on [Integration tests in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-2.1) documentation?**  
The [Shared Context between Tests](https://xunit.github.io/docs/shared-context.html) xunit documentation states that:
* using `IClassFixture<T>` makes shared instance within the tests belonging to that class, but not between test classes themselves, which means many initializations of the service code,
* using `ICollectionFixture<T>` will make one instance shared between all the tests and tests classes, but at the cost that none of the tests will run in parallel.

## 

