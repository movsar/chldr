﻿using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.Interfaces.Repositories;
using chldr_data.sql.Services;
using chldr_data.sql.SqlEntities;
using Microsoft.EntityFrameworkCore;

namespace chldr_data.sql.Repositories
{
    public class SqlSourcesRepository : SqlRepository<SqlSource, SourceModel, SourceDto>, ISourcesRepository
    {
        protected override RecordType RecordType => RecordType.Source;
        public SqlSourcesRepository(SqlContext context, IFileService fileService, string _userId) : base(context, fileService, _userId) { }
        public override async Task<List<SourceModel>> GetRandomsAsync(int limit)
        {
            var randomizer = new Random();
            var ids = await _dbContext.Set<SqlSource>().Select(e => e.SourceId).ToListAsync();
            var randomlySelectedIds = ids.OrderBy(x => randomizer.Next(1, Constants.EntriesApproximateCoount)).Take(limit).ToList();

            var entities = await _dbContext.Set<SqlSource>()
              .Where(e => randomlySelectedIds.Contains(e.SourceId))
              .AsNoTracking()
              .ToListAsync();

            var models = entities.Select(SourceModel.FromEntity).ToList();
            return models;
        }
        public override async Task<List<ChangeSetModel>> UpdateAsync(SourceDto dto)
        {
            var source = SqlSource.FromDto(dto);
            _dbContext.Update(source);

            var changeSet = CreateChangeSetEntity(Operation.Update, dto.SourceId);
            _dbContext.ChangeSets.Add(changeSet);

            _dbContext.SaveChanges();
            return new List<ChangeSetModel> { ChangeSetModel.FromEntity(changeSet) };
        }

        public override async Task<List<ChangeSetModel>> AddAsync(SourceDto dto)
        {
            var source = SqlSource.FromDto(dto);
            _dbContext.Add(source);

            var changeSet = CreateChangeSetEntity(Operation.Insert, dto.SourceId);
            _dbContext.ChangeSets.Add(changeSet);

            _dbContext.SaveChanges();
            return new List<ChangeSetModel> { ChangeSetModel.FromEntity(changeSet) };
        }

        public override async Task<SourceModel> GetAsync(string entityId)
        {
            var token = await _dbContext.Sources.FirstOrDefaultAsync(t => t.SourceId!.Equals(entityId));
            if (token == null)
            {
                throw new NullReferenceException();
            }

            return SourceModel.FromEntity(token);
        }

        public override async Task<List<SourceModel>> TakeAsync(int offset, int limit)
        {
            var entities = await _dbContext.Sources
                 .Skip(offset)
                 .Take(limit)
                 .ToListAsync();

            return entities.Select(SourceModel.FromEntity).ToList();
        }
    }
}
