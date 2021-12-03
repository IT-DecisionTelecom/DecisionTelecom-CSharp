namespace ITDecision
{
    public class Result<TValue, TError>
    {
        public bool Success { get; }

        public TError Error { get; }

        public TValue Value { get; }
        
        public bool Failure => !Success;

        private Result(TValue value, bool success, TError error)
        {
            Success = success;
            Error = error;
            Value = value;
        }
        
        public static Result<T, TError> Fail<T>(TError error)
        {
            return new Result<T, TError>(default, false, error);
        }
        
        public static Result<T, TError> Ok<T>(T value)
        {
            return new Result<T, TError>(value, true, default);
        }
        
        public static implicit operator Result<TValue, TError>(TValue value)
        {
            if (value is Result<TValue, TError> result)
            {
                TError resultError = result.Failure ? result.Error : default;
                TValue resultValue = result.Success ? result.Value : default;

                return new Result<TValue, TError>(resultValue, result.Success, resultError);
            }

            return Ok(value);
        }

        public static implicit operator Result<TValue, TError>(TError error)
        {
            if (error is Result<TValue, TError> result)
            {
                TError resultError = result.Failure ? result.Error : default;
                TValue resultValue = result.Success ? result.Value : default;

                return new Result<TValue, TError>(resultValue, result.Success, resultError);
            }

            return Fail<TValue>(error);
        }
    }
}