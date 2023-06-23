using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.Interfaces.Repositories;
using chldr_data.Models;
using chldr_data.remote.Services;
using chldr_data.remote.SqlEntities;
using chldr_utils.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace chldr_data.remote.Repositories
{
    internal abstract class SqlRepository<TEntity, TModel, TDto> : IRepository<TModel, TDto>
        where TDto : class, new()
        where TModel : class
        where TEntity : class
    {
        public SqlRepository(SqlContext context, FileService fileService, string userId)
        {
            _dbContext = context;
            _userId = userId;
            _fileService = fileService;
        }
        protected readonly SqlContext _dbContext;
        protected readonly FileService _fileService;
        protected readonly string _userId;

        protected abstract RecordType RecordType { get; }
        protected abstract TModel FromEntityShortcut(TEntity entity);

        public virtual TModel Get(string entityId)
        {
            var entry = _dbContext.Find<TEntity>(entityId);
            if (entry == null)
            {
                throw new Exception("There is no such word in the database");
            }

            return FromEntityShortcut(entry);
        }
        public abstract void Update(TDto dto);
        public abstract void Add(TDto dto);
        public virtual void Remove(string entityId)
        {
            var entity = _dbContext.Set<TEntity>().Find(entityId);
            if (entity == null)
            {
                return;
            }
            _dbContext.Remove(entity);
            _dbContext.SaveChanges();

            InsertChangeSet(Operation.Delete, _userId, entityId);
        }

        protected void InsertChangeSet(Operation operation, string userId, string recordId, List<Change>? changes = null)
        {
            var changeSet = new SqlChangeSet()
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
                _dbContext.ChangeSets.Add(changeSet);
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
        }

        protected static void SetPropertyValue(object obj, string propertyName, object value)
        {
            var propertyInfo = obj.GetType().GetProperty(propertyName);
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(obj, value);
            }
        }
        protected void ApplyChanges<T>(string entityId, List<Change> changes) where T : class
        {
            // Using this method, instead of updating the whole database entity, we can just update its particular, changed fields

            var sqlEntity = _dbContext.Find<T>(entityId);
            if (sqlEntity == null)
            {
                throw new NullReferenceException();
            }

            foreach (var change in changes)
            {
                SetPropertyValue(sqlEntity, change.Property, change.NewValue);
            }

            _dbContext.SaveChanges();
        }
        public virtual async Task<IEnumerable<TModel>> TakeAsync(int offset = 0, int limit = 50)
        {
            var entities = await _dbContext.Set<TEntity>().Take(limit).ToListAsync();
            return entities.Select(FromEntityShortcut).ToList();
        }
        public virtual List<TModel> GetRandoms(int limit)
        {
            var randomizer = new Random();

            var entries = _dbContext.Set<TEntity>()
              .OrderBy(x => randomizer.Next(0, 75000))
              .OrderBy(entry => entry.GetHashCode())
              .Take(limit)
              .Select(entry => FromEntityShortcut(entry));

            return entries.ToList();
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