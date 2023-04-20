﻿using chldr_data.Interfaces;
using Realms;
namespace chldr_data.Entities;

public partial class Word : RealmObject, IEntity
{
    [PrimaryKey]
    public string WordId { get; set; } = Guid.NewGuid().ToString();
    public Entry Entry { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string? Notes { get; set; }
    public int? PartOfSpeech { get; set; }
    public string? AdditionalDetails { get; set; }
}
