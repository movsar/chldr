using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models.Words;
using chldr_data.Enums;
using chldr_data.Interfaces;
using Newtonsoft.Json;

namespace chldr_data.DatabaseObjects.Models
{
    public class EntryModel : IEntry
    {
        public string EntryId { get; internal set; }
        public string UserId { get; set; }
        public string? ParentEntryId { get; set; }
        public int Rate { get; set; }
        public EntryType Type { get; set; }
        public int Subtype { get; set; }
        public string Content { get; set; }
        public IDetails? Details { get; set; }
        public SourceModel Source { get; set; }
        public string? SourceId => Source.SourceId;
        public List<TranslationModel> Translations { get; set; } = new List<TranslationModel>();
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }

        public static EntryModel FromEntity(IEntryEntity entry, ISourceEntity source, IEnumerable<ITranslationEntity> translations)
        {
            var entryModel = new EntryModel()
            {
                EntryId = entry.EntryId,
                UserId = entry.UserId,
                ParentEntryId = entry.ParentEntryId,
                Rate = entry.Rate,
                Type = (EntryType)entry.Type,
                Subtype = entry.Subtype,
                Source = SourceModel.FromEntity(source),
                Content = entry.Content,
                CreatedAt = entry.CreatedAt,
                UpdatedAt = entry.UpdatedAt
            };

            if (!string.IsNullOrEmpty(entry.Details))
            {
                switch (entryModel.Type)
                {
                    case EntryType.Word:
                        entryModel.Details = JsonConvert.DeserializeObject<WordDetailsDto>(entry.Details)!;
                        break;
                }
            }

            foreach (var translation in translations)
            {
                entryModel.Translations.Add(TranslationModel.FromEntity(translation));
            }

            return entryModel;
        }
    }
}
