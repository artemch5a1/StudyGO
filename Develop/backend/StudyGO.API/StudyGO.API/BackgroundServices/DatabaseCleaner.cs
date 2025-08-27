using StudyGO.Core.Abstractions.CleanupServices;
using StudyGO.Core.Abstractions.Repositories;

namespace StudyGO.API.BackgroundServices;

public class DatabaseCleaner : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DatabaseCleaner> _logger;

    public DatabaseCleaner(IServiceProvider serviceProvider, ILogger<DatabaseCleaner> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("������ ������� �������");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();

                var services = 
                    scope.ServiceProvider.GetServices<IDatabaseCleanupService>();

                int totalClean = 0;
                
                foreach (var service in services)
                {
                    totalClean += await service.CleanupAsync(stoppingToken);
                }
                
                _logger.LogInformation("���� ������� {count} ������� �� ��", totalClean);
                
                await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "��������� ������ ��� ������� ���� ������");
            }
        }
    }
}