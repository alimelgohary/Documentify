using Documentify.ApplicationCore.Features;
using Microsoft.AspNetCore.Mvc;

namespace Documentify.Api
{
    public static class ResultExtensions
    {
        public static ActionResult<Result<TResult>> ToActionResult<TResult>(this Result<TResult> result)
        {
            if (result.IsSuccess)
            {
                return new OkObjectResult(result);
            }
            return result.ErrorType switch
            {
                ErrorType.NotFound => new NotFoundObjectResult(result),
                ErrorType.BadInput => new BadRequestObjectResult(result),
                ErrorType.Unauthorized => new UnauthorizedObjectResult(result),
                ErrorType.Forbidden => new ObjectResult(result) { StatusCode = 403 },
                _ => new ObjectResult(result) { StatusCode = 500 },
            };
        }
    }
}
