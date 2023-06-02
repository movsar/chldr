using chldr_data.DatabaseObjects.SqlEntities;
using chldr_data.Interfaces;
using chldr_tools;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;

namespace chldr_data.Repositories
{
    public abstract class Repository<TEntity, TModel, TDto> : IRepository<TEntity, TModel, TDto> where TEntity : class
    {
        protected readonly SqlContext SqlContext;
        public Repository(SqlContext context)
        {
            SqlContext = context;
        }
        public abstract void Add(TDto dto);
        public abstract TModel Get(string entityId);
        public abstract IEnumerable<TModel> GetAll();
        public abstract void Update(TDto dto);
        public void Delete(string entityId)
        {
            var entity = SqlContext.Find<TEntity>(entityId);
            SqlContext.Remove<TEntity>(entity);
        }
    }
}