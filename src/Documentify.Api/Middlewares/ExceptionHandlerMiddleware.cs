using Documentify.ApplicationCore.Common.Exceptions;
using Documentify.ApplicationCore.Features;
using FluentValidation;

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
            catch (Exception ex)
            {
                await HandleUnknownExceptionAsync(ex, context);
            }
        }
        async Task HandleValidationExceptionAsync(ValidationException ex, HttpContext context)
        {
            var loggedErrors = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage, e.AttemptedValue });
            _logger.LogInformation("Validation error: {@loggedErrors}", loggedErrors);
            var errors = ex.Errors.GroupBy(x => x.PropertyName)
                                .ToDictionary(
                                    g => g.Key,
                                    g => g.Select(e => e.ErrorMessage).ToArray()
                                );

            await Results.BadRequest(ResultFactory.Failure(ErrorType.BadInput, "One or more validation errors occurred.", errors)).ExecuteAsync(context);
        }
        async Task HandleBadRequestExceptionAsync(BadRequestException ex, HttpContext context)
        {
            _logger.LogInformation("Bad Request: {Message}", ex.Message);
            
            await Results.BadRequest(ResultFactory.Failure(ErrorType.BadInput, ex.Message)).ExecuteAsync(context);
        }
        async Task HandleNotFoundExceptionAsync(NotFoundException ex, HttpContext context)
        {
            _logger.LogInformation("Not Found Exception: {Message}", ex.Message);
            await Results.NotFound(ResultFactory.Failure(ErrorType.BadInput, ex.Message)).ExecuteAsync(context);
        }

        async Task HandleUnknownExceptionAsync(Exception ex, HttpContext context)
        {
            _logger.LogError("Unhandled Exception {Message} {StackTrace}", ex.Message, ex.StackTrace);
            
            Dictionary<string, string[]>? errors = default;
            if (_hostEnvironment.IsDevelopment())
            {
                errors = new();
                errors.Add("Exception", [ex.Message, ex.StackTrace!]);
            }

            await Results.Json(data: ResultFactory.Failure(ErrorType.BadInput, "An internal server error occurred.", errors),
                               statusCode: StatusCodes.Status500InternalServerError)
                .ExecuteAsync(context);
        }
    }
}
