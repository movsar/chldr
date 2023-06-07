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
    [Ignored]
    public string? UserId => User?.UserId;
}
