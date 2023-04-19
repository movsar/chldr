using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace chldr_tools.Models;

[Table("Activity")]
public partial class SqlActivity
{
    public string ActivityId { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public string ObjectId { get; set; } = null!;

    public string ObjectClass { get; set; } = null!;

    public string ObjectProperty { get; set; } = null!;

    public string OldValue { get; set; } = null!;

    public string NewValue { get; set; } = null!;

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual SqlUser User { get; set; } = null!;
}
