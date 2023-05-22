﻿using chldr_data.Entities;
using chldr_data.Interfaces;
using chldr_data.Interfaces.DatabaseEntities;

namespace chldr_data.Models
{
    public class PhraseModel : EntryModel, IPhrase
    {
        public string PhraseId { get; set; }
        public override string Content { get; set; }
        public string? Notes { get; set; }
        public override DateTimeOffset CreatedAt { get; set; }
        public override DateTimeOffset UpdatedAt { get; set; }

        private static PhraseModel FromEntity(IEntryEntity entryEntity, IPhraseEntity phraseEntity, ISourceEntity sourceEntity, IEnumerable<KeyValuePair<ILanguageEntity, ITranslationEntity>> translationEntityInfos)
        {
            var phraseModel = new PhraseModel()
            {
                EntryId = entryEntity.EntryId,
                Rate = entryEntity.Rate,
                Type = entryEntity.Type,
                Source = SourceModel.FromEntity(sourceEntity),
                CreatedAt = entryEntity.CreatedAt, 
                UpdatedAt = entryEntity.UpdatedAt,

                PhraseId = phraseEntity.PhraseId,
                Content = phraseEntity.Content,
                Notes = phraseEntity.Notes,
            };

            foreach (var translationEntityToLanguage in translationEntityInfos)
            {
                phraseModel.Translations.Add(TranslationModel.FromEntity(translationEntityToLanguage.Value, translationEntityToLanguage.Key));
            }

            return phraseModel;
        }

        public static PhraseModel FromEntity(SqlPhrase wordEntity)
        {
            return FromEntity(wordEntity.Entry,
                wordEntity.Entry.Phrase,
                wordEntity.Entry.Source,
                wordEntity.Entry.Translations.Select(
                    t => new KeyValuePair<ILanguageEntity, ITranslationEntity>(t.Language, t)
                ));
        }

        public static PhraseModel FromEntity(RealmPhrase wordEntity)
        {
            return FromEntity(wordEntity.Entry,
                 wordEntity.Entry.Phrase,
                 wordEntity.Entry.Source,
                 wordEntity.Entry.Translations.Select(
                     t => new KeyValuePair<ILanguageEntity, ITranslationEntity>(t.Language, t)
                 ));
        }
    }
}
