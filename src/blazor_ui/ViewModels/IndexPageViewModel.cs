using domain.DatabaseObjects.Models;
using domain;
using domain.Models;
using domain.Enums;

namespace chldr_ui.ViewModels
{
    public class IndexPageViewModel : ViewModelBase
    {
        public List<string> Letters = new List<string>() {
              "Ӏ", "А", "Аь", "Б", "В", "Г", "ГӀ", "Д", "Е", "Ё", "Ж", "З", "И", "Й", "К", "Кх", "Къ", "КӀ",
              "Л", "М", "Н", "О", "Оь", "П", "ПӀ", "Р", "С", "Т", "ТӀ", "У", "Уь", "Ф", "Х", "Хь", "ХӀ",
              "Ц", "ЦӀ", "Ч", "ЧӀ", "Ш", "Э", "Ю", "Юь", "Я", "Яь"
        };

        public List<EntryModel> Entries;

        public int CurrentPage = 1;
        public int TotalPages;
        public string CurrentLetter;

        public async Task LetterSelectionHandlerAsync(string letter)
        {
            CurrentPage = 1;
            CurrentLetter = letter;

            var filtrationFlags = new FiltrationFlags()
            {
                EntryFilters = new EntryFilters()
                {
                    IncludeOnModeration = true,
                    StartsWith = CurrentLetter,
                    EntryTypes = new EntryType[] { EntryType.Word }
                }
            };

            var count = await ContentStore.EntryService.GetCountAsync(filtrationFlags);
            TotalPages = (int)Math.Ceiling((double)count / 50);

            await GetEntries();
        }
        protected async override Task OnInitializedAsync()
        {
            await LetterSelectionHandlerAsync(Letters[0]);
        }

        public async Task SelectPageAsync(int page)
        {
            CurrentPage = page;

            await GetEntries();
        }

        public async Task GetEntries()
        {
            var filtrationFlags = new FiltrationFlags()
            {
                EntryFilters = new EntryFilters()
                {
                    IncludeOnModeration = true,
                    StartsWith = CurrentLetter,
                    EntryTypes = new EntryType[] { EntryType.Word }
                }
            };

            var batch = await ContentStore.EntryService.TakeAsync((CurrentPage - 1) * 50, 50, filtrationFlags);
            Entries = batch.ToList();

            await RefreshUiAsync();
        }
    }
}
