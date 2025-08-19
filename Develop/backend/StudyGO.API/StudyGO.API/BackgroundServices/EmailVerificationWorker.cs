using System.Threading.Channels;
using StudyGO.Contracts.Contracts;
using StudyGO.Core.Abstractions.Verification;

namespace StudyGO.API.BackgroundServices;

public class EmailVerificationWorker : BackgroundService
{
    private readonly Channel<VerificationJob> _queue;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EmailVerificationWorker> _logger;

    public EmailVerificationWorker(
        Channel<VerificationJob> queue, 
        IServiceProvider serviceProvider,
        ILogger<EmailVerificationWorker> logger)
    {
        _queue = queue;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var job in _queue.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();

                var service = scope.ServiceProvider.GetRequiredService<IVerificationService>();

                var result = await 
                    service.CreateTokenAndSendMessage(job.UserId, job.Email, job.ConfirmEmailEndpoint, stoppingToken);

                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Не удалось отправить письмо для UserId={UserId}", job.UserId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обработке задания на отправку письма");
            }
        }
    }
}