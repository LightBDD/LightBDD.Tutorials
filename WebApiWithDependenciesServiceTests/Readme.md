# Web Api Service Tests

**Technologies:** AspNetCore, Swashbuckle, LiteDB, Rebus, WireMock 
**LightBDD concepts:** partial classes, parallel execution, async steps, [composite steps](https://github.com/LightBDD/LightBDD/wiki/Composite-Steps-Definition), MessageListener

This is a sample project showing LightBDD Service Tests using WireMock for mocking dependent APIs and Rebus for message based communication.

It consist of two projects:
* [OrdersApi](https://github.com/LightBDD/LightBDD.Tutorials/tree/master/WebApiWithDependenciesServiceTests/OrdersApi) - AspNetCore WebApi project,
* [OrdersApi.ServiceTests](https://github.com/LightBDD/LightBDD.Tutorials/tree/master/WebApiWithDependenciesServiceTests/OrdersApi.ServiceTests) - Service tests project based on xunit and LightBDD.

## Quick run

1. Open `cmd.exe` in solution directory
2. Run [run-tests.cmd](https://github.com/LightBDD/LightBDD.Tutorials/blob/master/WebApiWithDependenciesServiceTests/run-tests.cmd).

The command will run the tests and open the `FeaturesReport.html` produced in `OrdersApi.ServiceTests\bin\Debug\net5\Reports\` directory.

## OrdersApi

The OrdersApi is a sample AspNetCore WebApi project, generated with `dotnet new webapi` command and extended for the tutorial purpose with a `/orders` endpoint.

The `/orders` endpoint is implemented in [OrdersController](https://github.com/LightBDD/LightBDD.Tutorials/blob/master/WebApiWithDependenciesServiceTests/OrdersApi/Controllers/OrdersController.cs) and allows following operations:
* `POST /orders` - creates a new order,
* `GET /orders/{orderId}` - retrieves an order by ID.

The order data is stored in [LiteDB](http://www.litedb.org/) database, managed by the [OrdersRepository](https://github.com/LightBDD/LightBDD.Tutorials/blob/master/WebApiWithDependenciesServiceTests/OrdersApi/Repositories/OrdersRepository.cs) class.  

### Creating new Order

The `POST /orders` operation requires [CreateOrderRequest](https://github.com/LightBDD/LightBDD.Tutorials/blob/master/WebApiWithDependenciesServiceTests/OrdersApi/Models/CreateOrderRequest.cs) containing `AccountId` and `Products` properties.  

TBC

### Swagger

Finally, the [Swashbuckle Swagger](https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle) has been added to the Api, allowing to play with it by going to `https://localhost:5001/` when application is running.

TBC