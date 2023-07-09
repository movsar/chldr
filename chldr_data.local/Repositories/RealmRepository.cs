using chldr_utils;
using chldr_data.Enums;
using Realms;
using chldr_utils.Services;
using Microsoft.EntityFrameworkCore;
using chldr_data.local.Interfaces;

namespace chldr_data.Repositories
{
    public abstract class RealmRepository<TEntity, TModel, TDto> : IRealmRepository<TModel, TDto>
        where TEntity : RealmObject, new()
        where TDto : class, new()
        where TModel : class
    {
        protected abstract RecordType RecordType { get; }

        protected readonly Realm _dbContext;
        protected readonly ExceptionHandler _exceptionHandler;
        protected readonly FileService _fileService;
        protected readonly string _userId;

        public RealmRepository(Realm context, ExceptionHandler exceptionHandler, FileService fileService, string userId)
        {
            _dbContext = context;
            _exceptionHandler = exceptionHandler;
            _fileService = fileService;
            _userId = userId;
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
        public virtual void Remove(string entityId)
        {
            var entity = _dbContext.Find<TEntity>(entityId);
            if (entity == null)
            {
                return;
            }

            _dbContext.Write(() =>
            {
                _dbContext.Remove(entity);
            });

            if (RecordType == RecordType.ChangeSet)
            {
                return;
            }
        }

        public async Task<IEnumerable<TModel>> TakeAsync(int offset, int limit)
        {
            var entities = await _dbContext.All<TEntity>()
                .Skip(offset)
                .Take(limit)
                .ToListAsync();
            return entities.Select(FromEntityShortcut).ToList();
        }
        public virtual List<TModel> GetRandoms(int limit)
        {
            var randomizer = new Random();

            var entries = _dbContext.All<TEntity>().AsEnumerable()
                  .OrderBy(x => randomizer.Next(0, 75000))
                  .OrderBy(entry => entry.GetHashCode())
                  .Take(limit)
                  .Select(FromEntityShortcut)
                  .ToList();

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