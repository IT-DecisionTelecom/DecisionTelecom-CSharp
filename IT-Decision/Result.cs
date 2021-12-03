namespace ITDecision
{
    public class Result<TError>
    {
        public bool Success { get; }

        public TError Error { get; }

        public bool Failure => !Success;

        protected Result(bool success, TError error)
        {
            Success = success;
            Error = error;
        }

        public static Result<TError> Fail(TError error)
        {
            return new Result<TError>(false, error);
        }

        public static Result<T, TError> Fail<T>(TError error)
        {
            return new Result<T, TError>(default, false, error);
        }

        public static Result<TError> Ok()
        {
            return new Result<TError>(true, default);
        }

        public static Result<T, TError> Ok<T>(T value)
        {
            return new Result<T, TError>(value, true, default);
        }
    }
    
    public class Result<TValue, TError> : Result<TError>
    {
        public TValue Value { get; private set; }

        protected internal Result(TValue value, bool success, TError error)
            : base(success, error)
        {
            Value = value;
        }
    }
}