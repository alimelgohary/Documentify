using Documentify.ApplicationCore.Features;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Documentify.ApplicationCore.Common.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;
        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators,
            ILogger<ValidationBehavior<TRequest, TResponse>> logger)
        {
            _validators = validators;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);
                var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
                var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

                if (failures.Count != 0)
                {
                    var loggedErrors = failures.Select(e => new { e.PropertyName, e.ErrorMessage, e.AttemptedValue });
                    _logger.LogInformation("Validation error: {@loggedErrors}", loggedErrors);
                    
                    return GetResponse(failures);
                }
            }
            return await next();
        }
        TResponse GetResponse(List<ValidationFailure> failures)
        {
            var errors = failures.GroupBy(x => x.PropertyName)
                                        .ToDictionary(
                                            g => g.Key,
                                            g => g.Select(e => e.ErrorMessage).ToArray()
                                        );
            if (typeof(TResponse) == typeof(Result))
            {
                return (TResponse)(object)ResultFactory.Failure(ErrorType.BadInput, "One or more validation errors occurred.", errors);
            }
            if(typeof(TResponse).IsGenericType &&
                    typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
            {
                var resultType = typeof(TResponse).GetGenericArguments()[0];
                var failureMethod = typeof(ResultFactory)
                    .GetMethods()
                    .First(m => m.Name == nameof(ResultFactory.Failure) && m.IsGenericMethod)
                    .MakeGenericMethod(resultType);

                return (TResponse) failureMethod.Invoke(null, [ErrorType.BadInput, "One or more validation errors occurred.",  errors])!;
            }
            throw new ValidationException(failures);
        }
    }
}
