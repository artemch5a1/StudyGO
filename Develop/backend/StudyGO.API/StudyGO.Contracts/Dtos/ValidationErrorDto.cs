namespace StudyGO.Contracts.Dtos
{
    public class ValidationErrorDto
    {
        public string PropertyName { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorCode { get; set; }

        public ValidationErrorDto(string propertyName, string errorMessage, string errorCode = "")
        {
            PropertyName = propertyName;
            ErrorMessage = errorMessage;
            ErrorCode = errorCode;
        }
    }
}
