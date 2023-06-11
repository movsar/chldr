﻿using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace chldr_data.remote.SqlEntities;
[Table("Text")]

public class SqlText : ITextEntity
{
    public string TextId { get; set; }
    public string EntryId { get; set; } = null!;
    public string Content { get; set; } = null!;
    public virtual SqlEntry Entry { get; set; } = null!;
}
