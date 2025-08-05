using AutoMapper;
using Microsoft.Extensions.Logging;
using StudyGO.Application.Services.Account;
using StudyGO.Contracts.Dtos.Formats;
using StudyGO.Contracts.Result;
using StudyGO.Core.Abstractions.Repositories;
using StudyGO.Core.Abstractions.Services;
using StudyGO.Core.Abstractions.Utils;

namespace StudyGO.Application.Services
{
    public class FormatService : IFormatService
    {
        private readonly IFormatRepository _formatRepository;

        private readonly IMapper _mapper;

        private readonly ILogger<FormatService> _logger;

        public FormatService(
            IFormatRepository formatRepository,
            IMapper mapper,
            ILogger<FormatService> logger
        )
        {
            _formatRepository = formatRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<List<FormatDto>>> GetAllFormats()
        {
            var result = await _formatRepository.GetAll();

            return result.MapDataTo(_mapper.Map<List<FormatDto>>);
        }

        public async Task<Result<FormatDto?>> GetFormatById(int id)
        {
            var result = await _formatRepository.GetById(id);

            return result.MapDataTo(_mapper.Map<FormatDto?>);
        }
    }
}
