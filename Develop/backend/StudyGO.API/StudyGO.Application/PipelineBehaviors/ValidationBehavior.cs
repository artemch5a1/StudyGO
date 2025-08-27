using MediatR;
using StudyGO.Contracts.Result;
using StudyGO.Contracts.ValidatableMarker;
using StudyGO.Core.Abstractions.ValidationService;

namespace StudyGO.Application.PipelineBehaviors;

public class ValidationBehavior<TRequest, TResponse, TData> : IPipelineBehavior<TRequest, TResponse> 
    where TRequest : IRequest<TResponse>
    where TResponse : Result<TData>
{
    private readonly IValidationService _validationService;

    public ValidationBehavior(IValidationService validationService)
    {
        _validationService = validationService;
    }

    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        var dtoProperties = typeof(TRequest)
            .GetProperties()
            .Where(p => typeof(IValidatable).IsAssignableFrom(p.PropertyType));

        foreach (var dtoProperty in dtoProperties)
        {
            var dto = dtoProperty.GetValue(request);
            var validationResult = await _validationService.ValidateAsync(dto, cancellationToken);

            if (!validationResult.IsSuccess)
            {
                return (TResponse)Result<TData>.Failure(
                    validationResult.ErrorMessage ?? string.Empty,
                    validationResult.ErrorType
                );
            }
        }

        return await next(cancellationToken);
    }
}