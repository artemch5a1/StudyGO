using MediatR;
using Microsoft.AspNetCore.Mvc;
using StudyGO.API.Extensions;
using StudyGO.Application.UseCases.CatalogUseCases.SubjectUseCases.GetAllSubjects;
using StudyGO.Application.UseCases.CatalogUseCases.SubjectUseCases.GetSubjectById;
using StudyGO.Contracts.Dtos.Subjects;
using StudyGO.Core.Extensions;

namespace StudyGO.API.Controllers.CatalogController
{
    [ApiController]
    [Route("[controller]")]
    public class SubjectController : ControllerBase
    {
        private readonly ILogger<SubjectController> _logger;
        private readonly IMediator _mediator;

        public SubjectController(ILogger<SubjectController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet("get-all-subjects")]
        public async Task<ActionResult<List<SubjectDto>>> GetAllFormats(
            CancellationToken cancellationToken
        )
        {
            var result = 
                await _mediator.Send(new GetAllSubjectsQuery(), cancellationToken);
            
            _logger.LogResult(result, 
                "Успешно получены предметы", 
                "Ошибка получения предметов");
            
            return result.ToActionResult();
        }

        [HttpGet("get-subject-by-id/{subjectId:guid}")]
        public async Task<ActionResult<SubjectDto?>> GetFormatById(
            Guid subjectId,
            CancellationToken cancellationToken
        )
        {
            var result = 
                await _mediator.Send(new GetSubjectByIdQuery(subjectId), cancellationToken);
            
            _logger.LogResult(result, 
                "Успешно получен предмет по id", 
                "Ошибка получения предмета по id", 
                new { SubjectId = subjectId });
            
            return result.ToActionResult();
        }
    }
}
