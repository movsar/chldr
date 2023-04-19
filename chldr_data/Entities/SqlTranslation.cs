using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace chldr_tools.Models;
[Table("Translation")]

public partial class SqlTranslation
{
    public string TranslationId { get; set; } = null!;

    public string LanguageId { get; set; } = null!;

    public string EntryId { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string RawContents { get; set; } = null!;

    public string? Notes { get; set; }

    public int? Rate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual SqlEntry Entry { get; set; } = null!;

    public virtual SqlLanguage Language { get; set; } = null!;

    public virtual SqlUser User { get; set; } = null!;
}
