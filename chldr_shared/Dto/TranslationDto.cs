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
    public class TranslationDto
    {
        public ObjectId TranslationId { get; }
        public ObjectId EntryId { get; }
        public string Content { get; set; }
        public string Notes { get; set; }
        public string LanguageCode { get; set; }
        public int Rate { get; set; }
        public TranslationDto(TranslationModel translation)
        {
            TranslationId = translation.Id;
            Content = translation.Content;
            Notes = translation.Notes;
            Rate = translation.Rate;
            LanguageCode = translation.Language.Code;
        }
    }
}
