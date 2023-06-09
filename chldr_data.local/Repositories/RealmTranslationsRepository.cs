using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_tools;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using chldr_data.Enums;
using chldr_data.Services;
using chldr_data.local.RealmEntities;
using Realms;
using chldr_data.Interfaces.Repositories;
using chldr_utils.Interfaces;
using chldr_utils;
using chldr_data.Models;
using Realms.Sync;

namespace chldr_data.Repositories
{
    public class RealmTranslationsRepository : RealmRepository<RealmTranslation, TranslationModel, TranslationDto>, ITranslationsRepository
    {
        public RealmTranslationsRepository(Realm context, ExceptionHandler exceptionHandler, IGraphQLRequestSender graphQLRequestSender) : base(context, exceptionHandler, graphQLRequestSender) { }

        protected override RecordType RecordType => RecordType.Translation;

        public override async Task Insert(string userId, TranslationDto dto)
        {
            throw new NotImplementedException();
        }

        public override Task Delete(string userId, string entityId)
        {
            throw new NotImplementedException();
        }

        public override TranslationModel Get(string entityId)
        {
            var translation = _dbContext.All<RealmTranslation>()
                .FirstOrDefault(t => t.TranslationId.Equals(entityId));

            if (translation == null)
            {
                throw new ArgumentException($"Entity not found: {entityId}");
            }

            return TranslationModel.FromEntity(translation, translation.Language);
        }

        public override async Task Update(string userId, TranslationDto dto)
        {
            // TODO: Update remote
            // UpdateLocal(userId, dto);
        }

        // Updates local entity
        internal void Update(TranslationDto translationDto)
        {
            // Find out what has been changed
            var existing = Get(translationDto.TranslationId);
            var existingDto = TranslationDto.FromModel(existing);
            var changes = Change.GetChanges(translationDto, existingDto);
            if (changes.Count == 0)
            {
                return;
            }

            ApplyChanges<RealmTranslation>(translationDto.TranslationId, changes);
        }

        // Deletes local entity
        internal void Delete(string translationId)
        {
            var translation = _dbContext.Find<RealmTranslation>(translationId);
            _dbContext.Remove(translation);
        }

        // Inserts local entity
        internal void Insert(TranslationDto translationDto)
        {
            var language = _dbContext.Find<RealmLanguage>(translationDto.LanguageId);
            var user = _dbContext.Find<RealmUser>(translationDto.UserId);
            var entry = _dbContext.Find<RealmEntry>(translationDto.EntryId);
            var translation = RealmTranslation.FromDto(translationDto, entry, user, language) as RealmTranslation;
            _dbContext.Add(translation);
        }
    }
}
