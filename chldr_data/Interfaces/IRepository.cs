namespace chldr_data.Interfaces
{
    public interface IRepository<TEntity, TModel, TDto>
    {
        Task<TModel> GetByIdAsync(string id);
        Task<IEnumerable<TModel>> GetAllAsync();
        Task AddAsync(TDto dto);
        Task UpdateAsync(TDto dto);
        Task DeleteAsync(TDto dto);
    }
}
