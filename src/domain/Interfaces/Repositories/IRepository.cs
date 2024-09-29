namespace domain.Interfaces.Repositories
{
    public interface IRepository<TModel, TDto>
    {
        Task<List<TModel>> TakeAsync(int offset, int limit);
        Task<TModel> GetAsync(string entityId);
    }
}