using chldr_utils.Services;
using Plugin.Maui.Audio;
using ReactiveUI;
using System.Reactive;
using System.Text.RegularExpressions;

namespace dosham.ViewModels
{
    public class AlphabetPageViewModel : ViewModelBase
    {
        private readonly FileService _fileService;
        private readonly IAudioManager _audioRecorder;

        public ReactiveCommand<ImageSource, Unit> ImageButtonClicked { get; }

        public AlphabetPageViewModel()
        {
            _fileService = App.Services.GetRequiredService<FileService>();
            _audioRecorder = App.Services.GetRequiredService<IAudioManager>();

            ImageButtonClicked = ReactiveCommand.CreateFromTask<ImageSource>(OnImageButtonClicked);
        }

        private async Task PlaySoundFile(string filePath)
        {
            using (var fileStream = File.Open(filePath, FileMode.Open))
            {
                var player = _audioRecorder.CreatePlayer(fileStream);
                var tcs = new TaskCompletionSource<bool>();

                // Event handler for playback completion
                void PlaybackCompleted(object? sender, EventArgs args)
                {
                    player.PlaybackEnded -= PlaybackCompleted;
                    tcs.SetResult(true);
                }
                player.PlaybackEnded += PlaybackCompleted;

                player.Play();

                await tcs.Task;

                player?.Dispose();
            }
        }

        private async Task OnImageButtonClicked(ImageSource imageSource)
        {
            string imageFileName = (imageSource as dynamic).File;
            string audioFileName = Regex.Match(imageFileName, @"(?<=alpha_).*?(?=.png)").Value + ".flac";

            var filePath = Path.Combine(_fileService.AlphabetSoundsDirectory, audioFileName);
            await PlaySoundFile(filePath);
        }
    }
}
