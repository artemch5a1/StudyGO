using MediatR;
using StudyGO.Contracts.Result;

namespace StudyGO.Application.UseCases.PipelineBehaviors;

public class ResultEnforcementBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!IsResultType(typeof(TResponse)))
        {
            throw new InvalidOperationException(
                $"������� {typeof(TRequest).Name} ���������� ���������������� ��� ������ {typeof(TResponse).Name}. " +
                $"������ Result<T> ��� ��� ���������� ���������.");
        }

        return next(cancellationToken);
    }

    private static bool IsResultType(Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Result<>))
            return true;
        
        Type? baseType = type.BaseType;
        while (baseType != null)
        {
            if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(Result<>))
                return true;
            baseType = baseType.BaseType;
        }

        return false;
    }
}