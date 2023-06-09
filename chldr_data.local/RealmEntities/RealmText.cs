﻿using chldr_data.DatabaseObjects.Interfaces;
using Realms;

namespace chldr_data.local.RealmEntities;

[MapTo("Text")]
public class RealmText : RealmObject, ITextEntity
{
    [PrimaryKey]
    public string TextId { get; set; }
    public RealmEntry Entry { get; set; } = null!;
    public string Content { get; set; } = null!;
}