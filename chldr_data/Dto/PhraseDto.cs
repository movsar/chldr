﻿using chldr_data.Models;
using MongoDB.Bson;

namespace chldr_data.Dto
{
    public class PhraseDto : EntryDto
    {
        public ObjectId PhraseId { get; }
        public string Content { get; set; } = string.Empty;
        public string? Notes { get; set; } = string.Empty;

        public PhraseDto(PhraseModel phrase) : base(phrase)
        {
            PhraseId = phrase.Id;
            Content = phrase.Content;
            Notes = phrase.Notes;
        }

        public PhraseDto() { }
    }
}
