using chldr_data.DatabaseObjects.Models;
using chldr_shared.Stores;

namespace chldr_ui.ViewModels
{
    public class IndexPageViewModel : ViewModelBase
    {
        public List<string> Letters = new List<string>() {
  "А", "Аь", "Б", "В", "Г", "ГӀ", "Д", "Е", "Ё", "Ж", "З", "И", "Й", "К", "Кх", "Къ", "КӀ", "Л", "М", "Н", "О",
  "Оь", "П", "ПӀ", "Р", "С", "Т", "ТӀ", "У", "Уь", "Ф", "Х", "Хь", "ХӀ", "Ц", "ЦӀ", "Ч", "ЧӀ", "Ш", "Щ", "Ъ", "Ы",
  "Ь", "Э", "Ю", "Юь", "Я", "Яь", "Ӏ"
  };

        public List<EntryModel> Entries;

        public int currentPage = 1;
        public int totalPages;

        protected async override Task OnInitializedAsync()
        {
            await GetEntries(currentPage);
        }

        public async Task SelectPageAsync(int page)
        {
            currentPage = page;
            await GetEntries(page);
        }

        private async Task GetEntries(int page)
        {
            var batch = await ContentStore.TakeEntriesAsync((page - 1) * 50, 50, true);
            Entries = batch.ToList();
            totalPages = await ContentStore.GetEntriesCount() / 50;
        }

        public async Task FilterByLetter(string letter)
        {
            await SelectPageAsync(1);

            var batch = await ContentStore.TakeEntriesAsync((currentPage - 1) * 50, 50, true, letter);
            Entries = batch.ToList();
            totalPages = await ContentStore.GetEntriesCount() / 50;
        }

    }
}
