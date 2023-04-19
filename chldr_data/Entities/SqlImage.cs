using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace chldr_tools.Models;
[Table("Image")]
public partial class SqlImage
{
    public string ImageId { get; set; } = null!;

    public string? UserId { get; set; }

    public string EntryId { get; set; } = null!;

    public string? FileName { get; set; }

    public int Rate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual SqlEntry Entry { get; set; } = null!;

    public virtual SqlUser? User { get; set; }
}
