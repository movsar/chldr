using chldr_data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace chldr_data.Repositories
{
    public abstract class Repository<TEntity, TModel, TDto> : IRepository<TEntity, TModel, TDto> where TEntity : class
    {
        private readonly DbContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public Repository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public async Task<TModel> GetByIdAsync(string id)
        {
            //return await _dbSet.FindAsync(id);
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TModel>> GetAllAsync()
        {
            //return await _dbSet.ToListAsync();
            throw new NotImplementedException();
        }

        public async Task AddAsync(TDto entity)
        {
            //await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            throw new NotImplementedException();

        }

        public async Task UpdateAsync(TDto entity)
        {
            //_dbSet.Update(entity);
            await _context.SaveChangesAsync();
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(TDto entity)
        {
            //_dbSet.Remove(entity);
            await _context.SaveChangesAsync();
            throw new NotImplementedException();
        }
    }

}
}