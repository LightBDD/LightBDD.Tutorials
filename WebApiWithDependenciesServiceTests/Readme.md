# Web Api Service Tests

**Technologies:** AspNetCore, Swashbuckle, LiteDB, Rebus, WireMock 
**LightBDD concepts:** partial classes, parallel execution, async steps, [composite steps](https://github.com/LightBDD/LightBDD/wiki/Composite-Steps-Definition), [MessageListener](https://github.com/LightBDD/LightBDD/wiki/Test-Utilities#messagelistener), [DI containers](https://github.com/LightBDD/LightBDD/wiki/DI-Containers#default-di-container), [global setup](https://github.com/LightBDD/LightBDD/wiki/SetUp-and-TearDown#global-setup-and-teardown)

This is a sample project showing LightBDD Service Tests using WireMock for mocking dependent APIs and Rebus for message based communication.

It consist of two projects:
* [OrderService](https://github.com/LightBDD/LightBDD.Tutorials/tree/master/WebApiWithDependenciesServiceTests/OrderService) - AspNetCore WebApi project,
* [OrderService.ServiceTests](https://github.com/LightBDD/LightBDD.Tutorials/tree/master/WebApiWithDependenciesServiceTests/OrderService.ServiceTests) - Service tests project based on xunit and LightBDD.

## Quick run

1. Open `cmd.exe` in solution directory
2. Run [run-tests.cmd](https://github.com/LightBDD/LightBDD.Tutorials/blob/master/WebApiWithDependenciesServiceTests/run-tests.cmd).

The command will run the tests and open the `FeaturesReport.html` produced in `OrderService.ServiceTests\bin\Debug\net7\Reports\` directory.

## OrderService

The OrderService is a sample AspNetCore WebApi project, generated with `dotnet new webapi` command and extended for the tutorial purpose with a `/orders` endpoint.

The `/orders` endpoint is implemented in [OrdersController](https://github.com/LightBDD/LightBDD.Tutorials/blob/master/WebApiWithDependenciesServiceTests/OrdersApi/Controllers/OrdersController.cs) and allows following operations:
* `POST /orders` - creates a new order,
* `GET /orders/{orderId}` - retrieves an order by ID.

The order data is stored in [LiteDB](http://www.litedb.org/) database, managed by the [OrdersRepository](https://github.com/LightBDD/LightBDD.Tutorials/blob/master/WebApiWithDependenciesServiceTests/OrdersApi/Repositories/OrdersRepository.cs) class.  

Service uses also [Rebus](https://github.com/rebus-org/Rebus/) framework for asynchronous message communication with other services, which is configured on [Startup](https://github.com/LightBDD/LightBDD.Tutorials/blob/master/WebApiWithDependenciesServiceTests/OrderService/Startup.cs#L43) to use file-based queueing transport.  

### Creating new Order

The `POST /orders` operation requires [CreateOrderRequest](https://github.com/LightBDD/LightBDD.Tutorials/blob/master/WebApiWithDependenciesServiceTests/OrdersApi/Models/CreateOrderRequest.cs) containing `AccountId` and `Products` properties.  

The `AccountId` is first validated by calling AccountService endpoint using [AccountServiceClient.IsValidAccount](https://github.com/LightBDD/LightBDD.Tutorials/blob/master/WebApiWithDependenciesServiceTests/OrderService/Clients/AccountServiceClient.cs#L17) method.

Upon successful validation, the new order is saved in the database using [OrdersRepository.Upsert](https://github.com/LightBDD/LightBDD.Tutorials/blob/master/WebApiWithDependenciesServiceTests/OrdersApi/Repositories/OrdersRepository.cs#L17) method.  
The controller publishes then the [OrderCreatedEvent](https://github.com/LightBDD/LightBDD.Tutorials/blob/master/WebApiWithDependenciesServiceTests/OrderService/Messages/OrderCreatedEvent.cs#L5) using service bus.

### Approving / Rejecting Orders

The order approving or rejection is implemented using messaging and [Rebus](https://github.com/rebus-org/Rebus/) framework.  

The OrderService has [OrderStatusHandler](https://github.com/LightBDD/LightBDD.Tutorials/blob/master/WebApiWithDependenciesServiceTests/OrderService/Handlers/OrderStatusHandler.cs#L14) which handles [ApproveOrderCommand](https://github.com/LightBDD/LightBDD.Tutorials/blob/I4/WebApiWithDependenciesServiceTests/OrderService/Messages/ApproveOrderCommand.cs) and [RejectOrderCommand](https://github.com/LightBDD/LightBDD.Tutorials/blob/I4/WebApiWithDependenciesServiceTests/OrderService/Messages/RejectOrderCommand.cs) messages.

For `ApproveOrderCommand` it updates order status to `Complete` and publishes [OrderProductDispatchEvent](https://github.com/LightBDD/LightBDD.Tutorials/blob/I4/WebApiWithDependenciesServiceTests/OrderService/Messages/OrderProductDispatchEvent.cs) for each product associated with an order.

For `RejectOrderCommand` it just updates order status to `Rejected`.

### Swagger

Finally, the [Swashbuckle Swagger](https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle) has been added to the Api, allowing to play with it by going to `https://localhost:5001/` when application is running.

_Please note that the solution does not contain implementations of AccountService nor service issuing `ApproveOrderCommand`/`RejectOrderCommand` which are necessary to complete the flow_

## OrderService.ServiceTests

### Infrastructure & Global Setup

The service tests uses three infrastructure components to run:
* [TestServer](https://github.com/LightBDD/LightBDD.Tutorials/blob/master/WebApiWithDependenciesServiceTests/OrderService.ServiceTests/Infrastructure/TestServer.cs) that is responsible for instantiating OrderService under test and allow interaction with it's Api via [OrdersClient](https://github.com/LightBDD/LightBDD.Tutorials/blob/master/WebApiWithDependenciesServiceTests/OrderService.ServiceTests/Infrastructure/OrdersClient.cs),
* [TestBus](https://github.com/LightBDD/LightBDD.Tutorials/blob/master/WebApiWithDependenciesServiceTests/OrderService.ServiceTests/Infrastructure/TestBus.cs) that allows to communicate with OrderService via messaging, where:
  * `MessageBus` property is used to send messages to the OrderService,
  * `Dispatcher` property is used to hook-up into events published by the OrderService,
* [MockAccountService](https://github.com/LightBDD/LightBDD.Tutorials/blob/master/WebApiWithDependenciesServiceTests/OrderService.ServiceTests/Infrastructure/MockAccountService.cs) that uses [WireMock.Net](https://github.com/WireMock-Net/WireMock.Net) and allows mocking AccountService Rest endpoints.

All of these infrastructure components are configured in [ConfiguredLightBDDScope](https://github.com/LightBDD/LightBDD.Tutorials/blob/master/WebApiWithDependenciesServiceTests/OrderService.ServiceTests/ConfiguredLightBddScope.cs#L29) and registered in the [DI container](https://github.com/LightBDD/LightBDD/wiki/DI-Containers#default-di-container) as singletons, so that a single instances of these components are used to run all scenarios.  

In addition, there is a set of [global setup and tear down](https://github.com/LightBDD/LightBDD/wiki/SetUp-and-TearDown#global-setup-and-teardown) activities registered:
* `RegisterGlobalTearDown("db cleanup", () => File.Delete(Path.Combine(AppContext.BaseDirectory, ".orders.db")))` is called first to deleted orders database after test completion. As TearDown methods are executed in reverse registration order, it will run as very last cleanup operation,
* `RegisterGlobalSetUp<TestServer>()` is called here to ensure the TestServer (as OrderService instance) is instantiated before TestBus,
* `RegisterGlobalSetUp<TestBus>()` is called as last one to connect into already working OrderService queues.

### Managing orders

The [Managing_orders](https://github.com/LightBDD/LightBDD.Tutorials/blob/master/WebApiWithDependenciesServiceTests/OrderService.ServiceTests/Features/Managing_orders.cs) feature contains scenarios for:
* creating order,
* an attempt to creating order for invalid account (AccountService Api validating negatively given accountId),
* approving order (via ApproveOrderCommand),
* rejecting order (via RejectOrderCommand),
* displatching products for approved order (via OrderProductDispatchEvent).

The scenarios mentioned above uses a mixture of Rest Api and messaging communication with OrderService and its dependencies.

The feature fixture leverages [contextual scenarios](https://github.com/LightBDD/LightBDD/wiki/Scenario-Steps-Definition#contextual-scenarios) capability and each scenario is written using [OrderContext](https://github.com/LightBDD/LightBDD.Tutorials/blob/master/WebApiWithDependenciesServiceTests/OrderService.ServiceTests/Contexts/OrderContext.cs) class to run steps.
The `OrderContext` instance is created using [dependency injection](https://github.com/LightBDD/LightBDD/wiki/DI-Containers#using-di-containers-in-contextual-scenarios) where `OrderServiceClient`, `MockAccountService` and `TestBus` dependencies are provided via its constructor.

Upon creation, `OrderContext` obtains instance [MessageListener](https://github.com/LightBDD/LightBDD/wiki/Test-Utilities#messagelistener) using `_listener = MessageListener.Start(testBus.Dispatcher);` that begins to listen for events coming from OrderService. The `OrderContext` implements `IDisposable` interface and message listener is disposed after scenario finish.

#### Mocking downstream Rest Apis

The `Given_a_valid_account()` / `Given_an_invalid_account()` steps configures WireMock to return specific response for the provided `_accountId`, which basically emulates AccountService to pass or fail account validation.

```c#
public Task Given_a_valid_account()
{
    _accountService.ConfigureGetAccount(_accountId, true);
    return Task.CompletedTask;
}

public Task Given_an_invalid_account()
{
    _accountService.ConfigureGetAccount(_accountId, false);
    return Task.CompletedTask;
}
```

#### Calling service under test Api

The `When_create_order_endpoint_is_called_for_products()` / `Then_get_order_endpoint_should_return_order_with_status()` steps interact with OrderService via its REST Api, using [OrdersClient](https://github.com/LightBDD/LightBDD.Tutorials/blob/master/WebApiWithDependenciesServiceTests/OrderService.ServiceTests/Infrastructure/OrdersClient.cs).

```c#
public async Task When_create_order_endpoint_is_called_for_products(params string[] products)
{
    var request = new CreateOrderRequest { AccountId = _accountId, Products = products };
    _response = await _client.CreateOrder(request);
}

public async Task Then_get_order_endpoint_should_return_order_with_status(Verifiable<OrderStatus> status)
{
    var order = await _client.GetOrder(_order.Id);
    status.SetActual(order!.Status);
}
```

#### Sending messages to service under test

The `When_ApproveOrderCommand_is_sent_for_this_order()` / `When_RejectOrderCommand_is_sent_for_this_order()` steps uses message bus obtained from [TestBus](https://github.com/LightBDD/LightBDD.Tutorials/blob/master/WebApiWithDependenciesServiceTests/OrderService.ServiceTests/Infrastructure/TestBus.cs) to send command messages to OrderService.

```c#
public async Task When_RejectOrderCommand_is_sent_for_this_order()
{
    await _messageBus.Send(new RejectOrderCommand { OrderId = _order.Id });
}

public async Task When_ApproveOrderCommand_is_sent_for_this_order()
{
    await _messageBus.Send(new ApproveOrderCommand { OrderId = _order.Id });
}
```

#### Asserting that events have been published

Finally, the `Then_OrderCreatedEvent_should_be_published()` / `Then_OrderStatusUpdatedEvent_should_be_published_with_status()` and `Then_OrderProductDispatchEvent_should_be_published_for_each_product()` steps uses `MessageListener` to verify that OrderService has published events.

```c#
public async Task Then_OrderCreatedEvent_should_be_published()
{
    await _listener.EnsureReceived<OrderCreatedEvent>(x => x.OrderId == _order.Id);
}

public async Task Then_OrderStatusUpdatedEvent_should_be_published_with_status(OrderStatus status)
{
    await _listener.EnsureReceived<OrderStatusUpdatedEvent>(x => x.OrderId == _order.Id && x.Status == status);
}

public async Task Then_OrderProductDispatchEvent_should_be_published_for_each_product()
{
    var messages = await _listener.EnsureReceivedMany<OrderProductDispatchEvent>(_order.Products.Length, x => x.OrderId == _order.Id);
    messages.Select(m => m.Product).ToArray()
        .ShouldBe(_order.Products, true);
}
```

These methods have very simple implementation but it is worth talking about them a bit more. The events are published in asynchronous manner thus at the time when these steps are called, events could have been already published, already received and processed or not published yet. Of course there may be also a scenario that they will be never published (due to the bug or incorrectly written assertion).

As MessageListener is created with `_listener = MessageListener.Start(testBus.Dispatcher);` code in `OrderContext` constructor, since that time it starts collecting all messages subscribed by the [TestBus](https://github.com/LightBDD/LightBDD.Tutorials/blob/master/WebApiWithDependenciesServiceTests/OrderService.ServiceTests/Infrastructure/TestBus.cs). When it comes to `_listener.EnsureReceived()` the listener has logic to verify already received messages and return immediately if expected message is on the list or will wait for the pre-configured time for it to arrive. In that case, if message arrives, the listener will return it immediately, otherwise it will throw `TimeoutException` when max waiting time is reached.
