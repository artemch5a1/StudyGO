using Microsoft.AspNetCore.Mvc;
using StudyGO.API.Extensions;
using StudyGO.Contracts.Dtos.Subjects;
using StudyGO.Core.Abstractions.Services;

namespace StudyGO.API.Controllers.CatalogController
{
    [ApiController]
    [Route("[controller]")]
    public class SubjectController : ControllerBase
    {
        private readonly ISubjectService _subjectService;

        private readonly ILogger<SubjectController> _logger;

        public SubjectController(ISubjectService subjectService, ILogger<SubjectController> logger)
        {
            _subjectService = subjectService;
            _logger = logger;
        }

        [HttpGet("get-all-subjects")]
        public async Task<ActionResult<List<SubjectDto>>> GetAllFormats(
            CancellationToken cancellationToken
        )
        {
            var result = await _subjectService.GetAllSubjects(cancellationToken);

            return result.ToActionResult();
        }

        [HttpGet("get-subject-by-id/{subjectID}")]
        public async Task<ActionResult<SubjectDto?>> GetFormatById(
            Guid subjectID,
            CancellationToken cancellationToken
        )
        {
            var result = await _subjectService.GetSubjectById(subjectID, cancellationToken);

            return result.ToActionResult();
        }
    }
}
