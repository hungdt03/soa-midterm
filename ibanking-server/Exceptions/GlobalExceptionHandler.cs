using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ibanking_server.Exceptions
{
    public class GlobalExceptionHandler : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            switch (context.Exception)
            {
                case InvalidOTPException:
                    var invalid = new
                    {
                        message = context.Exception.Message,
                        success = false,
                        statusCode = 400
                    };

                    context.Result = new ObjectResult(invalid)
                    {
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                    break;

                case NotFoundException:
                    var notFound = new
                    {
                        message = context.Exception.Message,
                        success = false,
                        statusCode = 404
                    };

                    context.Result = new ObjectResult(notFound)
                    {
                        StatusCode = StatusCodes.Status404NotFound
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

                case BadCredentialException:
                    var badCredential = new
                    {
                        message = context.Exception.Message,
                        success = false,
                        statusCode = 401
                    };

                    context.Result = new ObjectResult(badCredential)
                    {
                        StatusCode = StatusCodes.Status401Unauthorized
                    };
                    break;

            }
        }
    }
}
