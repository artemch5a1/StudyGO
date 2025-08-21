using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StudyGO.Contracts.Result;
using StudyGO.Core.Abstractions.DatabaseSeed;
using StudyGO.infrastructure.Data.SeedServices.SeedOptions;
using StudyGO.infrastructure.Entities;

namespace StudyGO.infrastructure.Data.SeedServices;

public class SubjectsSeeder : ISeedService
{
    private readonly ApplicationDbContext _context;

    private readonly SubjectSeedOptions _options;
    
    private readonly ILogger<SubjectsSeeder> _logger;
    
    public SubjectsSeeder(
        ApplicationDbContext context, 
        IOptions<SubjectSeedOptions> options, 
        ILogger<SubjectsSeeder> logger)
    {
        _context = context;
        _logger = logger;
        _options = options.Value;
    }
    
    public async Task<Result<int>> SeedDataAsync()
    {
        try
        {
            var subjects = _options.Subjects;
        
            if(!subjects.Any())
                return Result<int>.Success(0);
        
            await _context.Database.ExecuteSqlRawAsync(
                "TRUNCATE TABLE \"SubjectsEntity\" RESTART IDENTITY CASCADE;");
        
            var entities = subjects.Select(s => new SubjectEntity
            {
                SubjectId = Guid.NewGuid(),
                Title = s.Title,
                Description = s.Description
            });
        
            await _context.SubjectsEntity.AddRangeAsync(entities);

            int affectedRows = await _context.SaveChangesAsync();

            return Result<int>.Success(affectedRows);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Сидирование данных завершилось ошибкой");
            throw;
        }
    }
}