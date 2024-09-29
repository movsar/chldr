using domain.DatabaseObjects.Interfaces;
using domain.DatabaseObjects.Models;
using domain;
using Newtonsoft.Json;
using domain.Enums;

namespace domain.DatabaseObjects.Dtos
{
    public class EntryDto : IEntry
    {
        private string content = string.Empty;

        public string EntryId { get; set; } = Guid.NewGuid().ToString();
        public string? UserId { get; set; }
        public string SourceId { get; set; }
        public string? ParentEntryId { get; set; }
        public int Rate { get; set; }
        public string Content
        {
            get => content;

            set
            {
                var trimmedValue = value.Trim();
                if (trimmedValue.Contains(' ') || trimmedValue.Contains('.') || trimmedValue.Contains(','))
                {
                    Type = (int)EntryType.Text;
                }
                else
                {
                    Type = (int)EntryType.Word;
                }
                content = value;
            }
        }
        public string? Details { get; set; }
        public int Type { get; set; } = 1;
        public int Subtype { get; set; } = 0;
        public List<EntryDto> SubEntries { get; set; } = new List<EntryDto>();
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public virtual List<TranslationDto> Translations { get; set; } = new List<TranslationDto>();
        public virtual List<SoundDto> Sounds { get; set; } = new List<SoundDto>();
        public bool ExistsInDatabase()
        {
            return CreatedAt == DateTimeOffset.MinValue;
        }

        public static EntryDto FromModel(EntryModel entryModel)
        {
            var entryDto = new EntryDto()
            {
                EntryId = entryModel.EntryId,
                UserId = entryModel.UserId,
                ParentEntryId = entryModel.ParentEntryId,
                SubEntries = entryModel.SubEntries.Select(FromModel).ToList(),
                Type = (int)entryModel.Type,
                Subtype = entryModel.Subtype,
                Rate = entryModel.Rate,
                Content = entryModel.Content,
                Details = (entryModel.Details == null) ? null : JsonConvert.SerializeObject(entryModel.Details),
                CreatedAt = entryModel.CreatedAt,
                UpdatedAt = entryModel.UpdatedAt,
            };

            entryDto.Translations.Clear();
            entryDto.Translations.AddRange(entryModel.Translations.Select(t => TranslationDto.FromModel(t)));

            entryDto.Sounds.Clear();
            entryDto.Sounds.AddRange(entryModel.Sounds.Select(s => SoundDto.FromModel(s)));
            return entryDto;
        }
    }
}
