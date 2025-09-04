using MediatR;
using StudyGO.Contracts.Dtos.Subjects;
using StudyGO.Contracts.Result;

namespace StudyGO.Application.UseCases.CatalogUseCases.SubjectUseCases.GetAllSubjects;

public record GetAllSubjectsQuery() : IRequest<Result<List<SubjectDto>>>;