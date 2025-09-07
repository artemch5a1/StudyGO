using StudyGO.Core.Abstractions.Contracts;
using StudyGO.Core.Abstractions.Verification;

namespace StudyGO.API.BackgroundServices;

public class EmailVerificationWorker : BackgroundService
{
    private readonly IVerificationJobQueue _queue;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EmailVerificationWorker> _logger;

    public EmailVerificationWorker(
        IVerificationJobQueue queue, 
        IServiceProvider serviceProvider,
        ILogger<EmailVerificationWorker> logger)
    {
        _queue = queue;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var job in _queue.DequeueAllAsync(stoppingToken))
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IVerificationByLinkService>();
            try
            {
                var result = await 
                    service.CreateTokenAndSendMessage(job.UserId, job.Email, job.ConfirmEmailEndpoint, stoppingToken);

                if (!result.IsSuccess)
                {
                    await service.RollBackUser(job.UserId, stoppingToken);
                    _logger.LogWarning("Не удалось отправить письмо для UserId={UserId}", job.UserId);
                }
            }
            catch (Exception ex)
            {
                await service.RollBackUser(job.UserId, stoppingToken);
                _logger.LogError(ex, "Ошибка при обработке задания на отправку письма");
            }
        }
    }
}