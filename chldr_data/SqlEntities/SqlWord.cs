using chldr_data.Interfaces.DatabaseEntities;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace chldr_data.Entities;

[Table("Word")]
public class SqlWord
{
    public string WordId { get; set; } = Guid.NewGuid().ToString();
    public string EntryId { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string? Notes { get; set; }
    public int? PartOfSpeech { get; set; }
    public string? AdditionalDetails { get; set; }
    public virtual SqlEntry Entry { get; set; } = null!;
}
