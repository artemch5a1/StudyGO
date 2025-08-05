using StudyGO.Contracts.Dtos;
using StudyGO.Contracts.Result;

namespace StudyGO.Core.Abstractions.ValidationService
{
    public interface IValidationService
    {
        Result<List<ValidationErrorDto>> Validate<T>(T model);
        Task<Result<List<ValidationErrorDto>>> ValidateAsync<T>(
            T model,
            CancellationToken cancellationToken = default
        );
    }
}
