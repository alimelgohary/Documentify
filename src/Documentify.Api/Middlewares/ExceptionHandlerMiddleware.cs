using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            catch (Exception ex)
            {
                await HandleUnknownExceptionAsync(ex, context);
            }
        }
        async Task HandleValidationExceptionAsync(ValidationException ex, HttpContext context)
        {
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
            _logger.LogError(ex.Message);
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
