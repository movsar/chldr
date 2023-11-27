using ReactiveUI;
using System.Reactive;

namespace dosham.ViewModels
{
    public class MainPageViewModel : ReactiveObject
    {
        private int _count;
        public int Count
        {
            get => _count;
            set => this.RaiseAndSetIfChanged(ref _count, value);
        }

        public ReactiveCommand<Unit, Unit> CounterCommand { get; }

        public MainPageViewModel()
        {
            CounterCommand = ReactiveCommand.Create(IncrementCount);
        }

        private void IncrementCount()
        {
            Count++;
        }
    }
}
