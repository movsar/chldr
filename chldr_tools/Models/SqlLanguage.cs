using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace chldr_tools.Models;
[Table("Language")]
public partial class SqlLanguage
{
    public string LanguageId { get; set; } = null!;

    public string? UserId { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<SqlTranslation> Translations { get; set; } = new List<SqlTranslation>();

    public virtual SqlUser? User { get; set; }
}
