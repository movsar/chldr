using chldr_data.Dto;
using chldr_data.Interfaces.DatabaseEntities;
using chldr_data.SqlEntities;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace chldr_data.Entities;
[Table("User")]

public partial class SqlUser : IUser
{
    public SqlUser() { }
    public SqlUser(UserDto testUser)
    {

        Email = testUser.Email;
        Password = testUser.Password;
        UserStatus = (byte)testUser.UserStatus;
        FirstName = testUser.FirstName;
        LastName = testUser.LastName;
        Patronymic = testUser.Patronymic;
    }

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
    [JsonIgnore]
    public virtual ICollection<SqlActivity> Activities { get; set; } = new List<SqlActivity>();
    [JsonIgnore]
    public virtual ICollection<SqlEntry> Entries { get; set; } = new List<SqlEntry>();
    [JsonIgnore]
    public virtual ICollection<SqlImage> Images { get; set; } = new List<SqlImage>();
    [JsonIgnore]
    public virtual ICollection<SqlLanguage> Languages { get; set; } = new List<SqlLanguage>();
    [JsonIgnore]
    public virtual ICollection<SqlQuery> Queries { get; set; } = new List<SqlQuery>();
    [JsonIgnore]
    public virtual ICollection<SqlSound> Sounds { get; set; } = new List<SqlSound>();
    [JsonIgnore]
    public virtual ICollection<SqlSource> Sources { get; set; } = new List<SqlSource>();
    [JsonIgnore]
    public virtual ICollection<SqlTranslation> Translations { get; set; } = new List<SqlTranslation>();
    [JsonIgnore]
    public virtual ICollection<SqlToken> Tokens { get; set; } = new List<SqlToken>();
    [JsonIgnore]
    public virtual ICollection<SqlChangeSet> ChangeSets { get; set; } = new List<SqlChangeSet>();
}
