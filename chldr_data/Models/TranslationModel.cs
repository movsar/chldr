using chldr_data.Entities;
using chldr_data.Interfaces;
using MongoDB.Bson;
using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_data.Models
{
    public class TranslationModel : ModelBase
    {
        public string Content { get; }
        public string Notes { get; }
        public LanguageModel Language { get; }
        public int Rate { get; set; }
        public TranslationModel(Translation translation) : base(translation)
        {
            Content = translation.Content;
            Notes = translation.Notes;
            Rate = translation.Rate;
            Language = new LanguageModel(translation.Language);
        }
    }
}
