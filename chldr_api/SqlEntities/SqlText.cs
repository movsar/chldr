using chldr_data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Realms;
using System.ComponentModel.DataAnnotations.Schema;

namespace chldr_data.Entities;
[Table("Text")]

public partial class SqlText : RealmObject, IEntity
{
    [Realms.PrimaryKey]
    public string TextId { get; set; } = Guid.NewGuid().ToString();
    public string EntryId { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string? Notes { get; set; }
    [Ignored] public virtual SqlEntry Entry { get; set; } = null!;
}
