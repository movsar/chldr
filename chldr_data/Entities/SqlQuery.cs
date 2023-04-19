using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace chldr_tools.Models;
[Table("Query")]

public partial class SqlQuery
{
    public string QueryId { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public string Content { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual SqlUser User { get; set; } = null!;
}
