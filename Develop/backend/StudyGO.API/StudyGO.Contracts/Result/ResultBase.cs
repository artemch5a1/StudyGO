using StudyGO.Contracts.Result.ErrorTypes;

namespace StudyGO.Contracts.Result
{
    public abstract class ResultBase<T>
    {
        public T? Value { get; }

        public bool IsSuccess { get; }

        public string? ErrorMessage { get; }

        public ErrorTypeEnum ErrorType { get; } = ErrorTypeEnum.None;

        protected ResultBase(T? value, bool isSuccess, string? errorMessage)
        {
            Value = value;
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
        }

        protected ResultBase(string errorMessage, ErrorTypeEnum errorType)
        {
            ErrorMessage = errorMessage;
            ErrorType = errorType;
            IsSuccess = false;
            Value = default(T);
        }
    }
}
