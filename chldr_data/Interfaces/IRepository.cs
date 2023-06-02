namespace chldr_data.Interfaces
{
    public interface IRepository<TEntity, TModel, TDto>
    {
        void Add(TDto dto);
        void Update(TDto dto);
        void Delete(string entityId);
        TModel Get(string entityId);
    }
}
