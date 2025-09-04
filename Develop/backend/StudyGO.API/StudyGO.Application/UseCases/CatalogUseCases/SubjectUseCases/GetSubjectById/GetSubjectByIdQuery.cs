using MediatR;
using StudyGO.Contracts.Dtos.Subjects;
using StudyGO.Contracts.Result;

namespace StudyGO.Application.UseCases.CatalogUseCases.SubjectUseCases.GetSubjectById;

public record GetSubjectByIdQuery(Guid Id) : IRequest<Result<SubjectDto?>>;