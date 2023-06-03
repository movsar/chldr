﻿using System.ComponentModel.DataAnnotations.Schema;
using chldr_data.DatabaseObjects.Interfaces;

namespace chldr_data.DatabaseObjects.SqlEntities;
[Table("Entry")]
public class SqlEntry : IEntryEntity
{
    public string EntryId { get; set; }
    public string UserId { get; set; } = null!;
    public string SourceId { get; set; } = null!;
    public int Type { get; set; } = 0;
    public int Rate { get; set; } = 0;
    public string RawContents { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTime.Now;
    public DateTimeOffset UpdatedAt { get; set; } = DateTime.Now;
    public virtual ICollection<SqlSound> Sounds { get; set; } = new List<SqlSound>();
    public virtual SqlSource Source { get; set; } = null!;
    public virtual ICollection<SqlTranslation> Translations { get; set; } = new List<SqlTranslation>();
    public virtual SqlUser User { get; set; } = null!;
    public virtual SqlText? Text { get; set; }
    public virtual SqlPhrase? Phrase { get; set; }
    public virtual SqlWord? Word { get; set; }

}