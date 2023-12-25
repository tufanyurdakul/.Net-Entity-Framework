
using System.Data;
using System.Net;
using System.Security.Authentication;
using Microsoft.AspNetCore.Http;

namespace EntityFrameworkApi.Utilities.Middleware
{
    public class ErrorHandler
    {
        private RequestDelegate _next;
        //private readonly ILogger _logger;
        public ErrorHandler(RequestDelegate next/*,ILogger logger*/)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.StackTrace);
                var response = context.Response;
                response.ContentType = "application/json";
                (HttpStatusCode code, string mes) = GetErrorMessageToResponse(exception, exception.Message);
                response.StatusCode = (int)code;
                await response.WriteAsync(mes);
            }
        }

        private (HttpStatusCode, string) GetErrorMessageToResponse(Exception exception, string message) => exception switch
        {
            NotSupportedException => (HttpStatusCode.HttpVersionNotSupported, "Version does not match"),
            KeyNotFoundException => (HttpStatusCode.NotFound, message),
            InvalidCastException or NullReferenceException or NoNullAllowedException or ArgumentOutOfRangeException => (HttpStatusCode.BadRequest, message),
            AuthenticationException => (HttpStatusCode.NonAuthoritativeInformation, message),
            InvalidDataException => (HttpStatusCode.Conflict, message),

            _ => (HttpStatusCode.InternalServerError, "Bir Hata İle Karşılaşıldı")
        };

    }
}
