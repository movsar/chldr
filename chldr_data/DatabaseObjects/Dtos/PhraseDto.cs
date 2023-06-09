using chldr_data.Enums;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using MongoDB.Bson;

namespace chldr_data.DatabaseObjects.Dtos
{
    public class PhraseDto : EntryDto
    {
        public string PhraseId { get; set; } = Guid.NewGuid().ToString();
        public override string Content { get; set; } = string.Empty;
        public static PhraseDto FromModel(PhraseModel phrase)
        {
            var phraseDto = new PhraseDto()
            {
                EntryId = phrase.EntryId,
                SourceId = phrase.Source.SourceId,
                CreatedAt = phrase.CreatedAt,
                UpdatedAt = phrase.UpdatedAt,
                EntryType = (EntryType)phrase.Type,

                Rate = phrase.Rate,
                PhraseId = phrase.PhraseId,
                Content = phrase.Content,
            };

            phraseDto.Translations.AddRange(phrase.Translations.Select(t => TranslationDto.FromModel(t)));
            return phraseDto;
        }
    }
}
