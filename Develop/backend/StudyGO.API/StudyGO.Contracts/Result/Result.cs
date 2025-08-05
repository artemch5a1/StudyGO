namespace StudyGO.Contracts.Result
{
    public class Result<T> : ResultBase<T>
    {
        protected Result(T? value, bool isSuccess, string? errorMessage)
            : base(value, isSuccess, errorMessage) { }

        public static Result<T> Failure(string error) => new Result<T>(default, false, error);

        public static Result<T> FailureWithValue(string error, T value) =>
            new Result<T>(value, false, error);

        public static Result<T> Success(T value) => new Result<T>(value, true, null);

        public static Result<T> SuccessWithoutValue() => new Result<T>(default, true, null);

        public Result<AnotherT> MapDataTo<AnotherT>(Func<T, AnotherT> mapAction)
        {
            if (!IsSuccess)
            {
                return Value == null
                    ? Result<AnotherT>.Failure(ErrorMessage!)
                    : Result<AnotherT>.FailureWithValue(ErrorMessage!, mapAction(Value));
            }

            if (Value == null)
                return Result<AnotherT>.SuccessWithoutValue();

            return new Result<AnotherT>(
                mapAction.Invoke(this.Value),
                this.IsSuccess,
                this.ErrorMessage
            );
        }
    }
}
