namespace StudyGO.Core.Abstractions.Base.DataCrud
{
    public interface IWritable<TModel, TId>
    {
        Task<bool> Create(TModel model);
        Task<bool> Update(TModel model);
        Task<bool> Delete(TId id);
    }
}
