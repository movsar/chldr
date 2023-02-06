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
        public EntryDto(EntryModel entry)
        {
            EntryId = entry.Id.ToString();
            SourceId = entry.Source.Id.ToString();
            Rate = entry.Rate;
            EntryType = (EntryType)entry.Type;
            foreach (var translation in entry.Translations)
            {
                Translations.Add(new TranslationDto(translation));
            }
        }
    }
}
