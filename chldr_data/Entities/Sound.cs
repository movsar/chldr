﻿using chldr_data.Interfaces;
using Realms;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace chldr_data.Entities;
[Table("Sound")]

public partial class Sound : RealmObject, IEntity
{
    [Realms.PrimaryKey]
    public string SoundId { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = null!;
    public string EntryId { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public virtual Entry Entry { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}
