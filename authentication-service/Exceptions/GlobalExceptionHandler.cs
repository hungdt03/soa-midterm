using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace authentication_service.Exceptions
{
    public class GlobalExceptionHandler : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            switch (context.Exception)
            {
                case BadCredentialsException:
                    var badCredentials = new
                    {
                        message = context.Exception.Message,
                        success = false,
                        statusCode = 401
                    };

                    context.Result = new ObjectResult(badCredentials)
                    {
                        StatusCode = StatusCodes.Status401Unauthorized
                    };
                    break;

                case ConflictException:
                    var error = new
                    {
                        message = context.Exception.Message,
                        success = false,
                        statusCode = 409
                    };

                    context.Result = new ObjectResult(error)
                    {
                        StatusCode = StatusCodes.Status409Conflict
                    };
                    break;
            }
        }
    }
}
