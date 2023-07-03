using System;
using System.IO;
using CustomerApi.ServiceTests;
using LightBDD.Core.Configuration;
using LightBDD.XUnit2;

[assembly: ClassCollectionBehavior(AllowTestParallelization = true)]
[assembly: ConfiguredLightBddScope]

namespace CustomerApi.ServiceTests
{
    internal class ConfiguredLightBddScopeAttribute : LightBddScopeAttribute
    {
        protected override void OnConfigure(LightBddConfiguration configuration)
        {
            configuration.ExecutionExtensionsConfiguration()
                .RegisterGlobalTearDown("db cleanup", () => File.Delete(Path.Combine(AppContext.BaseDirectory, "customers.db")))
                .RegisterGlobalSetUp("test server", TestServer.Initialize, TestServer.Dispose);
        }
    }
}
