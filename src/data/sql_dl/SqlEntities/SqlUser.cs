using domain.DatabaseObjects.Dtos;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using domain.DatabaseObjects.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace sql_dl.SqlEntities;

public class SqlUser : IdentityUser, IUserEntity
{
    public SqlUser() { }
  
    public string? ImagePath { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Patronymic { get; set; }
    public string EmailConfirmationToken { get; set; } = string.Empty;
    public int Rate { get; set; } = 0;
    public int Type { get; set; } = 0;
    public int Status { get; set; } = 0;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    public virtual ICollection<SqlEntry> Entries { get; set; } = new List<SqlEntry>();
    public virtual ICollection<SqlQuery> Queries { get; set; } = new List<SqlQuery>();
    public virtual ICollection<SqlSound> Sounds { get; set; } = new List<SqlSound>();
    public virtual ICollection<SqlSource> Sources { get; set; } = new List<SqlSource>();
    public virtual ICollection<SqlTranslation> Translations { get; set; } = new List<SqlTranslation>();
    public virtual ICollection<SqlChangeSet> ChangeSets { get; set; } = new List<SqlChangeSet>();

    public static SqlUser FromDto(UserDto userDto)
    {
        return new SqlUser()
        {
            Id = userDto.Id,
            Rate = userDto.Rate,
            Email = userDto.Email,
            PasswordHash = userDto.Password,
            Status = (int)userDto.Status,
            Type = (int)userDto.Type,
            FirstName = userDto.FirstName,
            LastName = userDto.LastName,
            Patronymic = userDto.Patronymic,
        };
    }

}
