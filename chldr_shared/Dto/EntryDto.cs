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
    public abstract class EntryDto
    {
        public ObjectId EntryId { get; set; }
        public List<TranslationDto> Translations { get; } = new List<TranslationDto>();
        public SourceModel Source { get; set; }
        public int Rate { get; set; }
        public int Type { get; set; }
        public EntryDto(EntryModel entry)
        {
            EntryId = entry.EntryId;
            Source = entry.Source;
            Rate = entry.Rate;
            Type = entry.Type;
            foreach (var translation in entry.Translations)
            {
                Translations.Add(new TranslationDto(translation));
            }
        }
    }
}
