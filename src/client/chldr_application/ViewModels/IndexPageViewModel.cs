using core.DatabaseObjects.Models;
using core.Enums;
using core.Models;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;

namespace chldr_application.ViewModels
{
    public class IndexPageViewModel : ViewModelBase
    {
        public string[] Letters { get; } = {
              "Ӏ", "А", "Аь", "Б", "В", "Г", "ГӀ", "Д", "Е", "Ё", "Ж", "З", "И", "Й", "К", "Кх", "Къ", "КӀ",
              "Л", "М", "Н", "О", "Оь", "П", "ПӀ", "Р", "С", "Т", "ТӀ", "У", "Уь", "Ф", "Х", "Хь", "ХӀ",
              "Ц", "ЦӀ", "Ч", "ЧӀ", "Ш", "Э", "Ю", "Юь", "Я", "Яь"
        };
        public ReactiveCommand<Unit, Unit> BtnNextClickedCommand { get; }
        public ReactiveCommand<Unit, Unit> BtnPreviousClickedCommand { get; }
        public ReactiveCommand<string, Unit> LetterSelectionCommand { get; }

        private IEnumerable<EntryModel> _entries;
        public IEnumerable<EntryModel> Entries
        {
            get => _entries;
            set => this.RaiseAndSetIfChanged(ref _entries, value);
        }

        public int CurrentPage = 1;
        public int TotalPages;
        public string CurrentLetter;

        public IndexPageViewModel()
        {
            BtnNextClickedCommand = ReactiveCommand.Create(OnNextPage);
            BtnPreviousClickedCommand = ReactiveCommand.Create(OnPreviousPage);
            LetterSelectionCommand = ReactiveCommand.CreateFromTask<string>(LetterSelectionHandler);

            LetterSelectionHandler(Letters[0]).ConfigureAwait(false);
        }

        public async Task LetterSelectionHandler(string letter)
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


        }


        internal async void OnPreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage -= 1;
                await GetEntries();
            }
        }

        internal async void OnNextPage()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage += 1;
                await GetEntries();
            }
        }
    }
}
