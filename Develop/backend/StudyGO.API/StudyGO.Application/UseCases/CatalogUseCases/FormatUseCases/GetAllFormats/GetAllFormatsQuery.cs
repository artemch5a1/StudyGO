using MediatR;
using StudyGO.Contracts.Dtos.Formats;
using StudyGO.Contracts.Result;

namespace StudyGO.Application.UseCases.CatalogUseCases.FormatUseCases.GetAllFormats;

public record GetAllFormatsQuery() : IRequest<Result<List<FormatDto>>>;