using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StudyGO.Contracts.Result;
using StudyGO.Core.Abstractions.DatabaseSeed;
using StudyGO.infrastructure.Data.SeedServices.SeedOptions;
using StudyGO.infrastructure.Entities;

namespace StudyGO.infrastructure.Data.SeedServices;

public class FormatsSeeder : ISeedService
{
    private readonly ApplicationDbContext _context;

    private readonly FormatSeedOptions _options;
    
    private readonly ILogger<FormatsSeeder> _logger;
    
    public FormatsSeeder(
        ApplicationDbContext context, 
        IOptions<FormatSeedOptions> options, 
        ILogger<FormatsSeeder> logger)
    {
        _context = context;
        _logger = logger;
        _options = options.Value;
    }
    
    public async Task<Result<int>> SeedDataAsync()
    {
        try
        {
            var formats = _options.Formats;

            if (!formats.Any())
                return Result<int>.Success(0);
            
            var existingid = await _context.FormatsEntity
                .Select(f => f.FormatId)
                .ToListAsync();
            
            var newFormats = formats
                .Where(f => !existingid.Contains(f.Id))
                .Select(f => new FormatEntity
                {
                    FormatId = f.Id,
                    Title = f.Name
                })
                .ToList();

            if (!newFormats.Any())
                return Result<int>.Success(0);

            await _context.FormatsEntity.AddRangeAsync(newFormats);

            int affectedRows = await _context.SaveChangesAsync();

            return Result<int>.Success(affectedRows);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Сидирование данных завершилось ошибкой");
            throw;
        }
    }
}