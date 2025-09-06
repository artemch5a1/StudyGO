using StudyGO.Contracts.Result;

namespace StudyGO.Core.Abstractions.Base.DataCrud
{
    public interface IWritable<TModel, TId>
    {
        Task<Result<TId>> Create(TModel model, CancellationToken cancellationToken = default);
        Task<Result<TId>> Update(TModel model, CancellationToken cancellationToken = default);
        Task<Result<TId>> Delete(TId id, CancellationToken cancellationToken = default);
    }
}
