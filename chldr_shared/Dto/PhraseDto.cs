using chldr_data.Entities;
using chldr_data.Models;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_shared.Dto
{
    public class PhraseDto
    {
        public ObjectId PhraseId { get; }
        public string Content { get; set; }
        public string? Notes { get; set; }

        public PhraseDto(PhraseModel phrase)
        {
            PhraseId = phrase.PhraseId;
            Content = phrase.Content;
            Notes = phrase.Notes;
        }

        public PhraseDto() { }
    }
}
