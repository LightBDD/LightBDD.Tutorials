using System;

namespace CustomerApi.ErrorHandling
{
    internal class ApiException : InvalidOperationException
    {
        public string Code { get; }

        public ApiException(string code, string message) : base(message)
        {
            Code = code;
        }
    }
}