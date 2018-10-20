using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CustomerApi.ServiceTests
{
    internal static class TestServer
    {
        private static WebApplicationFactory<Startup> Instance { get; } = new WebApplicationFactory<Startup>();
        public static HttpClient GetClient() => Instance.CreateDefaultClient();
        public static void Dispose() => Instance.Dispose();
    }
}