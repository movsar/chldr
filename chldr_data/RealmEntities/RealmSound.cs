﻿using chldr_data.Interfaces.DatabaseEntities;
using Realms;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace chldr_data.Entities;

[MapTo("Sound")]
public class RealmSound : RealmObject, IEntity
{
    [PrimaryKey]
    public string SoundId { get; set; } = Guid.NewGuid().ToString();
    public RealmUser User { get; set; } = null!;
    public RealmEntry Entry { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; } = DateTime.Now;
    public DateTimeOffset UpdatedAt { get; set; } = DateTime.Now;
}
