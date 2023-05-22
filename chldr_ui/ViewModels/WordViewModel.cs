using chldr_data.DatabaseObjects.Models.Words;
using Microsoft.AspNetCore.Components;
using MongoDB.Bson;

namespace chldr_ui.ViewModels
{
    public class WordViewModel : EntryViewModelBase
    {

        public WordModel? Word { get; set; }

        public string? Header => Word?.Content;
        public string? Subheader => CreateSubheader();
        public void DeleteEntry()
        {
            ContentStore.DeleteEntry(Entry!.EntryId);
        }
        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (Word == null && Entry != null)
            {
                Word = Entry as WordModel;

                Translations = Entry.Translations;
                Source = ParseSource(Entry.Source.Name);
            }
        }
        
        private string CreateSubheader()
        {
            return $"";
        }

        public static string ParseGrammaticalClass(List<int> grammaticalClasses)
        {
            var ClassesMap = new Dictionary<int, string>()
            {
                { 1 ,"в, б/д"},
                { 2 ,"й, б/д"},
                { 3 ,"й, й"},
                { 4 ,"д, д"},
                { 5 ,"б, б/й"},
                { 6 ,"б, д"},
            };

            //if (grammaticalClass == 0)
            //{
            //    return null;
            //}

            //return ClassesMap[grammaticalClass];
            return "";
        }
    }
}
