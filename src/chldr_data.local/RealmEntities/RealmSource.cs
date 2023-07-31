using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using Realms;
using System;
using System.Collections.Generic;

namespace chldr_data.local.RealmEntities;
[MapTo("Source")]
public class RealmSource : RealmObject, ISourceEntity
{
    [PrimaryKey]
    public string SourceId { get; set; }
    public RealmUser? User { get; set; }
    public string Name { get; set; } = null!;
    public string? Notes { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTime.Now;
    public DateTimeOffset UpdatedAt { get; set; } = DateTime.Now;
    public IList<RealmEntry> Entries { get; }
    [Ignored] public string? UserId => User?.UserId;

    internal static RealmSource FromDto(SourceDto dto, Realm dbContext)
    {
        if (string.IsNullOrEmpty(dto.UserId) || dbContext == null)
        {
            throw new NullReferenceException();
        }

        // Translation
        var source = dbContext.Find<RealmSource>(dto.SourceId);
        if (source == null)
        {
            source = new RealmSource();
        }

        source.SourceId = dto.SourceId;
        source.Name = dto.Name;
        source.CreatedAt = dto.CreatedAt;
        source.UpdatedAt = dto.UpdatedAt;

        return source;
    }
}
