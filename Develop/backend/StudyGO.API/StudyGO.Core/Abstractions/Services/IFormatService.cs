using StudyGO.Contracts.Dtos.Formats;

namespace StudyGO.Core.Abstractions.Services
{
    public interface IFormatService
    {
        public Task<List<FormatDto>> GetAllFormats();

        public Task<FormatDto?> GetFormatById();
    }
}
