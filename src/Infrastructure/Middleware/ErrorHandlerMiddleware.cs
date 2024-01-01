using ICISAdminPortal.Application.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;

namespace ICISAdminPortal.Infrastructure.Middleware;
public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlerMiddleware> _logger;

    public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
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

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var result = string.Empty;
        context.Response.ContentType = "application/json";

        switch (exception)
        {
            case ValidationException validationException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                _logger.LogError(JsonConvert.SerializeObject(validationException.Data));

                result = JsonConvert.SerializeObject(new
                {
                    message = "SystemError ..!",
                    statusCode = validationException.ErrorCode,
                    data = validationException.Data
                });
                break;
        }
        return context.Response.WriteAsync(result);
    }
}
