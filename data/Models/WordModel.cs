using Data.Entities;
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
    public class WordModel : TargetModel
    {
        public class PartsOfSpeech
        {
            public const byte Verb = 1;
            public const byte Noun = 2;
        };

        public ObjectId EntityId { get; }
        public string Content { get; }
        public string Notes { get; }
        public string RawForms { get; }
        public string RawVerbTenses { get; }
        public string RawNounDeclensions { get; }
        public int GrammaticalClass { get; internal set; }

        public WordModel(Word word)
        {
            EntityId = word._id;
            Content = word.Content;
            Notes = word.Notes;
            RawForms = word.Forms;
            GrammaticalClass = word.GrammaticalClass;
            RawVerbTenses = word.VerbTenses;
            RawNounDeclensions = word.NounDeclensions;
        }

    }
}
