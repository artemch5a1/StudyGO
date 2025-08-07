using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StudyGO.Contracts.Result;
using StudyGO.Contracts.Result.ErrorTypes;
using StudyGO.Core.Abstractions.Repositories;
using StudyGO.Core.Models;
using StudyGO.infrastructure.Data;
using StudyGO.infrastructure.Entites;
using StudyGO.infrastructure.Extensions;

namespace StudyGO.infrastructure.Repositories
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly ApplicationDbContext _context;

        private readonly IMapper _mapper;

        private readonly ILogger<SubjectRepository> _logger;

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

        public async Task<Result<List<Subject>>> GetAll(
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                List<SubjectEntity> formatEntity = await _context.SubjectsEntity.ToListAsync(
                    cancellationToken
                );

                return Result<List<Subject>>.Success(_mapper.Map<List<Subject>>(formatEntity));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Возникла ошибка при получении данных из БД: {ex.Message}");

                return ex.HandleException<List<Subject>>();
            }
        }

        public async Task<Result<Subject?>> GetById(
            Guid id,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                SubjectEntity? formatEntity = await _context.SubjectsEntity.FirstOrDefaultAsync(
                    x => x.SubjectID == id,
                    cancellationToken
                );

                if (formatEntity == null)
                    return Result<Subject?>.Failure(
                        "Формата не существует",
                        ErrorTypeEnum.NotFound
                    );

                return Result<Subject?>.Success(_mapper.Map<Subject?>(formatEntity));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Возникла ошибка при получении данных из БД: {ex.Message}");

                return ex.HandleException<Subject?>();
            }
        }
    }
}
