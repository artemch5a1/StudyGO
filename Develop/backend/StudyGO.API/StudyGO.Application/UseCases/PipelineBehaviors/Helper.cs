using StudyGO.Contracts.Result;
using StudyGO.Contracts.Result.ErrorTypes;

namespace StudyGO.Application.UseCases.PipelineBehaviors;

public static class Helper<TResponse>
{
    public static TResponse BuildFailure(string message, ErrorTypeEnum type)
    {
        var resultType = typeof(TResponse);
        var method = resultType.GetMethod(nameof(Result<object>.Failure), new[] { typeof(string), typeof(ErrorTypeEnum) });

        return (TResponse)method!.Invoke(null, new object[] { message, type })!;
    }
}