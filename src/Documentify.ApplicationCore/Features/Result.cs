namespace Documentify.ApplicationCore.Features
{
    public class Result
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public Dictionary<string, string[]>? Errors { get; set; }

        public static Result Success(string? message = default)
            => new()
            {
                IsSuccess = true,
                Message = message
            };

        public static Result Failure(string? message = default, Dictionary<string, string[]>? errors = default)
            => new()
            {
                IsSuccess = false,
                Message = message,
                Errors = errors
            };
    }
    public class Result<T> : Result
    {
        public T? Data { get; set; }

        public static Result<T> Success(T data, string? message = default)
            => new()
            {
                IsSuccess = true,
                Data = data,
                Message = message,
                Errors = default
            };

        public static new Result<T> Failure(string? message = default, Dictionary<string, string[]>? errors = default)
            => new()
            {
                IsSuccess = false,
                Data = default,
                Message = message,
                Errors = errors
            };
    }

}
