using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace chldr_tools.Models;
[Table("Source")]

public partial class SqlSource
{
    public string SourceId { get; set; } = null!;

    public string? UserId { get; set; }

    public string Name { get; set; } = null!;

    public string? Notes { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? Sourcecol { get; set; }

    public virtual ICollection<SqlEntry> Entries { get; set; } = new List<SqlEntry>();

    public virtual SqlUser? User { get; set; }
}
