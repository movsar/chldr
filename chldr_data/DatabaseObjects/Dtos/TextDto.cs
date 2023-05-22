using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;

namespace chldr_data.DatabaseObjects.Dtos
{
    public class TextDto : EntryDto, IText
    {
        public string TextId { get; set; }
        public string Content { get; set; }
        public static TextDto FromModel(TextModel model)
        {
            var phraseDto = new TextDto()
            {
                EntryId = model.EntryId,
                SourceId = model.Source.SourceId,
                Rate = model.Rate,
                EntryType = (EntryType)model.Type,
                CreatedAt = model.CreatedAt,    
                UpdatedAt   = model.UpdatedAt,

                TextId = model.TextId,
                Content = model.Content,
            };

            phraseDto.Translations.AddRange(model.Translations.Select(t => TranslationDto.FromModel(t)));
            return phraseDto;
        }
    }
}
