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

        public static ResultError<TData, TError> Failure(string error, TError errorValue) =>
            new ResultError<TData, TError>(default, errorValue, false, error);

        public static ResultError<TData, TError> Success(TData value) =>
            new ResultError<TData, TError>(value, default, true, null);

        public static ResultError<TData, TError> SuccessWithoutValue() =>
            new ResultError<TData, TError>(default, default, true, null);

        public ResultError<AnotherT, TError> MapDataTo<AnotherT>(Func<TData, AnotherT> mapAction)
        {
            if (!IsSuccess)
                return ResultError<AnotherT, TError>.Failure(this.ErrorMessage!, this.ErrorValue!);

            if (Value == null)
                return ResultError<AnotherT, TError>.SuccessWithoutValue();

            return new ResultError<AnotherT, TError>(
                mapAction.Invoke(this.Value),
                this.ErrorValue,
                this.IsSuccess,
                this.ErrorMessage
            );
        }
    }
}
