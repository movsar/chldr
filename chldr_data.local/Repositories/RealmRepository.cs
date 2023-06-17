using chldr_utils.Interfaces;
using chldr_utils;
using chldr_data.Enums;
using chldr_data.Interfaces.Repositories;
using Realms;

namespace chldr_data.Repositories
{
    public abstract class RealmRepository<TEntity, TModel, TDto> : IRepository<TModel, TDto>
        where TEntity : RealmObject, new()
        where TDto : class, new()
        where TModel : class
    {
        protected abstract RecordType RecordType { get; }

        protected readonly Realm _dbContext;
        protected readonly ExceptionHandler _exceptionHandler;
        public RealmRepository(Realm context, ExceptionHandler exceptionHandler)
        {
            _dbContext = context;
            _exceptionHandler = exceptionHandler;
        }
        protected abstract TModel FromEntityShortcut(TEntity entity);

        public abstract void Add(TDto dto);  
        public abstract void Update(TDto EntryDto);
        public TModel Get(string entityId)
        {
            var entry = _dbContext.Find<TEntity>(entityId);
            if (entry == null)
            {
                throw new Exception("There is no such word in the database");
            }

            return FromEntityShortcut(entry);
        }
        public void Remove(string entityId)
        {
            var entity = _dbContext.Find<TEntity>(entityId);
            if (entity == null)
            {
                throw new ArgumentException("Entity doesn't exist");
            }

            _dbContext.Write(() =>
            {
                _dbContext.Remove(entity);
            });
        }

        public IEnumerable<TModel> Take(int limit)
        {
            var entities = _dbContext.All<TEntity>().Take(limit);
            return entities.Select(e => FromEntityShortcut(e));
        }
        public IEnumerable<TModel> GetRandoms(int limit)
        {
            var randomizer = new Random();

            var entries = _dbContext.All<TEntity>().AsEnumerable()
              .OrderBy(x => randomizer.Next(0, 75000))
              .OrderBy(entry => entry.GetHashCode())
              .Take(limit)
              .Select(FromEntityShortcut);

            return entries;
        }

        public void AddRange(IEnumerable<TDto> added)
        {
            foreach (var dto in added)
            {
                Add(dto);
            }
        }
        public void UpdateRange(IEnumerable<TDto> updated)
        {
            foreach (var dto in updated)
            {
                Update(dto);
            }
        }
        public void RemoveRange(IEnumerable<string> removed)
        {
            foreach (var id in removed)
            {
                Remove(id);
            }
        }
    }
}