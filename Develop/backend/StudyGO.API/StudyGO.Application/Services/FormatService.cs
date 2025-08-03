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

        public Task<Result<List<FormatDto>>> GetAllFormats()
        {
            throw new NotImplementedException();
        }

        public Task<Result<FormatDto?>> GetFormatById()
        {
            throw new NotImplementedException();
        }
    }
}
