using StudyGO.Contracts.Dtos;
using StudyGO.Contracts.Result;

namespace StudyGO.Core.Abstractions.ValidationService
{
    public interface IValidationService
    {
        ResultError<T, List<ValidationErrorDto>> Validate<T>(T model);
        Task<ResultError<T, List<ValidationErrorDto>>> ValidateAsync<T>(
            T model,
            CancellationToken cancellationToken = default
        );

        Task<ResultError<object, List<ValidationErrorDto>>> ValidateDynamicAsync
            (
                object model,
                CancellationToken cancellationToken = default
                );
    }
}
