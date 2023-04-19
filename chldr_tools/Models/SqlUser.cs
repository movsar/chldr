using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace chldr_tools.Models;
[Table("User")]

public partial class SqlUser
{
    public string UserId { get; set; } = null!;

    public string? Email { get; set; }

    public string? Password { get; set; }

    public int? Rate { get; set; }

    public string? ImagePath { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Patronymic { get; set; }

    public sbyte? IsModerator { get; set; }

    public sbyte? AccountStatus { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public virtual ICollection<SqlActivity> Activities { get; set; } = new List<SqlActivity>();

    public virtual ICollection<SqlEntry> Entries { get; set; } = new List<SqlEntry>();

    public virtual ICollection<SqlImage> Images { get; set; } = new List<SqlImage>();

    public virtual ICollection<SqlLanguage> Languages { get; set; } = new List<SqlLanguage>();

    public virtual ICollection<SqlQuery> Queries { get; set; } = new List<SqlQuery>();

    public virtual ICollection<SqlSound> Sounds { get; set; } = new List<SqlSound>();

    public virtual ICollection<SqlSource> Sources { get; set; } = new List<SqlSource>();

    public virtual ICollection<SqlTranslation> Translations { get; set; } = new List<SqlTranslation>();
}
