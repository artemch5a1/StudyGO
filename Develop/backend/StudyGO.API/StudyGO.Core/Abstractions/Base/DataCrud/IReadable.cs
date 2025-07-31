namespace StudyGO.Core.Abstractions.Base.DataCrud
{
    public interface IReadable<TModel, TId>
    {
        public Task<List<TModel>> GetAll();

        public Task<TModel?> GetById(TId id);
    }
}
