using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_data.Models;
using chldr_shared.Stores;

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

            var count = await ContentStore.GetEntriesStartingWithCount(new FiltrationFlags()
            {
                OnModeration = true,
                GroupWithSubEntries = true,
                StartsWith = CurrentLetter,
                EntryTypes = new EntryType[] { EntryType.Word }
            });
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
            var batch = await ContentStore.TakeEntriesAsync((CurrentPage - 1) * 50, 50, new FiltrationFlags()
            {
                OnModeration = true,
                GroupWithSubEntries = true,
                StartsWith = CurrentLetter,
                EntryTypes = new EntryType[] { EntryType.Word }
            });
            Entries = batch.ToList();

            await RefreshUiAsync();
        }
    }
}
