using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using Newtonsoft.Json;

namespace chldr_data.DatabaseObjects.Dtos
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
                if (trimmedValue.Contains(" ") || trimmedValue.Contains(".") || trimmedValue.Contains(","))
                {
                    if (trimmedValue.Length > 255)
                    {
                        Type = (int)EntryType.Text;
                    }
                    else
                    {
                        Type = (int)EntryType.Phrase;
                    }
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
        public int EntrySubtype { get; set; } = 0;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public virtual List<TranslationDto> TranslationsDtos { get; set; } = new List<TranslationDto>();
        public virtual List<SoundDto> SoundDtos { get; set; } = new List<SoundDto>();
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

            entryDto.TranslationsDtos.Clear();
            entryDto.TranslationsDtos.AddRange(entryModel.Translations.Select(t => TranslationDto.FromModel(t)));

            entryDto.SoundDtos.Clear();
            entryDto.SoundDtos.AddRange(entryModel.Sounds.Select(s => SoundDto.FromModel(s)));
            return entryDto;
        }
    }
}
