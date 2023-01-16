using Data.Entities;
using Data.Interfaces;
using MongoDB.Bson;
using Realms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class WordModel : EntryModel
    {
        public WordModel(Entities.Entry entry) : base(entry)
        {
            EntityId = entry.Word._id;
            Content = entry.Word.Content;
            Notes = entry.Word.Notes;
            RawForms = entry.Word.Forms;
            GrammaticalClass = entry.Word.GrammaticalClass;
            RawVerbTenses = entry.Word.VerbTenses;
            RawNounDeclensions = entry.Word.NounDeclensions;
            PartOfSpeech = entry.Word.PartOfSpeech;
        }

        public new ObjectId EntityId { get; }
        public string Content { get; }
        public new string Notes { get; }
        public string RawForms { get; }
        public string RawVerbTenses { get; }
        public string RawNounDeclensions { get; }
        public int PartOfSpeech { get; }
        public int GrammaticalClass { get; internal set; }


    }
}
