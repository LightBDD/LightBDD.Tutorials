using System;
using System.Net;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace OrdersService.ServiceTests
{
    public static class AccountServiceMocks
    {
        public static void ConfigureGetAccount(this WireMockServer server, Guid accountId, bool response)
        {
            server.Given(Request.Create().UsingGet().WithPath($"/accounts/{accountId}/validate"))
                .RespondWith(Response.Create().WithStatusCode(HttpStatusCode.OK).WithBodyAsJson(response));
        }
    }
}