using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Dtos;
using System.ComponentModel.DataAnnotations.Schema;

namespace chldr_data.remote.SqlEntities;

[Table("Word")]
public class SqlWord : IWordEntity
{
    public string WordId { get; set; }
    public string EntryId { get; set; } = null!;
    public string Content { get; set; } = null!;
    public int? PartOfSpeech { get; set; }
    public string? AdditionalDetails { get; set; }
    public virtual SqlEntry Entry { get; set; } = null!;
    public SqlWord() { }

    public static SqlWord FromDto(WordDto dto)
    {
        return new SqlWord()
        {
            WordId = dto.WordId,
            EntryId = dto.EntryId,
            Content = dto.Content,
            PartOfSpeech = (int)dto.PartOfSpeech,
            //AdditionalDetails = wordDto;
        };
    }
}
