using System;
using System.Net;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace OrdersService.ServiceTests.Infrastructure
{
    /// <summary>
    /// Mock Account Service to control Account Service API calls
    /// </summary>
    public class MockAccountService : IDisposable
    {
        private const int AccountServicePort = 5002;
        private readonly WireMockServer _server;

        public MockAccountService()
        {
            _server = WireMockServer.Start(AccountServicePort, true);
        }

        public void ConfigureGetAccount(Guid accountId, bool response)
        {
            _server.Given(Request.Create().UsingGet().WithPath($"/accounts/{accountId}/validate"))
                .RespondWith(Response.Create().WithStatusCode(HttpStatusCode.OK).WithBodyAsJson(response));
        }

        void IDisposable.Dispose()
        {
            _server?.Dispose();
        }
    }
}