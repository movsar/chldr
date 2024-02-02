namespace chldr_app.Services
{
    public interface IDboService<TModel, TDto>
    {
        Task AddAsync(TDto entryDto, string userId);
        Task<TModel> GetAsync(string entryId);
        Task RemoveAsync(TModel entry, string userId);
        Task UpdateAsync(TDto entryDto, string userId);
    }
}