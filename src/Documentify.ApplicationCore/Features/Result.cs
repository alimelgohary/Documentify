using System.Text.Json.Serialization;

namespace Documentify.ApplicationCore.Features
{
    public class Result
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public Dictionary<string, string[]>? Errors { get; set; }
        [JsonIgnore]
        public ErrorType ErrorType { get; set; } = ErrorType.None;
    }
    public class Result<T> : Result
    {
        public T? Data { get; set; }
    }
    public class ResultFactory
    {
        public static Result Success(string? message = default)
            => new()
            {
                IsSuccess = true,
                Message = message
            };

        public static Result Failure(ErrorType errorType, string? message = default, Dictionary<string, string[]>? errors = default)
            => new()
            {
                IsSuccess = false,
                Message = message,
                Errors = errors,
                ErrorType = errorType
            };
        public static Result<TResult> Success<TResult>(TResult data, string? message = default)
            => new()
            {
                IsSuccess = true,
                Data = data,
                Message = message,
                Errors = default
            };

        public static Result<TResult> Failure<TResult>(ErrorType errorType, string? message = default, Dictionary<string, string[]>? errors = default)
            => new()
            {
                IsSuccess = false,
                Data = default,
                Message = message,
                Errors = errors,
                ErrorType = errorType
            };
    }
    public enum ErrorType
    {
        None,
        BadInput,
        NotFound,
        Unauthorized,
        Forbidden,
        ServerError
    }
}
