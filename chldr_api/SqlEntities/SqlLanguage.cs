using chldr_data.Interfaces;
using Realms;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace chldr_data.Entities;
[Table("Language")]
public partial class SqlLanguage : RealmObject, IEntity
{
    [Realms.PrimaryKey]
    public string LanguageId { get; set; } = Guid.NewGuid().ToString();
    public string? UserId { get; set; }
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; } = DateTime.Now;
    public DateTimeOffset UpdatedAt { get; set; } = DateTime.Now;
    [Ignored] public virtual ICollection<SqlTranslation> Translations { get; set; } = new List<SqlTranslation>();
    [Ignored] public virtual SqlUser? User { get; set; }
}
