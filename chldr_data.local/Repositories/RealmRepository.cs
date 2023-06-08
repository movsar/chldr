using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_data.Interfaces.Repositories;
using chldr_data.local.RealmEntities;
using chldr_data.Models;
using chldr_tools;
using chldr_utils;
using chldr_utils.Interfaces;
using chldr_utils.Services;
using Realms;
using System.Xml;

namespace chldr_data.Repositories
{
    public abstract class RealmRepository<TEntity, TModel, TDto> : IRepository<TModel, TDto> where TEntity : RealmObject
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
        //public TEntryModel GetById(string entityId)
        //{
        //    var entry = Database.All<RealmEntry>().FirstOrDefault(e => e.EntryId == entryId);
        //    if (entry == null)
        //    {
        //        throw new NullReferenceException();
        //    }

        //    var entryModel = EntryModelFactory.CreateEntryModel(entry) as TEntryModel;
        //    return entryModel!;
        //}

        //public List<TModel> Take(int limit, int skip = 0)
        //{
        //    var entries = Database.All<RealmEntry>().AsEnumerable()
        //        .Skip(skip).Take(limit)
        //        .Select(e => EntryModelFactory.CreateEntryModel(e) as TModel)
        //        .ToList();
        //    return entries;
        //}
        public abstract Task Add(string userId, TDto dto);
        public abstract Task Update(string userId, TDto dto);
        public abstract Task Delete(string userId, string entityId);

        protected static void SetPropertyValue(object obj, string propertyName, object value)
        {
            var propertyInfo = obj.GetType().GetProperty(propertyName);
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(obj, value);
            }
        }

        protected void ApplyChanges(string entityId, List<Change> changes)
        {
            // Using this method, instead of updating the whole database entity, we can just update its particular, changed fields

            var entity = _dbContext.Find<TEntity>(entityId);
            if (entity == null)
            {
                throw new NullReferenceException();
            }

            foreach (var change in changes)
            {
                SetPropertyValue(entity, change.Property, change.NewValue);
            }
        }
    }
}