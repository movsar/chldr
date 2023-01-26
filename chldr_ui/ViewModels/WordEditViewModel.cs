using chldr_data.Models;
using chldr_shared.Stores;
using Microsoft.AspNetCore.Components;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_ui.ViewModels
{
    public class WordEditViewModel : EntryViewModelBase
    {
        #region Properties
        public int PartOfSpeech { get; set; }
        public int GrammaticalClass { get; set; }
        public string Content { get; set; }
        public string Notes { get; set; }
        public string RawForms { get; }
        public string RawVerbTenses { get; }
        public string RawNounDeclensions { get; }
        #endregion

        //protected override void InitializeViewModel(EntryModel entry)
        //{
        //    base.InitializeViewModel(entry);

        //    var word = entry as WordModel;

        //    EntityId = word.EntityId.ToString();
        //    GrammaticalClass = word.GrammaticalClass;
        //    Content = word.Content;
        //    Notes = word.Notes;
        //    PartOfSpeech = word.PartOfSpeech;

        //    var subheader = BuildWordInfoSubheader(word.Content, word.GrammaticalClass, word.RawForms, word.RawNounDeclensions, word.RawVerbTenses);
        //    Header = word.Content;
        //    Subheader = subheader;

        //    //case TextModel:
        //    //    var text = entry as TextModel;
        //    //    Header = text.Content;
        //    //    Type = EntryType.Text;

        //    //    break;
        //}
    }
}
