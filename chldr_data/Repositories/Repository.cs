using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.SqlEntities;
using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_tools;
using System.Xml;

namespace chldr_data.Repositories
{
    public abstract class Repository<TEntity, TModel, TDto> : IRepository<TEntity, TModel, TDto> where TEntity : class
    {
        protected readonly IEnumerable<ChangeSetModel> EmptyResult = new List<ChangeSetModel>();

        protected readonly SqlContext SqlContext;
        public Repository(SqlContext context)
        {
            SqlContext = context;
        }
        protected abstract RecordType RecordType { get; set; }
        public abstract TModel Get(string entityId);
        public abstract IEnumerable<ChangeSetModel> Update(string userId, TDto dto);
        public abstract IEnumerable<ChangeSetModel> Add(string userId, TDto dto);
        public IEnumerable<ChangeSetModel> Delete(string userId, string entityId)
        {
            var entity = SqlContext.Find<TEntity>(entityId);
            if (entity == null)
            {
                throw new NullReferenceException();
            }

            SqlContext.Remove(entity);

            // Insert changeset
            var changeSetDto = new ChangeSetDto()
            {
                Operation = Operation.Delete,
                UserId = userId!,
                RecordId = entityId!,
                RecordType = RecordType,
            };
            using var unitOfWork = new UnitOfWork(SqlContext);
            unitOfWork.ChangeSets.Add(userId, changeSetDto);

            // Return changeset with updated index
            var changeSetModel = unitOfWork.ChangeSets.Get(changeSetDto.ChangeSetId);
            return new List<ChangeSetModel>() { changeSetModel };
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

            var sqlEntity = SqlContext.Find<TEntity>(entityId);
            if (sqlEntity == null)
            {
                throw new NullReferenceException();
            }

            foreach (var change in changes)
            {
                SetPropertyValue(sqlEntity, change.Property, change.NewValue);
            }
        }
    }
}