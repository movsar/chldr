using chldr_data.Enums;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using MongoDB.Bson;

namespace chldr_data.DatabaseObjects.Dtos
{
    public class PhraseDto : EntryDto
    {
        public override string Content { get; set; } = string.Empty;
        public string? PhraseId { get; set; }
        public string? Notes { get; set; } = string.Empty;

        public static PhraseDto FromModel(PhraseModel phrase)
        {
            var phraseDto = new PhraseDto()
            {
                EntryId = phrase.EntryId,
                SourceId = phrase.Source.SourceId,
                Rate = phrase.Rate,
                EntryType = (EntryType)phrase.Type,
                CreatedAt = phrase.CreatedAt,
                UpdatedAt = phrase.UpdatedAt,

                PhraseId = phrase.PhraseId,
                Content = phrase.Content,
                Notes = phrase.Notes,
            };

            phraseDto.Translations.AddRange(phrase.Translations.Select(t => TranslationDto.FromModel(t)));
            return phraseDto;
        }
    }
}
