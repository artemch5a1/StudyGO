using Microsoft.AspNetCore.Mvc;
using StudyGO.API.Extensions;
using StudyGO.Contracts.Dtos.Subjects;
using StudyGO.Core.Abstractions.Services;
using StudyGO.Core.Extensions;

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
            
            _logger.LogResult(result, 
                "Успешно получены предметы", 
                "Ошибка получения предметов");
            
            return result.ToActionResult();
        }

        [HttpGet("get-subject-by-id/{subjectId}")]
        public async Task<ActionResult<SubjectDto?>> GetFormatById(
            Guid subjectId,
            CancellationToken cancellationToken
        )
        {
            var result = await _subjectService.GetSubjectById(subjectId, cancellationToken);
            
            _logger.LogResult(result, 
                "Успешно получен предмет по id", 
                "Ошибка получения предмета по id", 
                new { SubjectId = subjectId });
            
            return result.ToActionResult();
        }
    }
}
