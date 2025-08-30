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
            try
            {
                var validator = _serviceProvider.GetRequiredService<IValidator<T>>();

                var validationResult = validator.Validate(model);

                return ValidateProccessing<T>(validationResult);
            }
            catch (InvalidOperationException e)
            {
                _logger.LogError(e, "Использование валидации для модели, для которой она не была определена");
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Неизвестная ошибка валидации");
                throw;
            }
        }

        public async Task<ResultError<T, List<ValidationErrorDto>>> ValidateAsync<T>(
            T model,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                var validator = _serviceProvider.GetRequiredService<IValidator<T>>();

                var validationResult = await validator.ValidateAsync(model, cancellationToken);

                return ValidateProccessing<T>(validationResult);
            }
            catch (InvalidOperationException e)
            {
                _logger.LogError(e, "Использование валидации для модели, для которой она не была определена");
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Неизвестная ошибка валидации");
                throw;
            }
        }

        public async Task<ResultError<object, List<ValidationErrorDto>>> 
            ValidateDynamicAsync(object model, CancellationToken cancellationToken = default)
        {
            try
            {
                dynamic dynModel = model;
                dynamic result = await ValidateAsync(dynModel, cancellationToken);

                if (result.IsSuccess)
                    return ResultError<object, List<ValidationErrorDto>>.SuccessWithoutValue();
            
                return ResultError<object, List<ValidationErrorDto>>.Failure(
                    result.ErrorMessage,
                    result.ErrorValue,
                    result.ErrorType
                );
            }
            catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException e)
            {
                _logger.LogError(e, "Dynamic binder failed. Возможно, не найден валидатор для типа {Type}", model.GetType().Name);
                throw;
            }
            catch (InvalidCastException e)
            {
                _logger.LogError(e, "Dynamic cast failed при приведении результата валидации");
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Неизвестная ошибка динамической валидации");
                throw;
            }
        }

        private ResultError<T, List<ValidationErrorDto>> ValidateProccessing<T>(ValidationResult result)
        {
            if (!result.IsValid)
            {
                var firstError = result.Errors.FirstOrDefault();

                return ResultError<T, List<ValidationErrorDto>>.Failure(
                    $"Validation failed: {firstError}",
                    MapToErrorDto(result.Errors),
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
