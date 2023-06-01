﻿using chldr_data.Interfaces;
using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.RealmEntities;
using MongoDB.Bson;

namespace chldr_data.Repositories
{
    public class TranslationsRepository : Repository
    {
        public TranslationsRepository(ILocalDbReader dataAccess) : base(dataAccess) { }

        internal void SetPropertiesFromDto(RealmTranslation translation, TranslationDto translationDto)
        {
            translation.Entry = Database.All<RealmEntry>().First(e => e.EntryId == translationDto.EntryId);
            translation.Language = Database.All<RealmLanguage>().First(l => l.Code == translationDto.LanguageCode);
            translation.Rate = translationDto.Rate;
            translation.Content = translationDto.Content;
            translation.Notes = translationDto.Notes;
            translation.RawContents = translation.GetRawContents();
        }
    }
}
