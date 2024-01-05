using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using ICISAdminPortal.Application.Common.Interfaces;
using System.Net;
using Newtonsoft.Json;
using ICISAdminPortal.Application.Exceptions;
using ICISAdminPortal.Application.Common.Exceptions;
using ICISAdminPortal.Application.Resources;

namespace ICISAdminPortal.Infrastructure.Middleware;
internal class ExceptionMiddleware : IMiddleware
{
    private readonly ICurrentUser _currentUser;
    private readonly IStringLocalizer _t;
    private readonly ISerializerService _jsonSerializer;

    public ExceptionMiddleware(
        ICurrentUser currentUser,
        IStringLocalizer<ExceptionMiddleware> localizer,
        ISerializerService jsonSerializer)
    {
        _currentUser = currentUser;
        _t = localizer;
        _jsonSerializer = jsonSerializer;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
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

                result = JsonConvert.SerializeObject(new
                {
                    message = LocalizeResource.SystemError,
                    statusCode = validationException.ErrorCode,
                    data = validationException.Data
                });
                break;
        }
        return context.Response.WriteAsync(result);
    }
}