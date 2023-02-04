using chldr_data.Enums;
using chldr_data.Models;
using MongoDB.Bson;

namespace chldr_data.Dto
{
    public abstract class EntryDto
    {
        public ObjectId? EntryId { get; set; }
        public ObjectId? SourceId { get; set; }
        public List<TranslationDto> Translations { get; } = new List<TranslationDto>();
        public int Rate { get; set; }
        public EntryType EntryType { get; set; }
        public EntryDto() { }
        public EntryDto(EntryModel entry)
        {
            EntryId = entry.Id;
            SourceId = entry.Source.Id;
            Rate = entry.Rate;
            EntryType = (EntryType)entry.Type;
            foreach (var translation in entry.Translations)
            {
                Translations.Add(new TranslationDto(translation));
            }
        }
    }
}
