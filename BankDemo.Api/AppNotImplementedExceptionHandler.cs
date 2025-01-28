using Microsoft.AspNetCore.Diagnostics;

namespace BankDemo.Api
{
    public class AppNotImplementedExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is NotImplementedException)
            {
                var response = new ErrorResponse
                {
                    StatusCode = StatusCodes.Status501NotImplemented,
                    Message = "An error occurred.",
                    ExceptionMessage = exception.Message
                };

                httpContext.Response.StatusCode = StatusCodes.Status501NotImplemented;
                await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

                return true;
            }
            return false;
            
        }
    }
}