﻿using core.DatabaseObjects.Dtos;
using System.ComponentModel.DataAnnotations.Schema;
using core.DatabaseObjects.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace api_domain.SqlEntities;

[Table("Source")]
public class SqlSource : ISourceEntity
{
    [Key]
    public string SourceId { get; set; } 
    public string? UserId { get; set; }
    public string Name { get; set; } = null!;
    public string? Notes { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    public virtual ICollection<SqlEntry> Entries { get; set; } = new List<SqlEntry>();
    public virtual SqlUser? User { get; set; }
    public static ISourceEntity FromDto(SourceDto source)
    {
        return new SqlSource()
        {
            SourceId = source.SourceId,
            UserId = source.UserId,
            Name = source.Name,
            Notes = source.Notes,
        };
    }
}
