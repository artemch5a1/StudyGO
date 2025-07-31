namespace StudyGO.Core.Abstractions.Base
{
    public interface IBaseCrud<TModel>
    {
        public List<TModel> GetAll();

        public TModel GetById(int id);

        public bool Create(TModel user);

        public bool Update(TModel user);

        public bool Delete(int id);
    }
}
