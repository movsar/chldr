using ReactiveUI;
using System.Reactive;

namespace dosham.ViewModels
{
    public class AlphabetPageViewModel : ViewModelBase
    {
        public ReactiveCommand<ImageSource, Unit> ImageButtonClicked { get; }

        public AlphabetPageViewModel()
        {
            ImageButtonClicked = ReactiveCommand.Create<ImageSource>(OnImageButtonClicked);
        }

        private void OnImageButtonClicked(ImageSource imageSource)
        {
            string fileName = (imageSource as dynamic).File;

        }
    }
}
