using Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.ErrorHandling;

// Oversætter exceptions til HTTP svar i det standardiserese ProblemDetails format. 
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        ProblemDetails problem = exception switch
        {
            ValidationException validation => new ValidationProblemDetails(
                validation.Errors.ToDictionary(e => e.Key, e => new[] { e.Value }))
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Der er fejl i de indtastede oplysninger."
            },
            NotFoundException notFound => new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Ikke fundet.",
                Detail = notFound.Message
            },
            _ => new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Der er opstået en uventet fejl."
            }
        };

        if (problem.Status == StatusCodes.Status500InternalServerError)
            _logger.LogError(exception, "Uventet fejl ved {Method} {Path}", context.Request.Method, context.Request.Path);

        context.Response.StatusCode = problem.Status!.Value;

        await context.Response.WriteAsJsonAsync(problem, problem.GetType(), options: null,
            contentType: "application/problem+json", cancellationToken);

        return true;
    }
}
