using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.Models.Words;
using chldr_data.Enums;
using Microsoft.AspNetCore.Components;
using MongoDB.Bson;

namespace chldr_ui.ViewModels
{
    public class EntryViewModel : EntryViewModelBase
    {
        public EntryModel? Word { get; set; }
        // Only for Words
        public List<EntryModel> SubWords { get; set; }

        public string? Header => Word?.Content;
        public string? Subheader => CreateSubheader();

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (Word == null && Entry != null)
            {
                Word = Entry as EntryModel;

                Translations = Entry.Translations;
                Source = ParseSource(Entry.Source.Name);
            }
        }

        public bool CanEdit()
        {
            // Anyone should be able to open an entry for edit mode, if they're logged in and active
            // However, they might not be able to change anything, that will be governed by CanEdit* methods
            return UserStore.IsLoggedIn && UserStore.CurrentUser!.Status == UserStatus.Active;
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
