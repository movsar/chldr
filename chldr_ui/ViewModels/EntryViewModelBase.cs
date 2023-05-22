using chldr_data.DatabaseObjects.Models;
using Microsoft.AspNetCore.Components;

namespace chldr_ui.ViewModels
{
    public abstract class EntryViewModelBase : ViewModelBase
    {
        [Parameter]
        public EntryModel? Entry { get; set; }

        #region Properties
        public string? Source { get; set; }
        public List<TranslationModel> Translations { get; set; }
        #endregion

        #region Actions
        public void ListenToPronunciation() { }
        public void NewTranslation() { }
        public void AddToFavorites() { }
        public void Share() { }
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
    }
}
