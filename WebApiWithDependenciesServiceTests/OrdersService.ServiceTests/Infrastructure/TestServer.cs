using System;
using System.Net.Http;
using System.Threading.Tasks;
using LightBDD.Core.Execution;
using Microsoft.AspNetCore.Mvc.Testing;

namespace OrdersService.ServiceTests.Infrastructure
{
    internal class TestServer : IDisposable, IGlobalResourceSetUp
    {
        private readonly WebApplicationFactory<Startup> _testServer;

        /// <summary>
        /// HttpClient to the OrdersService under test.
        /// </summary>
        public HttpClient Client { get; }

        public TestServer()
        {
            _testServer = new WebApplicationFactory<Startup>();
            Client = _testServer.CreateDefaultClient();
        }

        public void Dispose()
        {
            _testServer?.Dispose();
        }

        public Task SetUpAsync() => Task.CompletedTask;

        public async Task TearDownAsync() => await _testServer.DisposeAsync();
    }
}