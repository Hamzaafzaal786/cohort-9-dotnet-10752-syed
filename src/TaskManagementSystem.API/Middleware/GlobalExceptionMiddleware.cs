using System.Net;
using System.Text.Json;
using Serilog;

namespace TaskManagementSystem.API.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Always log the full exception
            Log.Error(exception, "An unhandled exception occurred: {Message}", exception.Message);

            var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";

            object response;

            if (isDevelopment)
            {
                response = new
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = "An error occurred while processing your request.",
                    Detailed = exception.Message,
                    StackTrace = exception.StackTrace
                };
            }
            else
            {
                response = new
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = "An error occurred while processing your request."
                };
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}