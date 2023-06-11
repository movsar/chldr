using chldr_data.DatabaseObjects.Dtos;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using chldr_data.DatabaseObjects.Interfaces;

namespace chldr_data.remote.SqlEntities;

[Table("User")]
public class SqlUser : IUserEntity
{
    public SqlUser() { }
    public static SqlUser FromDto(UserDto testUser)
    {
        return new SqlUser()
        {
            Email = testUser.Email,
            Password = testUser.Password,
            Status = (int)testUser.Status,
            FirstName = testUser.FirstName,
            LastName = testUser.LastName,
            Patronymic = testUser.Patronymic
        };
    }

    public string UserId { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public int Rate { get; set; } = 0;
    public string? ImagePath { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Patronymic { get; set; }
    public int? IsModerator { get; set; }
    public int? Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTime.Now;
    public DateTimeOffset UpdatedAt { get; set; } = DateTime.Now;
    public virtual ICollection<SqlEntry> Entries { get; set; } = new List<SqlEntry>();
    public virtual ICollection<SqlLanguage> Languages { get; set; } = new List<SqlLanguage>();
    public virtual ICollection<SqlQuery> Queries { get; set; } = new List<SqlQuery>();
    public virtual ICollection<SqlSound> Sounds { get; set; } = new List<SqlSound>();
    public virtual ICollection<SqlSource> Sources { get; set; } = new List<SqlSource>();
    public virtual ICollection<SqlTranslation> Translations { get; set; } = new List<SqlTranslation>();
    public virtual ICollection<SqlToken> Tokens { get; set; } = new List<SqlToken>();
    public virtual ICollection<SqlChangeSet> ChangeSets { get; set; } = new List<SqlChangeSet>();
}
