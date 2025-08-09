using StudyGO.Contracts.Result.ErrorTypes;

namespace StudyGO.Contracts.Result
{
    public class Result<T> : ResultBase<T>
    {
        protected Result(T? value, bool isSuccess, string? errorMessage)
            : base(value, isSuccess, errorMessage) { }

        protected Result(string errorMessage, ErrorTypeEnum errorType)
            : base(errorMessage, errorType) { }

        public static Result<T> Failure(
            string error,
            ErrorTypeEnum errorType = ErrorTypeEnum.Unknown
        ) => new Result<T>(error, errorType);

        public static Result<T> Success(T value) => new Result<T>(value, true, null);

        public static Result<T> SuccessWithoutValue() => new Result<T>(default, true, null);

        public Result<TAnotherT> MapDataTo<TAnotherT>(Func<T, TAnotherT> mapAction)
        {
            if (!IsSuccess)
            {
                return Result<TAnotherT>.Failure(ErrorMessage ?? string.Empty, this.ErrorType);
            }

            if (Value == null)
                return Result<TAnotherT>.SuccessWithoutValue();

            return new Result<TAnotherT>(
                mapAction.Invoke(this.Value),
                this.IsSuccess,
                this.ErrorMessage
            );
        }
    }
}
