﻿using chldr_data.Interfaces;
using Realms;

namespace chldr_data.Entities;
public  class Translation : RealmObject, IEntity
{
    [PrimaryKey]
    public string TranslationId { get; set; } = Guid.NewGuid().ToString();
    public Language Language { get; set; } = null!;
    public Entry Entry { get; set; } = null!;
    public User User { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string RawContents { get; set; } = null!;
    public string? Notes { get; set; }
    public int Rate { get; set; } = 0;
    public DateTimeOffset CreatedAt { get; set; } = DateTime.Now;
    public DateTimeOffset UpdatedAt { get; set; } = DateTime.Now;
    internal string GetRawContents()
    {
        return Content.ToString();
    }
}
