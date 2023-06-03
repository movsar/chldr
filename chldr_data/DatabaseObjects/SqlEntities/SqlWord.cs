using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Dtos;
using System.ComponentModel.DataAnnotations.Schema;

namespace chldr_data.DatabaseObjects.SqlEntities;

[Table("Word")]
public class SqlWord : IWordEntity
{
    public string WordId { get; set; }
    public string EntryId { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string? Notes { get; set; }
    public int? PartOfSpeech { get; set; }
    public string? AdditionalDetails { get; set; }
    public virtual SqlEntry Entry { get; set; } = null!;
    public SqlWord() { }

    public static SqlWord FromDto(WordDto dto)
    {
        return new SqlWord()
        {
            // Update properties from the WordDto object
            EntryId = dto.EntryId,
            Content = dto.Content,
            Notes = dto.Notes,
            PartOfSpeech = (int)dto.PartOfSpeech,
            //AdditionalDetails = wordDto;
        };
    }
}
