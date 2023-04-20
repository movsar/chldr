﻿using Realms;
using chldr_data.Interfaces;

namespace chldr_data.Entities;

public class Activity : RealmObject, IEntity
{
    [PrimaryKey]
    public string ActivityId { get; set; } = Guid.NewGuid().ToString();
    public User User { get; set; } = null!;
    public string ObjectId { get; set; } = null!;
    public string ObjectClass { get; set; } = null!;
    public string ObjectProperty { get; set; } = null!;
    public string OldValue { get; set; } = null!;
    public string NewValue { get; set; } = null!;
    public string? Notes { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTime.Now;
    public DateTimeOffset UpdatedAt { get; set; } = DateTime.Now;
}
