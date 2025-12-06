using Microsoft.Extensions.Caching.Memory;

namespace Documentify.Api.Middlewares
{
    public class IdempotencyMiddleware(RequestDelegate _next, IMemoryCache _cache, ILogger<IdempotencyMiddleware> _logger)
    {
        private record ResponseCacheEntry(int StatusCode, string ResponseBody, string? Location);
        public async Task InvokeAsync(HttpContext context)
        {
            // Only handle POST requests
            if (context.Request.Method != "POST")
            {
                await _next(context);
                return;
            }

            // Check for Idempotency-Key header
            if (!context.Request.Headers.TryGetValue("Idempotency-Key", out var idempotencyKey))
            {
                await _next(context);
                return;
            }

            // Check if response is cached
            if (_cache.TryGetValue(idempotencyKey.ToString(), out ResponseCacheEntry? cachedResponse))
            {
                context.Response.ContentType = "application/json; charset=utf-8";
                context.Response.StatusCode = cachedResponse!.StatusCode;

                if (!string.IsNullOrEmpty(cachedResponse.Location))
                    context.Response.Headers["Location"] = cachedResponse.Location;

                await context.Response.WriteAsync(cachedResponse.ResponseBody);
                _logger.LogInformation("Returned cached response for Idempotency-Key: {IdempotencyKey}", idempotencyKey.ToString());
                return;
            }

            var originalBody = context.Response.Body;
            await using var memStream = new MemoryStream();
            context.Response.Body = memStream;
            await _next(context);

            var statusCode = context.Response.StatusCode;

            // Don't cache failures
            if (statusCode > 299)
            {
                memStream.Position = 0;
                await memStream.CopyToAsync(originalBody);
                context.Response.Body = originalBody;
                return;
            }

            // Read response to cache it
            memStream.Position = 0;
            string responseText = await new StreamReader(memStream).ReadToEndAsync();

            // Cache the response for future requests
            var response = new ResponseCacheEntry(statusCode, responseText, context.Response.Headers["Location"]);
            _cache.Set(idempotencyKey.ToString(), response, TimeSpan.FromMinutes(5));

            // Write response to original body
            memStream.Position = 0;
            await memStream.CopyToAsync(originalBody);
            context.Response.Body = originalBody;
            return;
        }
    }
}
