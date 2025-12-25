namespace Documentify.ApplicationCore.Features
{
    public class Result
    {
        public bool IsSuccess { get; }
        public string Message { get; }
        public Dictionary<string, string[]>? Errors { get; }

        protected Result(bool success, string message, Dictionary<string, string[]>? errors = null)
        {
            IsSuccess = success;
            Message = message;
            Errors = errors ?? new();
        }

        public static Result Success(string message = "")
            => new(true, message);

        public static Result Failure(string message, Dictionary<string, string[]>? errors = null)
            => new(false, message, errors);
    }
    public class Result<T> : Result
    {
        public T? Value { get; }

        private Result(bool success, T? value, string message, Dictionary<string, string[]>? errors)
            : base(success, message, errors)
            => Value = value;

        public static Result<T> Success(T value, string message = "")
            => new(true, value, message, null);

        public static new Result<T> Failure(string message, Dictionary<string, string[]>? errors = null)
            => new(false, default, message, errors);
    }

}
