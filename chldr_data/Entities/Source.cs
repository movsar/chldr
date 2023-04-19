using chldr_data.Interfaces;
using Realms;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace chldr_data.Entities;
[Table("Source")]

public partial class Source : RealmObject, IEntity
{
    [Realms.PrimaryKey]
    public string SourceId { get; set; } = Guid.NewGuid().ToString();
    public string? UserId { get; set; }
    public string Name { get; set; } = null!;
    public string? Notes { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTime.Now;
    public DateTimeOffset UpdatedAt { get; set; } = DateTime.Now;
    [Ignored] public virtual ICollection<Entry> Entries { get; set; } = new List<Entry>();
    [Ignored] public virtual User? User { get; set; }
}
