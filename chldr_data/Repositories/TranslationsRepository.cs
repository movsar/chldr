using chldr_data.Interfaces;
using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.RealmEntities;
using MongoDB.Bson;
using chldr_data.DatabaseObjects.SqlEntities;
using chldr_data.DatabaseObjects.Models;
using chldr_tools;

namespace chldr_data.Repositories
{
    public class TranslationsRepository : Repository<SqlTranslation, TranslationModel, TranslationDto>
    {
        public TranslationsRepository(SqlContext context) : base(context) { }

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
