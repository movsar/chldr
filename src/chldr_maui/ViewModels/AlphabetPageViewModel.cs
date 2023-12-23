using chldr_utils.Services;
using ReactiveUI;
using System.Media;
using System.Reactive;
using System.Text.RegularExpressions;

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
            string imageFileName = (imageSource as dynamic).File;
            string audioFileName = Regex.Match(imageFileName, @"(?<=alpha_).*?(?=.png)").Value + ".flac";

            var fileService = App.Services.GetRequiredService<FileService>();
            var filePath = Path.Combine(fileService.AlphabetSoundsDirectory, audioFileName);

            using (System.IO.Stream stream = File.OpenRead(filePath))
            using (var player = new SoundPlayer(stream))
            {
                player.Play();
            }

        }
    }
}
