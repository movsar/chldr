using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.Interfaces.Repositories;
using chldr_data.Models;
using chldr_data.remote.Interfaces;
using chldr_data.remote.Services;
using chldr_data.remote.SqlEntities;
using chldr_utils.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Realms;

namespace chldr_data.remote.Repositories
{
    public abstract class SqlRepository<TEntity, TModel, TDto> : ISqlRepository<TModel, TDto>
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
        public abstract List<ChangeSetModel> Add(TDto dto);
        public abstract List<ChangeSetModel> Update(TDto dto);
        public virtual List<ChangeSetModel> Remove(string entityId)
        {
            var entity = _dbContext.Set<TEntity>().Find(entityId);
            if (entity == null)
            {
                throw new NullReferenceException();
            }
            _dbContext.Remove(entity);

            if (RecordType == RecordType.ChangeSet)
            {
                throw new ArgumentException();
            }

            var changeSetEntity = CreateChangeSetEntity(Operation.Delete, entityId);
            _dbContext.ChangeSets.Add(changeSetEntity);

            _dbContext.SaveChanges();

            return new List<ChangeSetModel> { ChangeSetModel.FromEntity(changeSetEntity) };
        }

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

        protected SqlChangeSet CreateChangeSetEntity(Operation operation, string recordId, List<Change>? changes = null)
        {
            var changeSet = new SqlChangeSet()
            {
                Operation = (int)operation,
                UserId = _userId!,
                RecordId = recordId,
                RecordType = (int)RecordType,
            };

            if (changeSet.Operation == (int)Operation.Update && changes != null)
            {
                changeSet.RecordChanges = JsonConvert.SerializeObject(changes);
            }

            return changeSet;
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
        public virtual async Task<IEnumerable<TModel>> TakeAsync(int offset, int limit)
        {
            var entities = await _dbContext.Set<TEntity>()
                .Skip(offset)
                .Take(limit)

                .ToListAsync();

            return entities.Select(FromEntityShortcut).ToList();
        }
        public virtual async Task<List<TModel>> GetRandomsAsync(int limit)
        {
            var randomizer = new Random();

            var entries = await _dbContext.Set<TEntity>()
              .OrderBy(x => randomizer.Next(0, 75000))
              .OrderBy(entry => entry.GetHashCode())
              .Take(limit)
              .Select(entry => FromEntityShortcut(entry))
              .ToListAsync();

            return entries;
        }

        #region Bulk actions
        public List<ChangeSetModel> AddRange(IEnumerable<TDto> added)
        {
            var changeSets = new List<ChangeSetModel>();
            foreach (var dto in added)
            {
                var changeSet = Add(dto);
                changeSets.AddRange(changeSet);
            }
            return changeSets;
        }
        public List<ChangeSetModel> UpdateRange(IEnumerable<TDto> updated)
        {
            var changeSets = new List<ChangeSetModel>();
            foreach (var dto in updated)
            {
                var changeSet = Update(dto);
                changeSets.AddRange(changeSet);
            }
            return changeSets;
        }
        public List<ChangeSetModel> RemoveRange(IEnumerable<string> removed)
        {
            var changeSets = new List<ChangeSetModel>();
            foreach (var id in removed)
            {
                var changeSet = Remove(id);
                changeSets.AddRange(changeSet);
            }
            return changeSets;
        }
        #endregion

    }
}