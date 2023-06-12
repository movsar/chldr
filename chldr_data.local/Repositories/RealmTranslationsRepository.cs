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
     

        // Updates local entity
        public override void Update(TranslationDto translationDto)
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
        public override void Delete(string translationId)
        {
            var translation = _dbContext.Find<RealmTranslation>(translationId);
            _dbContext.Remove(translation);
        }

        // Inserts local entity
        public override void Insert(TranslationDto translationDto)
        {
            //var translation = RealmTranslation.FromDto(translationDto,  _dbContext);
            //_dbContext.Add(translation);
        }
    }
}
