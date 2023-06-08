namespace chldr_data.Interfaces.Repositories
{
    public interface IRepository<TModel, TDto>
    {
        TModel Get(string entityId);
        Task Add(string userId, TDto dto);
        Task Update(string userId, TDto dto);
        Task Delete(string userId, string entityId);
    }
}