using System.Net;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using PMS.Application.DTOs;

namespace PMS.API.Middleware
{

    // Created custom ExceptionMiddleware and it will return consistent error responses 
    public class ExceptionMiddleware(RequestDelegate _next) // using primary constructor for simplicity
    {
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;
            if (exception is InvalidOperationException or ArgumentException)
            {
                code = HttpStatusCode.BadRequest;
            }
            else if (exception is UnauthorizedAccessException)
            {
                code = HttpStatusCode.Unauthorized;
            }

            var response = ApiResponse<object>.Fail(
                "Request failed",
                [exception.Message]);

            var result = JsonSerializer.Serialize(response);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}
