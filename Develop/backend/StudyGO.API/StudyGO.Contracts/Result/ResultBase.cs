namespace StudyGO.Contracts.Result
{
    public abstract class ResultBase<T>
    {
        public T? Value { get; }

        public bool IsSuccess { get; }

        public string? ErrorMessage { get; }

        protected ResultBase(T? value, bool isSuccess, string? errorMessage)
        {
            Value = value;
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
        }
    }
}
