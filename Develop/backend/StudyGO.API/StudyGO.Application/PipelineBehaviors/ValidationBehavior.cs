using MediatR;
using StudyGO.Contracts.Result;
using StudyGO.Contracts.Result.ErrorTypes;
using StudyGO.Contracts.ValidatableMarker;
using StudyGO.Core.Abstractions.ValidationService;

namespace StudyGO.Application.PipelineBehaviors;

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
                var resultType = typeof(TResponse);
                var failureMethod = resultType.GetMethod("Failure", new[] { typeof(string), typeof(ErrorTypeEnum) });

                if (failureMethod != null)
                {
                    return (TResponse)failureMethod.Invoke(null, new object[]
                    {
                        validationResult.ErrorMessage ?? "",
                        validationResult.ErrorType
                    })!;
                }
            }
        }

        return await next(cancellationToken);
    }
}