using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_shared.Stores;
using Microsoft.AspNetCore.Components;

namespace chldr_ui.ViewModels
{
    public class EntryViewModel : ViewModelBase
    {
        #region Properties
        [Parameter] public EntryModel Entry { get; set; }
        #endregion

        #region Actions
        public void ListenToPronunciation() { }
        public void NewTranslation() { }
        public void AddToFavorites() { }
        public async Task Remove()
        {
            await ContentStore.DeleteEntry(UserStore.CurrentUser!, Entry);
        }
        public void Share() { }
        public async Task PromoteEntryAsync() {
            await ContentStore.EntryService.PromoteAsync(Entry, UserStore.CurrentUser);
        }
        public async Task PromoteTranslationAsync(ITranslation translation)
        {
            await ContentStore.TranslationService.PromoteAsync(translation, UserStore.CurrentUser);
        }
        public void Downvote() { }
        public void Flag() { }

        #endregion
    
        protected static string ParseSource(string sourceName)
        {
            string sourceTitle = null;
            switch (sourceName)
            {
                case "Maciev":
                    sourceTitle = "Чеченско - русский словарь, А.Г.Мациева";
                    break;
                case "Karasaev":
                    sourceTitle = "Русско - чеченский словарь, Карасаев А.Т., Мациев А.Г.";
                    break;
                case "User":
                    sourceTitle = "Добавлено пользователем";
                    break;
                case "Malaev":
                    sourceTitle = "Чеченско - русский словарь, Д.Б. Малаева";
                    break;
                case "Anatslovar":
                    sourceTitle = "Чеченско-русский, русско-чеченский словарь анатомии человека, Р.У. Берсанова";
                    break;
                case "ikhasakhanov":
                    sourceTitle = "Ислам Хасаханов";
                    break;
            }
            return sourceTitle;
        }

        public string? Header => Entry?.Content;
        public string? Subheader => ParseSource(Entry.Source.Name);


        public bool CanEdit()
        {
            // Anyone should be able to open an entry for edit mode, if they're logged in and active
            // However, they might not be able to change anything, that will be governed by CanEdit* methods
            return UserStore.IsLoggedIn && UserStore.CurrentUser!.Status == UserStatus.Active;
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
