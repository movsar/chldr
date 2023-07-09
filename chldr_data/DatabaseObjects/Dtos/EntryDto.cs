using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using Newtonsoft.Json;

namespace chldr_data.DatabaseObjects.Dtos
{
    public class EntryDto : IEntry
    {
        public string EntryId { get; set; } = Guid.NewGuid().ToString();
        public string? UserId { get; set; }
        public string SourceId { get; set; }
        public string? ParentEntryId { get; set; }
        public int Rate { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? Details { get; set; }
        public int Type { get; set; } = 1;
        public int EntrySubtype { get; set; } = 0;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public virtual List<TranslationDto> Translations { get; set; } = new List<TranslationDto>();
        public virtual List<SoundDto> Sounds { get; set; } = new List<SoundDto>();
        public static EntryDto FromModel(EntryModel entryModel)
        {
            var entryDto = new EntryDto()
            {
                EntryId = entryModel.EntryId,
                UserId = entryModel.UserId,
                SourceId = entryModel.SourceId!,
                ParentEntryId = entryModel.ParentEntryId,
                Type = (int)entryModel.Type,
                EntrySubtype = entryModel.Subtype,
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
