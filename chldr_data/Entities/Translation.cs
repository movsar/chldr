﻿using chldr_data.Interfaces;
using Realms;
using System.ComponentModel.DataAnnotations.Schema;

namespace chldr_data.Entities;
[Table("Translation")]

public partial class Translation : RealmObject, IEntity
{
    [Realms.PrimaryKey]
    public string TranslationId { get; set; } = Guid.NewGuid().ToString();
    public string LanguageId { get; set; } = null!;
    public string EntryId { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string RawContents { get; set; } = null!;
    public string? Notes { get; set; }
    public int Rate { get; set; } = 0;
    [Ignored] public virtual Entry Entry { get; set; } = null!;
    [Ignored] public virtual Language Language { get; set; } = null!;
    [Ignored] public virtual User User { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; } = DateTime.Now;
    public DateTimeOffset UpdatedAt { get; set; } = DateTime.Now;
    internal string GetRawContents()
    {
        return Content.ToString();
    }
}
