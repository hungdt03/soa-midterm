using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace tution_service.Exceptions
{
    public class GlobalExceptionHandler : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            switch (context.Exception)
            {
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

                case Exception:
                    var internalServer = new
                    {
                        message = context.Exception.Message,
                        success = false,
                        statusCode = 500
                    };

                    context.Result = new ObjectResult(internalServer)
                    {
                        StatusCode = StatusCodes.Status500InternalServerError
                    };
                    break;

                
            }
        }
    }
}
