using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StudyGO.Contracts.Result;
using StudyGO.Core.Abstractions.DatabaseSeed;

namespace StudyGO.infrastructure.Data;

public class Seeder : ISeedProvider
{
    private readonly IServiceProvider _serviceProvider;

    private readonly ILogger<Seeder> _logger;
    
    public Seeder(IServiceProvider serviceProvider, ILogger<Seeder> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }


    public async Task<Result<int>> SeedDataBaseAsync()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();

            var provider = scope.ServiceProvider;

            var seedServices = provider.GetServices<ISeedService>();

            int totalInsert = 0;
        
            foreach (var seedService in seedServices)
            {
                var result = await seedService.SeedDataAsync();

                if (!result.IsSuccess)
                {
                    _logger.LogError("Не удалось сидировать данные {NameService}", nameof(seedService));
                }

                totalInsert += result.Value;
            }
            
            return Result<int>.Success(totalInsert);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Сидирование данных завершилось ошибкой");
            throw;
        }
    }
}