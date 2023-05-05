using chldr_data.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace chldr_data.Entities;
[Table("User")]

public partial class SqlUser : IUser
{
    public string UserId { get; set; } = Guid.NewGuid().ToString();
    public string? Email { get; set; }
    public string? Password { get; set; }
    public int Rate { get; set; } = 0;
    public string? ImagePath { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Patronymic { get; set; }
    public byte? IsModerator { get; set; }
    public byte? UserStatus { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTime.Now;
    public DateTimeOffset UpdatedAt { get; set; } = DateTime.Now;
    public virtual ICollection<SqlActivity> Activities { get; set; } = new List<SqlActivity>();
    public virtual ICollection<SqlEntry> Entries { get; set; } = new List<SqlEntry>();
    public virtual ICollection<SqlImage> Images { get; set; } = new List<SqlImage>();
    public virtual ICollection<SqlLanguage> Languages { get; set; } = new List<SqlLanguage>();
    public virtual ICollection<SqlQuery> Queries { get; set; } = new List<SqlQuery>();
    public virtual ICollection<SqlSound> Sounds { get; set; } = new List<SqlSound>();
    public virtual ICollection<SqlSource> Sources { get; set; } = new List<SqlSource>();
    public virtual ICollection<SqlTranslation> Translations { get; set; } = new List<SqlTranslation>();
    public virtual ICollection<SqlToken> Tokens { get; set; } = new List<SqlToken>();
}
