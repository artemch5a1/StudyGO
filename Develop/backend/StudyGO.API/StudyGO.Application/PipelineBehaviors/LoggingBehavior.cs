using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace StudyGO.Application.PipelineBehaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> 
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }
    
    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var responseType = typeof(TResponse).Name;
        
        _logger.LogInformation("������ {RequestName} �������: {@Request}", requestName, request);

        var stopwatch = Stopwatch.StartNew();
        try
        {
            var response = await next(cancellationToken);
            _logger.LogInformation("������ {RequestName} ��������� �� {Elapsed} ��, �����: {ResponseType}",
                requestName, stopwatch.ElapsedMilliseconds, responseType);

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _logger.LogError(ex, "������ ��� ��������� {RequestName} ����� {Elapsed} ��",
                requestName, stopwatch.ElapsedMilliseconds);

            throw;
        }
    }
}