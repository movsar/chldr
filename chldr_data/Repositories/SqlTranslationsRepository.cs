﻿using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.SqlEntities;
using chldr_data.DatabaseObjects.Models;
using chldr_tools;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using chldr_data.Enums;
using chldr_data.Services;
using chldr_data.Interfaces.Repositories;

namespace chldr_data.Repositories
{
    public class SqlTranslationsRepository : SqlRepository<SqlTranslation, TranslationModel, TranslationDto>, ITranslationsRepository
    {
        public SqlTranslationsRepository(SqlContext context) : base(context) { }

        protected override RecordType RecordType => RecordType.Translation;

        public override async Task Add(string userId, TranslationDto dto)
        {
            var entity = SqlTranslation.FromDto(dto);
            SqlContext.Add(entity);

            // Insert changeset
            var changeSet = new SqlChangeSet()
            {
                UserId = userId!,
                Operation = (int)Operation.Insert,
                RecordId = dto.TranslationId!,
                RecordType = (int)RecordType,
            };
            SqlContext.ChangeSets.Add(changeSet);
            SqlContext.SaveChanges();
        }


        public override TranslationModel Get(string entityId)
        {
            var translation = SqlContext.Translations
                .Include(translation => translation.Language)
                .FirstOrDefault(t => t.TranslationId.Equals(entityId));

            if (translation == null)
            {
                throw new ArgumentException($"Entity not found: {entityId}");
            }

            return TranslationModel.FromEntity(translation, translation.Language);
        }

        public override async Task Update(string userId, TranslationDto translationDto)
        {
            // Find out what has been changed
            var existing = Get(translationDto.TranslationId);
            var existingDto = TranslationDto.FromModel(existing);
            var changes = SqlUnitOfWork.GetChanges(translationDto, existingDto);
            if (changes.Count == 0)
            {
                return ;
            }

            ApplyChanges(translationDto.TranslationId, changes);

            // Insert changeset
            var changeSet = new SqlChangeSet()
            {
                UserId = userId!,
                Operation = (int)Operation.Update,
                RecordId = translationDto.TranslationId!,
                RecordType = (int)RecordType,
                RecordChanges = JsonConvert.SerializeObject(changes)
            };
            SqlContext.ChangeSets.Add(changeSet);
            SqlContext.SaveChanges();
        }
    }
}
