# Web Api Service Tests

**Technologies:** AspNetCore, Swashbuckle, LiteDB  
**LightBDD concepts:** partial classes, parallel execution, async steps, [State\<T>](https://github.com/LightBDD/LightBDD/wiki/Scenario-State-Management#ensuring-state-is-initialized-before-use), [tabular parameters](https://github.com/LightBDD/LightBDD/wiki/Advanced-Step-Parameters#verifiabledatatable)

This is a sample solution showing how to use LightBDD to service test Web Api project.

It consist of two projects:
* [CustomerApi](https://github.com/LightBDD/LightBDD.Tutorials/tree/master/WebApiServiceTests/CustomerApi) - AspNetCore WebApi project,
* [CustomerApi.ServiceTests](https://github.com/LightBDD/LightBDD.Tutorials/tree/master/WebApiServiceTests/CustomerApi.ServiceTests) - Service tests project based on xunit and LightBDD.

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
