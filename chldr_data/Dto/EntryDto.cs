using chldr_data.Entities;
using chldr_data.Enums;
using chldr_data.Models;
using MongoDB.Bson;

namespace chldr_data.Dto
{
    public abstract class EntryDto
    {
        public string? EntryId { get; set; }
        public string? SourceId { get; set; }
        public List<TranslationDto> Translations { get; } = new List<TranslationDto>();
        public int Rate { get; set; }
        public EntryType EntryType { get; set; }
        public EntryDto() { }
        public EntryDto(SqlEntry entry) {
            EntryId = entry.EntryId;
            SourceId = entry.Source.SourceId;
            Rate = entry.Rate;
            EntryType = (EntryType)entry.Type;
            foreach (var translation in entry.Translations)
            {
                Translations.Add(new TranslationDto(translation));
            }
        }

        public EntryDto(EntryModel entry)
        {
            EntryId = entry.EntryId;
            SourceId = entry.Source.SourceId;
            Rate = entry.Rate;
            EntryType = (EntryType)entry.Type;
            foreach (var translation in entry.Translations)
            {
                Translations.Add(new TranslationDto(translation));
            }
        }
    }
}
