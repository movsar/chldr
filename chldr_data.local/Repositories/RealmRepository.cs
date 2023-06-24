using chldr_utils.Interfaces;
using chldr_utils;
using chldr_data.Enums;
using chldr_data.Interfaces.Repositories;
using Realms;
using chldr_utils.Services;
using chldr_data.Interfaces;
using Microsoft.EntityFrameworkCore;
using chldr_data.Models;
using chldr_data.local.RealmEntities;
using Newtonsoft.Json;

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
        protected readonly FileService _fileService;

        public RealmRepository(Realm context, ExceptionHandler exceptionHandler, FileService fileService)
        {
            _dbContext = context;
            _exceptionHandler = exceptionHandler;
            _fileService = fileService;
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
        }

        public async Task<IEnumerable<TModel>> TakeAsync(int offset, int limit)
        {
            var entities = await _dbContext.All<TEntity>()
                .Skip(offset)
                .Take(limit)
                .ToListAsync();
            return entities.Select(FromEntityShortcut).ToList();
        }
        public List<TModel> GetRandoms(int limit)
        {
            var randomizer = new Random();

            var entries = _dbContext.All<TEntity>().AsEnumerable()
              .OrderBy(x => randomizer.Next(0, 75000))
              .OrderBy(entry => entry.GetHashCode())
              .Take(limit)
              .Select(FromEntityShortcut);

            return entries.ToList();
        }

        protected void InsertChangeSet(Operation operation, string userId, string recordId, List<Change>? changes = null)
        {
            var changeSet = new RealmChangeSet()
            {
                Operation = (int)operation,
                UserId = userId!,
                RecordId = recordId,
                RecordType = (int)RecordType,
            };

            if (changeSet.Operation == (int)Operation.Update && changes != null)
            {
                changeSet.RecordChanges = JsonConvert.SerializeObject(changes);
            }

            try
            {
                _dbContext.Write(() =>
                {
                    _dbContext.Add(changeSet);
                });
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
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