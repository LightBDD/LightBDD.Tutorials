using System;
using System.Collections.Generic;
using System.Linq;
using CustomerApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CustomerApi.ErrorHandling
{
    public static class ErrorHandler
    {
        public static IActionResult FromException(Exception exception)
        {
            if (exception is ApiException ex)
                return new BadRequestObjectResult(new Errors(new Error(ex.Code, ex.Message)));
            return new ObjectResult(new Errors(new Error(ErrorCodes.Unexpected, exception.Message))) { StatusCode = 500 };
        }

        public static IActionResult FromInvalidModel(ActionContext context)
        {
            return new BadRequestObjectResult(new Errors(ToErrors(context.ModelState).ToArray()));
        }

        private static IEnumerable<Error> ToErrors(ModelStateDictionary model)
        {
            foreach (var entry in model)
                foreach (var error in entry.Value.Errors)
                    yield return new Error(ErrorCodes.ValidationError, error.ErrorMessage);
        }
    }
}