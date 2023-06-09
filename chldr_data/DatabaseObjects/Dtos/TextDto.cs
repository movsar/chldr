using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.Models.Words;
using chldr_data.Enums;

namespace chldr_data.DatabaseObjects.Dtos
{
    public class TextDto : EntryDto, IText
    {
        public string TextId { get; set; } = Guid.NewGuid().ToString();
        public override string Content { get; set; }
        public static TextDto FromModel(TextModel model)
        {
            var textDto = new TextDto()
            {
                EntryId = model.EntryId,
                SourceId = model.SourceId!,
                EntryType = model.Type,
                CreatedAt = model.CreatedAt,
                UpdatedAt = model.UpdatedAt,

                TextId = model.TextId,
                Content = model.Content,
                Rate = model.Rate,
            };

            textDto.Translations.AddRange(model.Translations.Select(t => TranslationDto.FromModel(t)));
            return textDto;
        }
    }
}
