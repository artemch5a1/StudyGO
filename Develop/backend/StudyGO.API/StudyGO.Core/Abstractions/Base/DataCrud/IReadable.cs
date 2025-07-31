namespace StudyGO.Core.Abstractions.Base.DataCrud
{
    public interface IReadable<TModel>
    {
        public List<TModel> GetAll();

        public TModel GetById(int id);
    }
}
