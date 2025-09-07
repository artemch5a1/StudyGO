using StudyGO.Contracts.Result;

namespace StudyGO.Core.Abstractions.Base.DataCrud
{
    public interface IReadable<TModel, TId>
    {
        public Task<Result<List<TModel>>> GetAll(CancellationToken cancellationToken = default);

        public Task<Result<TModel?>> GetById(TId id, CancellationToken cancellationToken = default);
    }
}
