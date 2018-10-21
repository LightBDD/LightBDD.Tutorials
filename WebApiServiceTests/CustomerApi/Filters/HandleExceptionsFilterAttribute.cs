using CustomerApi.ErrorHandling;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CustomerApi.Filters
{
    public class HandleExceptionsFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            context.Result = ErrorHandler.FromException(context.Exception);
            context.ExceptionHandled = true;
        }
    }
}