﻿using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.Enums;

namespace chldr_data.DatabaseObjects.Dtos
{
    public abstract class EntryDto : IEntry
    {
        public string EntryId { get; set; }
        public string? SourceId { get; set; }
        public int Rate { get; set; }
        public abstract string Content { get; set; }
        public List<TranslationDto> Translations { get; set; } = new List<TranslationDto>();
        public EntryType EntryType { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
