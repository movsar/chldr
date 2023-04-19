using chldr_data.Interfaces;
using Realms;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace chldr_data.Entities;
[Table("Image")]
public partial class Image : RealmObject, IEntity
{
    [Realms.PrimaryKey]
    public string ImageId { get; set; } = Guid.NewGuid().ToString();
    public string? UserId { get; set; }
    public string EntryId { get; set; } = null!;
    public string? FileName { get; set; }
    public int Rate { get; set; }
    public DateTimeOffset? CreatedAt { get; set; } 
    public DateTimeOffset? UpdatedAt { get; set; }
    [Ignored] public virtual Entry Entry { get; set; } = null!;
    [Ignored] public virtual User? User { get; set; }
}
