using MediatR;
using Microsoft.AspNetCore.Mvc;
using StudyGO.API.Extensions;
using StudyGO.Application.UseCases.CatalogUseCases.FormatUseCases.GetAllFormats;
using StudyGO.Application.UseCases.CatalogUseCases.FormatUseCases.GetFormatById;
using StudyGO.Contracts.Dtos.Formats;
using StudyGO.Core.Extensions;

namespace StudyGO.API.Controllers.CatalogController
{
    [ApiController]
    [Route("[controller]")]
    public class FormatController : ControllerBase
    {
        private readonly ILogger<FormatController> _logger;

        private readonly IMediator _mediator;
        
        public FormatController(ILogger<FormatController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet("get-all-formats")]
        public async Task<ActionResult<List<FormatDto>>> GetAllFormats(
            CancellationToken cancellationToken
        )
        {
            var result = 
                await _mediator.Send(new GetAllFormatsQuery(), cancellationToken);
            
            _logger.LogResult(result, 
                "Успешно получены форматы", 
                "Ошибка получения форматов");
            
            return result.ToActionResult();
        }

        [HttpGet("get-format-by-id/{formatId:int}")]
        public async Task<ActionResult<FormatDto?>> GetFormatById(
            int formatId,
            CancellationToken cancellationToken
        )
        {
            var result = 
                await _mediator.Send(new GetFormatByIdQuery(formatId), cancellationToken);
            
            _logger.LogResult(result, 
                "Успешно получен формат по id", 
                "Ошибка получения формата по id", 
                new { FormatId = formatId });
            
            return result.ToActionResult();
        }
    }
}
