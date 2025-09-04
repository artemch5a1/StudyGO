using MediatR;
using StudyGO.Contracts.Dtos.Formats;
using StudyGO.Contracts.Result;

namespace StudyGO.Application.UseCases.CatalogUseCases.FormatUseCases.GetFormatById;

public record GetFormatByIdQuery(int Id) : IRequest<Result<FormatDto?>>;