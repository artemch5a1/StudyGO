using MediatR;
using StudyGO.Contracts.ValidatableMarker;
using StudyGO.Core.Abstractions.ValidationService;

namespace StudyGO.Application.UseCases.PipelineBehaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> 
    where TRequest : IRequest<TResponse>
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
            var  dtoObject = dtoProperty.GetValue(request)!;
            
            if (dtoObject is null) continue;

            dynamic dto = dtoObject;
            
            var validationResult = await _validationService.ValidateAsync(dto, cancellationToken);

            if (!validationResult.IsSuccess)
            {
                return Helper<TResponse>
                    .BuildFailure(validationResult.ErrorMessage, validationResult.ErrorType);
            }
        }

        return await next(cancellationToken);
    }
}