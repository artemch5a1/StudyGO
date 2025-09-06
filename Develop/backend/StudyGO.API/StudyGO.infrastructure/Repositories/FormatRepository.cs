using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StudyGO.Contracts.Result;
using StudyGO.Contracts.Result.ErrorTypes;
using StudyGO.Core.Abstractions.Repositories;
using StudyGO.Core.Models;
using StudyGO.infrastructure.Data;
using StudyGO.infrastructure.Entities;
using StudyGO.infrastructure.Extensions;

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

        public async Task<Result<List<Format>>> GetAll(
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                List<FormatEntity> formatEntity = await _context.FormatsEntity.ToListAsync(
                    cancellationToken
                );

                return Result<List<Format>>.Success(_mapper.Map<List<Format>>(formatEntity));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Возникла ошибка при получении данных из БД: {ex.Message}");

                return ex.HandleException<List<Format>>();
            }
        }

        public async Task<Result<Format?>> GetById(
            int id,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                FormatEntity? formatEntity = await _context.FormatsEntity.FirstOrDefaultAsync(
                    x => x.FormatId == id,
                    cancellationToken
                );

                if (formatEntity == null)
                    return Result<Format?>.Failure("Формата не существует", ErrorTypeEnum.NotFound);

                return Result<Format?>.Success(_mapper.Map<Format?>(formatEntity));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Возникла ошибка при получении данных из БД: {ex.Message}");

                return ex.HandleException<Format?>();
            }
        }
    }
}
