using chldr_data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Realms;
using System.ComponentModel.DataAnnotations.Schema;

namespace chldr_data.Entities;
[Table("Text")]

public partial class Text : RealmObject, IEntity
{
    [Realms.PrimaryKey]
    public string TextId { get; set; } = Guid.NewGuid().ToString();
    public string EntryId { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string? Notes { get; set; }
    [Ignored] public virtual Entry Entry { get; set; } = null!;
}
