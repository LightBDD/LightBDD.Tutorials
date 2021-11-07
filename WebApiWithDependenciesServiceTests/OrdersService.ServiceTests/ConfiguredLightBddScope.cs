using LightBDD.Core.Configuration;
using LightBDD.XUnit2;
using OrdersService.ServiceTests;

[assembly: ClassCollectionBehavior(AllowTestParallelization = true)]
[assembly: ConfiguredLightBddScope]

namespace OrdersService.ServiceTests
{
    internal class ConfiguredLightBddScopeAttribute : LightBddScopeAttribute
    {
        protected override void OnConfigure(LightBddConfiguration configuration)
        {
            // LightBDD configuration
        }

        protected override void OnSetUp()
        {
            TestServer.Initialize();
        }

        protected override void OnTearDown()
        {
            TestServer.Dispose();
        }
    }
}
