# LightBDD.Tutorials

Welcome to the LightBDD Tutorials repository.

The purpose of this repository is to present how LightBDD can be used with different types of projects and tests.

At this moment, the repository does not contain many samples, but it will grow over the time.

## List of Tutorials

### [Web Api Service Tests](https://github.com/LightBDD/LightBDD.Tutorials/tree/master/WebApiServiceTests)  
A sample project showing how to use LightBDD to service test Web Api project.  
  
**Technologies:** AspNetCore, Swashbuckle, LiteDB  
**LightBDD concepts:** partial classes, parallel execution, async steps, [State\<T>](https://github.com/LightBDD/LightBDD/wiki/Scenario-State-Management#ensuring-state-is-initialized-before-use), [composite steps](https://github.com/LightBDD/LightBDD/wiki/Composite-Steps-Definition), [tabular parameters](https://github.com/LightBDD/LightBDD/wiki/Advanced-Step-Parameters#verifiabledatatable), [global setup](https://github.com/LightBDD/LightBDD/wiki/SetUp-and-TearDown#global-setup-and-teardown)

### [Web Api With Dependencies Service Tests](https://github.com/LightBDD/LightBDD.Tutorials/tree/master/WebApiWithDependenciesServiceTests)  
A sample project showing LightBDD Service Tests using WireMock for mocking dependent APIs and Rebus for message based communication.  
  
**Technologies:** AspNetCore, Swashbuckle, LiteDB, Rebus, WireMock  
**LightBDD concepts:** partial classes, parallel execution, async steps, [composite steps](https://github.com/LightBDD/LightBDD/wiki/Composite-Steps-Definition), [MessageListener](https://github.com/LightBDD/LightBDD/wiki/Test-Utilities#messagelistener), [DI containers](https://github.com/LightBDD/LightBDD/wiki/DI-Containers#default-di-container), [global setup](https://github.com/LightBDD/LightBDD/wiki/SetUp-and-TearDown#global-setup-and-teardown)

