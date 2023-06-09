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
        public int EntryType { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public static EntryDto FromModel(EntryModel entryModel)
        {
            EntryDto entryDto = null!;

            // Word / Phrase / Text
            switch ((EntryType)entryModel.Type)
            {
                case Enums.EntryType.Word:
                    entryDto = new WordDto()
                    {
                        WordId = ((WordModel)entryModel).WordId,
                        PartOfSpeech = ((WordModel)entryModel).PartOfSpeech,
                        //AdditionalDetails =
                    };
                    break;

                case Enums.EntryType.Phrase:
                    entryDto = new PhraseDto()
                    {
                        PhraseId = ((PhraseModel)entryModel).PhraseId,
                    };
                    break;

                case Enums.EntryType.Text:
                    entryDto = new TextDto()
                    {
                        TextId = ((TextModel)entryModel).TextId,
                    };
                    break;

            }

            // Shared fields
            entryDto.Content = entryModel.Content;

            // Entry
            entryDto.EntryId = entryModel.EntryId;
            entryDto.UserId = entryModel.UserId;
            entryDto.SourceId = entryModel.SourceId!;
            entryDto.ParentEntryId = entryModel.ParentEntryId;
            entryDto.EntryType = entryModel.Type;
            entryDto.Rate = entryModel.Rate;
            entryDto.CreatedAt = entryModel.CreatedAt;
            entryDto.UpdatedAt = entryModel.UpdatedAt;


            // Translations
            entryDto.Translations.AddRange(entryModel.Translations.Select(t => TranslationDto.FromModel(t)));
            return entryDto;
        }
    }
}
