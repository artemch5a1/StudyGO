using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using StudyGO.Contracts.Dtos.Formats;
using StudyGO.Contracts.Result;
using StudyGO.Core.Abstractions.Repositories;

namespace StudyGO.Application.UseCases.CatalogUseCases.FormatUseCases.GetAllFormats;

public class GetAllFormatsHandler : IRequestHandler<GetAllFormatsQuery, Result<List<FormatDto>>>
{
    private readonly IFormatRepository _formatRepository;

    private readonly IMapper _mapper;

    private readonly ILogger<GetAllFormatsHandler> _logger;

    public GetAllFormatsHandler(
        IFormatRepository formatRepository,
        IMapper mapper,
        ILogger<GetAllFormatsHandler> logger
    )
    {
        _formatRepository = formatRepository;
        _mapper = mapper;
        _logger = logger;
    }
    
    public async Task<Result<List<FormatDto>>> Handle(GetAllFormatsQuery request, CancellationToken cancellationToken)
    {
        var result = await _formatRepository.GetAll(cancellationToken);
            
        _logger.LogDebug("Отправлен запрос на получение форматов");
            
        return result.MapDataTo(_mapper.Map<List<FormatDto>>);
    }
}