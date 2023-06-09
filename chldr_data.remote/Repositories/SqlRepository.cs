using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_data.Interfaces.Repositories;
using chldr_data.Models;
using chldr_data.remote.Services;
using chldr_data.remote.SqlEntities;
using Newtonsoft.Json;

namespace chldr_data.remote.Repositories
{
    internal abstract class SqlRepository<TModel, TDto> : IRepository<TModel, TDto>
        where TDto : class, new()
        where TModel : class
    {
        protected abstract RecordType RecordType { get; }
        protected readonly SqlContext _dbContext;
        protected readonly string _userId;

        public SqlRepository(SqlContext context, string userId)
        {
            _dbContext = context;
            _userId = userId;
        }
        public abstract TModel Get(string entityId);
        public abstract void Update(TDto dto);
        public abstract void Insert(TDto dto);
        public abstract void Delete(string entityId);

        protected void InsertChangeSet(Operation operation, string userId, string wordId, List<Change>? wordChanges = null)
        {
            var wordChangeSet = new SqlChangeSet()
            {
                Operation = (int)operation,
                UserId = userId!,
                RecordId = wordId,
                RecordType = (int)RecordType,
            };

            if (wordChanges != null)
            {
                wordChangeSet.RecordChanges = JsonConvert.SerializeObject(wordChanges);
            }

            try
            {
                _dbContext.ChangeSets.Add(wordChangeSet);
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


        public IEnumerable<TModel> Take(int limit)
        {
            //var entities = _dbContext.Set<TEntity>().Take(limit);
            //return entities.Select(e => TModel.FromEntity(e));
            return new List<TModel>();
        }
    }
}