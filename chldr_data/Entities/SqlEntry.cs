using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace chldr_tools.Models;
[Table("Entry")]
public partial class SqlEntry
{
    public string EntryId { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public string SourceId { get; set; } = null!;

    public int? Type { get; set; }

    public int Rate { get; set; }

    public string? RawContents { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<SqlImage> Images { get; set; } = new List<SqlImage>();

    public virtual ICollection<SqlPhrase> Phrases { get; set; } = new List<SqlPhrase>();

    public virtual ICollection<SqlSound> Sounds { get; set; } = new List<SqlSound>();

    public virtual SqlSource Source { get; set; } = null!;

    public virtual ICollection<SqlText> Texts { get; set; } = new List<SqlText>();

    public virtual ICollection<SqlTranslation> Translations { get; set; } = new List<SqlTranslation>();

    public virtual SqlUser User { get; set; } = null!;

    public virtual ICollection<SqlWord> Words { get; set; } = new List<SqlWord>();
}
