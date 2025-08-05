using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StudyGO.Contracts.Dtos;
using StudyGO.Contracts.Result;
using StudyGO.Core.Abstractions.ValidationService;

namespace StudyGO.Application.Validations.Service
{
    public class ValidationService : IValidationService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ValidationService> _logger;

        public ValidationService(
            IServiceProvider serviceProvider,
            ILogger<ValidationService> logger
        )
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public Result<List<ValidationErrorDto>> Validate<T>(T model)
        {
            var validator = _serviceProvider.GetService<IValidator<T>>();

            if (validator == null)
            {
                return Result<List<ValidationErrorDto>>.Failure(
                    $"Validator for type {typeof(T).Name} not found"
                );
            }

            var validationResult = validator!.Validate(model);

            if (!validationResult.IsValid)
            {
                var firstError = validationResult.Errors.FirstOrDefault();

                return Result<List<ValidationErrorDto>>.FailureWithValue(
                    "Validation failed",
                    MapToErrorDto(validationResult.Errors)
                );
            }

            return Result<List<ValidationErrorDto>>.SuccessWithoutValue();
        }

        public async Task<Result<List<ValidationErrorDto>>> ValidateAsync<T>(
            T model,
            CancellationToken cancellationToken = default
        )
        {
            var validator = _serviceProvider.GetService<IValidator<T>>();

            if (validator == null)
            {
                return Result<List<ValidationErrorDto>>.Failure(
                    $"Validator for type {typeof(T).Name} not found"
                );
            }

            var validationResult = await validator!.ValidateAsync(model, cancellationToken);

            if (!validationResult.IsValid)
            {
                var firstError = validationResult.Errors.FirstOrDefault();

                return Result<List<ValidationErrorDto>>.FailureWithValue(
                    "Validation failed",
                    MapToErrorDto(validationResult.Errors)
                );
            }

            return Result<List<ValidationErrorDto>>.SuccessWithoutValue();
        }

        private List<ValidationErrorDto> MapToErrorDto(List<ValidationFailure> failures)
        {
            return failures
                .Select(x => new ValidationErrorDto(x.PropertyName, x.ErrorMessage, x.ErrorCode))
                .ToList();
        }
    }
}
