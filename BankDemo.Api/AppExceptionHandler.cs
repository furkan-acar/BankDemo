using Microsoft.AspNetCore.Diagnostics;

namespace BankDemo.Api
{
    public class AppExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is not NotImplementedException)
            {
                var response = new ErrorResponse
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred.",
                    ExceptionMessage = exception.Message
                };
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await httpContext.Response.WriteAsJsonAsync(response, cancellationToken: cancellationToken);
                
                return true;
            }
            return false;
            
        }
    }
}