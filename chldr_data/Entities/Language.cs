﻿using chldr_data.Interfaces;
using Realms;
using System;
using System.Collections.Generic;

namespace chldr_data.Entities;
public  class Language : RealmObject, IEntity
{
    [PrimaryKey]
    public string LanguageId { get; set; } = Guid.NewGuid().ToString();
    public User? User { get; set; }
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; } = DateTime.Now;
    public DateTimeOffset UpdatedAt { get; set; } = DateTime.Now;
    public IList<Translation> Translations { get; set; } = new List<Translation>();
}
