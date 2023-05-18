using chldr_data.Dto;
using chldr_data.Interfaces.DatabaseEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace chldr_data.Entities;

[Table("Word")]
public class SqlWord : IWordEntity
{
    public string WordId { get; set; } = Guid.NewGuid().ToString();
    public string EntryId { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string? Notes { get; set; }
    public int? PartOfSpeech { get; set; }
    public string? AdditionalDetails { get; set; }
    public virtual SqlEntry Entry { get; set; } = null!;

    public SqlWord(WordDto wordDto)
    {
        // Update properties from the WordDto object
        EntryId = wordDto.EntryId;
        Content = wordDto.Content;
        Notes = wordDto.Notes;
        PartOfSpeech = (int)wordDto.PartOfSpeech;
        //AdditionalDetails = wordDto;
    }

}
