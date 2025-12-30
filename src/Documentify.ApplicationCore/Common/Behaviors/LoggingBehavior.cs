using Documentify.ApplicationCore.Features;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Documentify.ApplicationCore.Common.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse> where TResponse : Result where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
            => _logger = logger;

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
        {
            var response = await next();
            if (!response.IsSuccess)
            {
                _logger.LogInformation("Request {Name} failed. Error: {ErrorType} - {Message}",
                    typeof(TRequest).Name,
                    response.ErrorType,
                    response.Message);
            }
            return response;
        }
    }
}
