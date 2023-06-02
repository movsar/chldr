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

    public SqlWord(WordDto wordDto)
    {
        // Update properties from the WordDto object
        EntryId = wordDto.EntryId;
        Content = wordDto.Content;
        Notes = wordDto.Notes;
        PartOfSpeech = (int)wordDto.PartOfSpeech;
        //AdditionalDetails = wordDto;
    }

    public static SqlWord FromDto(WordDto dto)
    {
        throw new NotImplementedException();
    }
}
