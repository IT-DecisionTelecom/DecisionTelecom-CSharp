using ITDecision.Sms.Models;

namespace ITDecision
{
    public class Result
    {
        public bool Success { get; }

        public ErrorCode ErrorCode { get; }

        public bool Failure => !Success;

        protected Result(bool success, ErrorCode errorCode)
        {
            Success = success;
            ErrorCode = errorCode;
        }

        public static Result Fail(ErrorCode errorCode)
        {
            return new Result(false, errorCode);
        }

        public static Result<T> Fail<T>(ErrorCode errorCode)
        {
            return new Result<T>(default, false, errorCode);
        }

        public static Result Ok()
        {
            return new Result(true, default);
        }

        public static Result<T> Ok<T>(T value)
        {
            return new Result<T>(value, true, default);
        }
    }
    
    public class Result<T> : Result
    {
        public T Value { get; private set; }

        protected internal Result(T value, bool success, ErrorCode errorCode)
            : base(success, errorCode)
        {
            Value = value;
        }
    }
}