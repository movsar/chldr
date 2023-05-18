using chldr_data.Dto;
using chldr_data.Enums;

namespace chldr_data.Interfaces.DatabaseEntities
{
    public interface IEntryDto : IEntry, IDto
    {
        EntryType EntryType { get; set; }
        List<TranslationDto> Translations { get; set; }
    }
}
