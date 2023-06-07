using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.SqlEntities;
using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.local.RealmEntities;
using chldr_data.Models;
using chldr_tools;
using Realms;
using System.Xml;

namespace chldr_data.Repositories
{
    public abstract class RealmRepository<TEntity, TModel, TDto> : IRepository<TEntity, TModel, TDto> where TEntity : RealmObject
    {
        protected abstract RecordType RecordType { get; }
        protected readonly IEnumerable<ChangeSetModel> EmptyResult = new List<ChangeSetModel>();
        protected readonly Realm DbContext;
        public RealmRepository(Realm context)
        {
            DbContext = context;
        }
        public abstract TModel Get(string entityId);
        public abstract IEnumerable<ChangeSetModel> Update(string userId, TDto dto);
        public abstract IEnumerable<ChangeSetModel> Add(string userId, TDto dto);
        public IEnumerable<ChangeSetModel> Delete(string userId, string entityId)
        {
            var entity = DbContext.Find<TEntity>(entityId);
            if (entity == null)
            {
                throw new NullReferenceException();
            }

            RealmChangeSet changeSet = null;

            DbContext.Write(() =>
            {
                DbContext.Remove(entity);

                // Insert changeset
                changeSet = new RealmChangeSet()
                {
                    Operation = (int)Operation.Delete,
                    UserId = userId!,
                    RecordId = entityId!,
                    RecordType = (int)RecordType,
                };

                DbContext.Add(changeSet);
            });

            // Return changeset with updated index
            changeSet = DbContext.Find<RealmChangeSet>(changeSet.ChangeSetId);
            return new List<ChangeSetModel>() { ChangeSetModel.FromEntity(changeSet) };
        }

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

            var entity = DbContext.Find<TEntity>(entityId);
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