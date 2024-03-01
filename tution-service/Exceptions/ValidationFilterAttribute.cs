using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace ibanking_server.Exceptions
{
    public class ValidationFilterAttribute : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var firstError = context.ModelState.Values
                    .SelectMany(v => v.Errors)
                    .FirstOrDefault();

                if (firstError != null)
                {
                    var response = new
                    {
                        success = false,
                        message = firstError.ErrorMessage,
                        data = ""
                    };
                    context.Result = new UnprocessableEntityObjectResult(response);

                }
            }
        }
    }
}
