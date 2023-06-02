namespace chldr_data.Interfaces
{
    public interface IRepository<TEntity, TModel, TDto>
    {
        void Add(TDto dto);
        void Update(TDto dto);
        TModel Get(string entityId);
        void Delete(string entityId);
        IEnumerable<TModel> GetAll();
    }
}
