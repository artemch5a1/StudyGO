namespace StudyGO.Core.Abstractions.Base.DataCrud
{
    public interface IWritable<TModel, TId>
    {
        Task<TId> Create(TModel model);
        Task<bool> Update(TModel model);
        Task<bool> Delete(TId id);
    }
}
