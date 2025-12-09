using Documentify.ApplicationCore.Common.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Documentify.Api.Middlewares
{
    public class ExceptionHandlerMiddleware(IHostEnvironment _hostEnvironment,
        ILogger<ExceptionHandlerMiddleware> _logger,
        RequestDelegate _next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException ex)
            {
                await HandleValidationExceptionAsync(ex, context);
            }
            catch (NotFoundException ex)
            {
                _logger.LogInformation("Not Found Exception: {Message}", ex.Message);
                var problem = new ProblemDetails()
                {
                    Status = StatusCodes.Status404NotFound,
                    Detail = ex.Message
                };
                await Results.Problem(problem).ExecuteAsync(context);
            }
            catch (Exception ex)
            {
                await HandleUnknownExceptionAsync(ex, context);
            }
        }
        async Task HandleValidationExceptionAsync(ValidationException ex, HttpContext context)
        {
            var loggedErrors = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage, e.AttemptedValue });
            _logger.LogInformation("Validation error: {@loggedErrors}", loggedErrors);
            await Results.ValidationProblem(
                errors: ex.Errors.GroupBy(x => x.PropertyName)
                                .ToDictionary(
                                    g => g.Key,
                                    g => g.Select(e => e.ErrorMessage).ToArray()
                                )
            ).ExecuteAsync(context);
        }
        async Task HandleUnknownExceptionAsync(Exception ex, HttpContext context)
        {
            _logger.LogError("Unhandled Exception {Message} {StackTrace}", ex.Message, ex.StackTrace);
            var problem = new ProblemDetails()
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An internal server error occurred."
            };

            if (_hostEnvironment.IsDevelopment())
            {
                problem.Extensions.Add("message", ex.Message);
                problem.Extensions.Add("stackTrace", ex.StackTrace);
            }

            await Results.Problem(problem).ExecuteAsync(context);
        }
    }
}
