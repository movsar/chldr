using chldr_data.Interfaces;
using Realms;
using System;
using System.Collections.Generic;

namespace chldr_data.Entities;
public class Source : RealmObject, IEntity
{
    [PrimaryKey]
    public string SourceId { get; set; } = Guid.NewGuid().ToString();
    public User? User { get; set; }
    public string Name { get; set; } = null!;
    public string? Notes { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTime.Now;
    public DateTimeOffset UpdatedAt { get; set; } = DateTime.Now;
    public  IList<Entry> Entries { get; } = new List<Entry>();
}
