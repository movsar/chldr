using chldr_data.Interfaces;
using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.RealmEntities;
using MongoDB.Bson;
using chldr_data.DatabaseObjects.SqlEntities;
using chldr_data.DatabaseObjects.Models;
using chldr_tools;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models.Words;
using Microsoft.EntityFrameworkCore;

namespace chldr_data.Repositories
{
    public class TranslationsRepository : Repository<SqlTranslation, TranslationModel, TranslationDto>, ITranslationsRepository
    {
        public TranslationsRepository(SqlContext context) : base(context) { }

        public override void Add(TranslationDto dto)
        {
            var entity = SqlTranslation.FromDto(dto);
            SqlContext.Add(entity);
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

        public override void Update(TranslationDto dto)
        {
            var translation = SqlContext.Translations
             .Include(translation => translation.Language)
             .FirstOrDefault(t => t.TranslationId.Equals(dto.TranslationId));

            if (translation == null)
            {
                throw new ArgumentException($"Entity not found: {dto.TranslationId}");
            }

            throw new NotImplementedException();
        }

        internal void SetPropertiesFromDto(RealmTranslation translation, TranslationDto translationDto)
        {
            //translation.Entry = Database.All<RealmEntry>().First(e => e.EntryId == translationDto.EntryId);
            //translation.Language = Database.All<RealmLanguage>().First(l => l.Code == translationDto.LanguageCode);
            translation.Rate = translationDto.Rate;
            translation.Content = translationDto.Content;
            translation.Notes = translationDto.Notes;
            translation.RawContents = translation.GetRawContents();
        }
    }
}
