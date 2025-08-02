using StudyGO.Contracts.Result;

namespace StudyGO.Core.Abstractions.Base.DataCrud
{
    public interface IWritable<TModel, TId>
    {
        Task<Result<TId>> Create(TModel model);
        Task<Result<TId>> Update(TModel model);
        Task<Result<TId>> Delete(TId id);
    }
}
