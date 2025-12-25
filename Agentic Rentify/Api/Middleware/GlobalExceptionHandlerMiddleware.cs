using System.Net;
using System.Text.Json;
using FluentValidation;
using Stripe;
using Agentic_Rentify.Core.Exceptions;

namespace Agentic_Rentify.Api.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
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

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";

        if (ex is ValidationException validationException)
        {
            _logger.LogWarning(ex, "Validation failed");
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            var validationErrors = validationException.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            var response = new
            {
                statusCode = context.Response.StatusCode,
                message = "Validation failed",
                errors = validationErrors
            };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            return;
        }
        if (ex is BadRequestException badRequest)
        {
            _logger.LogInformation("Bad Request: {Message}", badRequest.Message);
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            var response = new
            {
                statusCode = context.Response.StatusCode,
                message = badRequest.Message
            };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            return;
        }
        if (ex is UnauthorizedException unauthorized)
        {
            _logger.LogInformation("Unauthorized: {Message}", unauthorized.Message);
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            var response = new
            {
                statusCode = context.Response.StatusCode,
                message = unauthorized.Message
            };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            return;
        }
        if (ex is NotFoundException notFound)
        {
            _logger.LogInformation("Not Found: {Message}", notFound.Message);
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            var response = new
            {
                statusCode = context.Response.StatusCode,
                message = notFound.Message
            };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            return;
        }
        if (ex is StripeException stripeException)
        {
            _logger.LogWarning(ex, "Stripe error");
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            var response = new
            {
                statusCode = context.Response.StatusCode,
                message = stripeException.Message,
                stripeError = stripeException.StripeError?.Message
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            return;
        }

        _logger.LogError(ex, "An unexpected error occurred.");
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        var generic = new
        {
            statusCode = context.Response.StatusCode,
            message = "An unexpected error occurred.",
            detail = _env.IsDevelopment() ? ex.Message : null,
            stackTrace = _env.IsDevelopment() ? ex.StackTrace : null
        };
        await context.Response.WriteAsync(JsonSerializer.Serialize(generic));
    }
}
