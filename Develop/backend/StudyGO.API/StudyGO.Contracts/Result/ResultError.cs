using StudyGO.Contracts.Result.ErrorTypes;

namespace StudyGO.Contracts.Result
{
    public class ResultError<TData, TError> : ResultBase<TData>
    {
        public TError? ErrorValue { get; }

        protected ResultError(
            TData? value,
            TError? errorValue,
            bool isSuccess,
            string? errorMessage
        )
            : base(value, isSuccess, errorMessage)
        {
            ErrorValue = errorValue;
        }

        protected ResultError(
            string errorMessage,
            TError errorValue,
            ErrorTypeEnum errorType = ErrorTypeEnum.Unknown
        )
            : base(errorMessage, errorType)
        {
            ErrorValue = errorValue;
        }

        public static ResultError<TData, TError> Failure(
            string error,
            TError errorValue,
            ErrorTypeEnum errorType = ErrorTypeEnum.Unknown
        ) => new ResultError<TData, TError>(error, errorValue, errorType);

        public static ResultError<TData, TError> Success(TData value) =>
            new ResultError<TData, TError>(value, default, true, null);

        public static ResultError<TData, TError> SuccessWithoutValue() =>
            new ResultError<TData, TError>(default, default, true, null);
    }
}
