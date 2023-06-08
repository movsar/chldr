﻿using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.SqlEntities;
using chldr_data.Enums;
using chldr_data.Interfaces.Repositories;
using chldr_data.Models;
using chldr_tools;
using System.Xml;

namespace chldr_data.Repositories
{
    public abstract class SqlRepository<TEntity, TModel, TDto> : IRepository<TModel, TDto> where TEntity : class, new()
    {
        protected abstract RecordType RecordType { get; }
        protected readonly IEnumerable<ChangeSetModel> EmptyResult = new List<ChangeSetModel>();
        protected readonly SqlContext SqlContext;

        public event Action<EntryModel>? EntryUpdated;
        public event Action<EntryModel>? EntryInserted;
        public event Action<EntryModel>? EntryDeleted;
        public event Action<EntryModel>? EntryAdded;

        public SqlRepository(SqlContext context)
        {
            SqlContext = context;
        }
        public abstract TModel Get(string entityId);
        public abstract Task Update(string userId, TDto dto);
        public abstract Task Add(string userId, TDto dto);
        public async Task Delete(string userId, string entityId)
        {
            var entity = SqlContext.Find<TEntity>(entityId);
            if (entity == null)
            {
                throw new NullReferenceException();
            }
            SqlContext.Remove(entity);

            // Insert changeset
            var changeSet = new SqlChangeSet()
            {
                Operation = (int)Operation.Delete,
                UserId = userId!,
                RecordId = entityId!,
                RecordType = (int)RecordType,
            };

            SqlContext.ChangeSets.Add(changeSet);
            SqlContext.SaveChanges();
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

            var sqlEntity = SqlContext.Find<T>(entityId);
            if (sqlEntity == null)
            {
                throw new NullReferenceException();
            }

            foreach (var change in changes)
            {
                SetPropertyValue(sqlEntity, change.Property, change.NewValue);
            }

            SqlContext.SaveChanges();
        }
    }
}