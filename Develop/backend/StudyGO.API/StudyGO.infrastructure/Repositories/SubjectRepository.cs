using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StudyGO.Core.Abstractions.Repositories;
using StudyGO.Core.Models;
using StudyGO.infrastructure.Data;
using StudyGO.infrastructure.Entites;

namespace StudyGO.infrastructure.Repositories
{
    public class SubjectRepository : ISubjectRepository
    {
        private ApplicationDbContext _context;

        private IMapper _mapper;

        private ILogger<SubjectRepository> _logger;

        public SubjectRepository(
            ApplicationDbContext context,
            IMapper mapper,
            ILogger<SubjectRepository> logger
        )
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<Subject>> GetAll()
        {
            try
            {
                List<SubjectEntity> formatEntity = await _context.SubjectsEntity.ToListAsync();
                return _mapper.Map<List<Subject>>(formatEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Возникла ошибка при получении данных из БД: {ex.Message}");
                return new();
            }
        }

        public async Task<Subject?> GetById(Guid id)
        {
            try
            {
                SubjectEntity? formatEntity = await _context.SubjectsEntity.FirstOrDefaultAsync(x =>
                    x.SubjectID == id
                );
                return _mapper.Map<Subject?>(formatEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Возникла ошибка при получении данных из БД: {ex.Message}");
                return new();
            }
        }
    }
}
