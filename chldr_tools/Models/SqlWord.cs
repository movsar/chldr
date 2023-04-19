using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace chldr_tools.Models;
[Table("Word")]

public partial class SqlWord
{
    public string WordId { get; set; } = null!;

    public string EntryId { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string? Notes { get; set; }

    public int? PartOfSpeech { get; set; }

    public string? AdditionalDetails { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual SqlEntry Entry { get; set; } = null!;
}
