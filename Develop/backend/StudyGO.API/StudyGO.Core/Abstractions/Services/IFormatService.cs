using StudyGO.Contracts.Dtos.Formats;
using StudyGO.Contracts.Result;

namespace StudyGO.Core.Abstractions.Services
{
    public interface IFormatService
    {
        public Task<Result<List<FormatDto>>> GetAllFormats();

        public Task<Result<FormatDto?>> GetFormatById();
    }
}
