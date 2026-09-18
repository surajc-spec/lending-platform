using FluentValidation;
using System.Net;
using System.Text.Json;

namespace LendingPlatform.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            await HandleValidationExceptionAsync(context, ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "An unexpected error occurred while processing the request.");

            await HandleUnexpectedExceptionAsync(context);
        }
    }

    private static async Task HandleValidationExceptionAsync(
        HttpContext context,
        ValidationException exception)
    {
        context.Response.StatusCode =
            (int)HttpStatusCode.BadRequest;

        context.Response.ContentType = "application/json";

        var errors = exception.Errors
            .GroupBy(error => error.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group
                    .Select(error => error.ErrorMessage)
                    .ToArray());

        var response = new
        {
            message = "One or more validation errors occurred.",
            errors
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response));
    }

    private static async Task HandleUnexpectedExceptionAsync(
        HttpContext context)
    {
        context.Response.StatusCode =
            (int)HttpStatusCode.InternalServerError;

        context.Response.ContentType = "application/json";

        var response = new
        {
            message = "An unexpected error occurred."
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response));
    }
}