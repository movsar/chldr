using chldr_data.Entities;
using chldr_data.Interfaces;
using MongoDB.Bson;
using Realms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_data.Models
{
    public class WordModel : EntryModel
    {
        public WordModel(Entities.Word word) : base(word.Entry)
        {
            Id = word._id;
            Content = word.Content;
            Notes = word.Notes;
            RawForms = word.Forms;
            GrammaticalClass = word.GrammaticalClass;
            RawVerbTenses = word.VerbTenses;
            RawNounDeclensions = word.NounDeclensions;
            PartOfSpeech = word.PartOfSpeech;
        }

        public WordModel(Entities.Entry entry) : this(entry.Word) { }

        public new ObjectId Id { get; }
        public string Content { get; }
        public string Notes { get; }
        public string RawForms { get; }
        public string RawVerbTenses { get; }
        public string RawNounDeclensions { get; }
        public int PartOfSpeech { get; }
        public int GrammaticalClass { get; internal set; }


    }
}
