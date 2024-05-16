using chldr_app.Stores;
using core.DatabaseObjects.Dtos;
using core.DatabaseObjects.Interfaces;
using core.DatabaseObjects.Models;
using core.DatabaseObjects.Models.Words;
using core.Enums;
using core.Enums.WordDetails;
using core.Interfaces;
using Microsoft.AspNetCore.Components;
using System;
using System.Security.Claims;
using System.Text.RegularExpressions;

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
            await ContentStore.EntryService.RemoveAsync(Entry, UserStore.CurrentUser.Id!);
        }
        public void Share() { }
        public async Task PromoteEntryAsync()
        {
            await ContentStore.EntryService.PromoteAsync(Entry, UserStore.CurrentUser);
        }
        public async Task PromoteTranslationAsync(ITranslation translation)
        {
            await ContentStore.EntryService.PromoteTranslationAsync(translation, UserStore.CurrentUser);
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
        public async Task DoSearch()
        {
            var translationText = "";//Translation?.Content.ToLower();

            string[] prefixesToSearch = {
                "см",
                "понуд.? от",
                "потенц.? от",
                "прил.? к",
                "масд.? от"
            };

            foreach (var prefix in prefixesToSearch)
            {

                string pattern = $"(?<={prefix}\\W?\\s?)[1ӀӏА-яA-z]+";
                var match = Regex.Match(translationText, pattern, RegexOptions.CultureInvariant);

                if (match.Success)
                {
                    await ContentStore.EntryService.FindAsync(match.ToString());
                    return;
                }
            }
        }
      
        private string GetHeader()
        {
            string header = Entry?.Content!;
            string className = string.Empty;

            if (Entry!.Details != null)
            {
                switch ((WordType)Entry.Subtype)
                {

                    case WordType.Noun:
                        var details = Entry.Details as NounDetails;
                        if (details?.Class > 0)
                        {
                            className = @GrammaticalClassToString(details!.Class);
                        }
                        break;

                    case WordType.Verb:
                        break;

                    default:
                        Console.WriteLine("no handler for the details of this type");
                        break;
                }
                if (!string.IsNullOrEmpty(className))
                {
                    header = string.Join(" ", header, className);
                }
            }
            return header;
        }
        public string? Header => GetHeader();

        public string? Subheader => "";/*ParseSource(Entry.Source.Name);*/


        public bool CanEdit()
        {
            // Anyone should be able to open an entry for edit mode, if they're logged in and active
            // However, they might not be able to change anything, that will be governed by CanEdit* methods
            return UserStore.IsLoggedIn && UserStore.CurrentUser!.Status == UserStatus.Active;
        }


        public static string GrammaticalClassToString(int grammaticalClass)
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

            if (ClassesMap.TryGetValue(grammaticalClass, out string? classString))
            {
                return classString;
            }
            else
            {
                return "";
            }
        }
    }
}
