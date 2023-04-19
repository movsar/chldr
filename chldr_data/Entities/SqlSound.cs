using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace chldr_tools.Models;
[Table("Sound")]

public partial class SqlSound
{
    public string SoundId { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public string EntryId { get; set; } = null!;

    public string FileName { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual SqlEntry Entry { get; set; } = null!;

    public virtual SqlUser User { get; set; } = null!;
}
