using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using StudyGO.Contracts.Dtos.Formats;
using StudyGO.Contracts.Result;
using StudyGO.Core.Abstractions.Repositories;

namespace StudyGO.Application.UseCases.CatalogUseCases.FormatUseCases.GetFormatById;

public class GetFormatByIdHandler : IRequestHandler<GetFormatByIdQuery, Result<FormatDto?>>
{
    private readonly IFormatRepository _formatRepository;

    private readonly IMapper _mapper;

    private readonly ILogger<GetFormatByIdHandler> _logger;

    public GetFormatByIdHandler(
        IFormatRepository formatRepository,
        IMapper mapper,
        ILogger<GetFormatByIdHandler> logger
    )
    {
        _formatRepository = formatRepository;
        _mapper = mapper;
        _logger = logger;
    }
    
    public async Task<Result<FormatDto?>> Handle(GetFormatByIdQuery request, CancellationToken cancellationToken)
    {
        int id = request.Id;
        
        var result = await _formatRepository.GetById(id, cancellationToken);
            
        _logger.LogDebug("Отправлен запрос на получение формата по id {FormatId}", id);
            
        return result.MapDataTo(_mapper.Map<FormatDto?>);
    }
}