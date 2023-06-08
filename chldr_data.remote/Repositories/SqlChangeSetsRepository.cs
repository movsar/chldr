﻿using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_data.Interfaces.Repositories;
using chldr_data.remote.Services;
using chldr_data.remote.SqlEntities;
using chldr_tools;

namespace chldr_data.remote.Repositories
{
    public class SqlChangeSetsRepository : SqlRepository<SqlChangeSet, ChangeSetModel, ChangeSetDto>, IChangeSetsRepository
    {
        protected override RecordType RecordType => throw new Exception("Shouldn't be used");

        public SqlChangeSetsRepository(SqlContext sqlContext) : base(sqlContext) { }

        public override ChangeSetModel Get(string entityId)
        {
            var changeSet = _dbContext.Find<SqlChangeSet>(entityId);
            return ChangeSetModel.FromEntity(changeSet);
        }

        public void AddRange(IEnumerable<ChangeSetDto> dtos)
        {
            var entities = dtos.Select(d => (SqlChangeSet)SqlChangeSet.FromDto(d));
            _dbContext.AddRange(entities);
        }

        public IEnumerable<ChangeSetModel> GetLatest(int limit)
        {
            var models = _dbContext.ChangeSets
                .OrderByDescending(c => c.ChangeSetIndex)
                .Take(limit)
                .Select(c => (ChangeSetModel)ChangeSetModel.FromEntity(c));

            return models;
        }

        public IEnumerable<ChangeSetModel> Get(string[] changeSetIds)
        {
            var models = _dbContext.ChangeSets
                .Where(c => changeSetIds.Contains(c.ChangeSetId))
                .Select(c => ChangeSetModel.FromEntity(c));

            return models;
        }

        public override async Task Update(string userId, ChangeSetDto dto)
        {
            throw new Exception("This method should never be called for ChangeSets, they're immutable");
        }

        public override async Task Insert(string userId, ChangeSetDto dto)
        {
            var changeSet = SqlChangeSet.FromDto(dto);
            _dbContext.Add(changeSet);
        }
    }
}
