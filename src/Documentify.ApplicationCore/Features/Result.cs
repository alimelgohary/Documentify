namespace Documentify.ApplicationCore.Features
{
    public class Result
    {
        public bool IsSuccess { get; }
        public string? Message { get; }
        public Dictionary<string, string[]>? Errors { get; }

        protected Result(bool success, string? message = default, Dictionary<string, string[]>? errors = default)
        {
            IsSuccess = success;
            Message = message;
            Errors = errors ?? new();
        }

        public static Result Success(string? message = default)
            => new(true, message);

        public static Result Failure(string? message = default, Dictionary<string, string[]>? errors = default)
            => new(false, message, errors);
    }
    public class Result<T> : Result
    {
        public T? Data { get; }

        private Result(bool success, T? data, string? message = default, Dictionary<string, string[]>? errors = default)
            : base(success, message, errors)
            => Data = data;

        public static Result<T> Success(T data, string? message = default)
            => new(true, data, message, default);

        public static new Result<T> Failure(string? message = default, Dictionary<string, string[]>? errors = default)
            => new(false, default, message, errors);
    }

}
