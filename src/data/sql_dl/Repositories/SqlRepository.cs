using domain.DatabaseObjects.Models;
using sql_dl.Interfaces;
using domain.Models;
using domain.Interfaces;
using sql_dl.SqlEntities;
using Newtonsoft.Json;
using domain.Enums;

namespace sql_dl.Repositories
{
    public abstract class SqlRepository<TEntity, TModel, TDto> : ISqlRepository<TModel, TDto>
        where TDto : class, new()
        where TModel : class
        where TEntity : class
    {
        public SqlRepository(SqlContext context, IFileService fileService, string userId)
        {
            _dbContext = context;
            _userId = userId ?? _dbContext.Users.First().Id;

            _fileService = fileService;
        }
        protected SqlContext _dbContext { get; set; }
        protected readonly IFileService _fileService;
        protected readonly string _userId;
        public abstract Task<TModel> GetAsync(string entityId);
        public abstract Task<List<ChangeSetModel>> AddAsync(TDto dto);
        public abstract Task<List<ChangeSetModel>> UpdateAsync(TDto dto);
        public virtual async Task<List<ChangeSetModel>> RemoveAsync(string entityId)
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

            await _dbContext.SaveChangesAsync();

            return new List<ChangeSetModel> { ChangeSetModel.FromEntity(changeSetEntity) };
        }

        protected abstract RecordType RecordType { get; }
        protected SqlChangeSet CreateChangeSetEntity(Operation operation, string recordId, List<Change>? changes = null)
        {
            var changeSet = new SqlChangeSet()
            {
                Operation = (int)operation,
                UserId = _userId,
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
        public abstract Task<List<TModel>> TakeAsync(int offset, int limit);
        public abstract Task<List<TModel>> GetRandomsAsync(int limit);

        #region Bulk actions
        public async Task<List<ChangeSetModel>> AddRange(IEnumerable<TDto> added)
        {
            var result = new List<ChangeSetModel>();
            foreach (var dto in added)
            {
                result.AddRange(await AddAsync(dto));
            }

            return result;
        }
        public async Task<List<ChangeSetModel>> UpdateRange(IEnumerable<TDto> updated)
        {
            var result = new List<ChangeSetModel>();
            foreach (var dto in updated)
            {
                result.AddRange(await UpdateAsync(dto));

            }

            return result;
        }
        public async Task<List<ChangeSetModel>> RemoveRange(IEnumerable<string> removed)
        {
            var result = new List<ChangeSetModel>();
            foreach (var id in removed)
            {
                result.AddRange(await RemoveAsync(id));
            }

            return result;
        }
        #endregion

    }
}