using domain.DatabaseObjects.Dtos;
using domain.DatabaseObjects.Interfaces;
using domain.DatabaseObjects.Models;
using Realms;
using System;
using System.Collections.Generic;

namespace realm_dl.RealmEntities;
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
    [Ignored] public string? UserId => User?.Id;

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

    internal static RealmSource FromModel(SourceModel sourceModel)
    {
        if (string.IsNullOrEmpty(sourceModel.SourceId))
        {
            throw new NullReferenceException();
        }

        var source = new RealmSource();

        source.SourceId = sourceModel.SourceId;
        source.Name = sourceModel.Name;

        return source;
    }
}
