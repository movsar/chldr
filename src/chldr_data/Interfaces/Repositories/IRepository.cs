using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace chldr_data.Interfaces.Repositories
{
    public interface IRepository<TModel, TDto>
    {
        Task<List<TModel>> TakeAsync(int offset, int limit);
        Task<List<TModel>> GetRandomsAsync(int limit);

        Task<TModel> GetAsync(string entityId);
    }
}