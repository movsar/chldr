﻿using chldr_data.Interfaces;
using Realms;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace chldr_data.Entities;

public  class Sound : RealmObject, IEntity
{
    [PrimaryKey]
    public string SoundId { get; set; } = Guid.NewGuid().ToString();
    public User User { get; set; } = null!;
    public Entry Entry { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; } = DateTime.Now;
    public DateTimeOffset UpdatedAt { get; set; } = DateTime.Now;
}
