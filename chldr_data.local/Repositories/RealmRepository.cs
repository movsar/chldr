using chldr_utils.Interfaces;
using chldr_utils;
using chldr_data.Enums;
using chldr_data.Models;
using chldr_data.DatabaseObjects.Models;
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
        protected readonly IEnumerable<ChangeSetModel> EmptyResult = new List<ChangeSetModel>();

        protected readonly Realm _dbContext;
        protected readonly ExceptionHandler _exceptionHandler;
        protected readonly IGraphQLRequestSender _graphQLRequestSender;
        public RealmRepository(Realm context, ExceptionHandler exceptionHandler, IGraphQLRequestSender graphQLRequestSender)
        {
            _dbContext = context;
            _exceptionHandler = exceptionHandler;
            _graphQLRequestSender = graphQLRequestSender;
        }
        public abstract TModel Get(string entityId);
        public abstract void Insert(TDto dto);
        public abstract void Delete(string entityId);
        public abstract void Update(TDto wordDto);
        public IEnumerable<TModel> Take(int limit)
        {
            var entities = _dbContext.All<TEntity>().Take(limit);
            //return entities.Select(e => TModel.FromEntity(e));
            return new List<TModel>();
        }

        protected static void SetPropertyValue(object obj, string propertyName, object value)
        {
            var propertyInfo = obj.GetType().GetProperty(propertyName);
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(obj, value);
            }
        }

        protected void ApplyChanges<T>(string entityId, List<Change> changes) where T : RealmObject
        {
            // Using this method, instead of updating the whole database entity, we can just update its particular, changed fields

            var sqlEntity = _dbContext.Find<T>(entityId);
            if (sqlEntity == null)
            {
                throw new NullReferenceException();
            }

            _dbContext.Write(() =>
            {
                foreach (var change in changes)
                {
                    SetPropertyValue(sqlEntity, change.Property, change.NewValue);
                }
            });
        }
    }
}