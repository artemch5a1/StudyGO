namespace StudyGO.Core.Abstractions.Base.DataCrud
{
    public interface IWritable<TModel>
    {
        bool Create(TModel model);
        bool Update(TModel model);
        bool Delete(int id);
    }
}
