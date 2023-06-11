using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.Models.Words;
using chldr_data.Enums;
using System.Xml;

namespace chldr_data.DatabaseObjects.Dtos
{
    public abstract class EntryDto : IEntry
    {
        public string EntryId { get; set; } = Guid.NewGuid().ToString();
        public string? UserId { get; set; }
        public string SourceId { get; set; }
        public string? ParentEntryId { get; set; }
        public int Rate { get; set; }
        public abstract string Content { get; set; }
        public List<TranslationDto> Translations { get; set; } = new List<TranslationDto>();
        public int EntryType { get; set; } = 1;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        protected void SetEntryFields(EntryModel entry)
        {
            EntryId = entry.EntryId;
            UserId = entry.UserId;
            SourceId = entry.SourceId!;
            ParentEntryId = entry.ParentEntryId;
            EntryType = entry.Type;
            Rate = entry.Rate;
            CreatedAt = entry.CreatedAt;
            UpdatedAt = entry.UpdatedAt;

            Translations.Clear();
            Translations.AddRange(entry.Translations.Select(t => TranslationDto.FromModel(t)));
        }
    }
}
