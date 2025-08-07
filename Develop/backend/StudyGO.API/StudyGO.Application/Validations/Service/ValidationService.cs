using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StudyGO.Contracts.Dtos;
using StudyGO.Contracts.Result;
using StudyGO.Contracts.Result.ErrorTypes;
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

        public ResultError<T, List<ValidationErrorDto>> Validate<T>(T model)
        {
            var validator = _serviceProvider.GetService<IValidator<T>>();

            if (validator == null)
            {
                return ResultError<T, List<ValidationErrorDto>>.Failure(
                    $"Validator for type {typeof(T).Name} not found",
                    new(),
                    ErrorTypeEnum.ServerError
                );
            }

            var validationResult = validator!.Validate(model);

            if (!validationResult.IsValid)
            {
                var firstError = validationResult.Errors.FirstOrDefault();

                return ResultError<T, List<ValidationErrorDto>>.Failure(
                    $"Validation failed: {firstError}",
                    MapToErrorDto(validationResult.Errors),
                    ErrorTypeEnum.ValidationError
                );
            }

            return ResultError<T, List<ValidationErrorDto>>.SuccessWithoutValue();
        }

        public async Task<ResultError<T, List<ValidationErrorDto>>> ValidateAsync<T>(
            T model,
            CancellationToken cancellationToken = default
        )
        {
            var validator = _serviceProvider.GetService<IValidator<T>>();

            if (validator == null)
            {
                return ResultError<T, List<ValidationErrorDto>>.Failure(
                    $"Validator for type {typeof(T).Name} not found",
                    new(),
                    ErrorTypeEnum.ServerError
                );
            }

            var validationResult = await validator!.ValidateAsync(model, cancellationToken);

            if (!validationResult.IsValid)
            {
                var firstError = validationResult.Errors.FirstOrDefault();

                return ResultError<T, List<ValidationErrorDto>>.Failure(
                    $"Validation failed: {firstError}",
                    MapToErrorDto(validationResult.Errors),
                    ErrorTypeEnum.ValidationError
                );
            }

            return ResultError<T, List<ValidationErrorDto>>.SuccessWithoutValue();
        }

        private List<ValidationErrorDto> MapToErrorDto(List<ValidationFailure> failures)
        {
            return failures
                .Select(x => new ValidationErrorDto(x.PropertyName, x.ErrorMessage, x.ErrorCode))
                .ToList();
        }
    }
}
