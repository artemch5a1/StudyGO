using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StudyGO.Contracts.Result;
using StudyGO.Core.Abstractions.Repositories;
using StudyGO.Core.Models;
using StudyGO.infrastructure.Data;
using StudyGO.infrastructure.Entites;

namespace StudyGO.infrastructure.Repositories
{
    public class FormatRepository : IFormatRepository
    {
        private readonly ApplicationDbContext _context;

        private readonly IMapper _mapper;

        private readonly ILogger<FormatRepository> _logger;

        public FormatRepository(
            ApplicationDbContext context,
            IMapper mapper,
            ILogger<FormatRepository> logger
        )
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<List<Format>>> GetAll()
        {
            try
            {
                List<FormatEntity> formatEntity = await _context.FormatsEntity.ToListAsync();

                return Result<List<Format>>.Success(_mapper.Map<List<Format>>(formatEntity));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Возникла ошибка при получении данных из БД: {ex.Message}");

                return Result<List<Format>>.Failure(ex.Message);
            }
        }

        public async Task<Result<Format?>> GetById(int id)
        {
            try
            {
                FormatEntity? formatEntity = await _context.FormatsEntity.FirstOrDefaultAsync(x =>
                    x.FormatID == id
                );
                return Result<Format?>.Success(_mapper.Map<Format?>(formatEntity));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Возникла ошибка при получении данных из БД: {ex.Message}");

                return Result<Format?>.Failure(ex.Message);
            }
        }
    }
}
