using Microsoft.AspNetCore.Mvc;
using StudyGO.Contracts.Dtos.Formats;
using StudyGO.Core.Abstractions.Services;

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

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
        }

        [HttpGet("get-format-by-id/{formatID}")]
        public async Task<ActionResult<FormatDto>> GetFormatById(
            int formatID,
            CancellationToken cancellationToken
        )
        {
            var result = await _formatService.GetFormatById(formatID, cancellationToken);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
        }
    }
}
