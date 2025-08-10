using Microsoft.AspNetCore.Mvc;
using StudyGO.API.Extensions;
using StudyGO.Contracts.Dtos.Formats;
using StudyGO.Core.Abstractions.Services;
using StudyGO.Core.Extensions;

namespace StudyGO.API.Controllers.CatalogController
{
    [ApiController]
    [Route("[controller]")]
    public class FormatController : ControllerBase
    {
        private readonly IFormatService _formatService;

        private readonly ILogger<FormatController> _logger;

        public FormatController(IFormatService formatService, ILogger<FormatController> logger)
        {
            _formatService = formatService;
            _logger = logger;
        }

        [HttpGet("get-all-formats")]
        public async Task<ActionResult<List<FormatDto>>> GetAllFormats(
            CancellationToken cancellationToken
        )
        {
            var result = await _formatService.GetAllFormats(cancellationToken);
            
            _logger.LogResult(result, 
                "Успешно получены форматы", 
                "Ошибка получения форматов");
            
            return result.ToActionResult();
        }

        [HttpGet("get-format-by-id/{formatId}")]
        public async Task<ActionResult<FormatDto?>> GetFormatById(
            int formatId,
            CancellationToken cancellationToken
        )
        {
            var result = await _formatService.GetFormatById(formatId, cancellationToken);
            
            _logger.LogResult(result, 
                "Успешно получен формат по id", 
                "Ошибка получения формата по id", 
                new { FormatId = formatId });
            
            return result.ToActionResult();
        }
    }
}
