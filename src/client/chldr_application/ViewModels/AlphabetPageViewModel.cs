using core.Interfaces;
using Plugin.Maui.Audio;
using ReactiveUI;
using System.Reactive;
using System.Text.RegularExpressions;

namespace chldr_application.ViewModels
{
    public class AlphabetPageViewModel : ViewModelBase
    {
        private readonly IFileService _fileService;
        private readonly IAudioManager _audioRecorder;

        public ReactiveCommand<object, Unit> ImageButtonClicked { get; }

        public AlphabetPageViewModel(IFileService fileService, IAudioManager audioRecorder)
        {
            _fileService = fileService;
            _audioRecorder = audioRecorder;

            ImageButtonClicked = ReactiveCommand.CreateFromTask<object>(OnImageButtonClicked);
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

        private async Task OnImageButtonClicked(object imageSource)
        {
            string imageFileName = (imageSource as dynamic).File;
            string audioFileName = Regex.Match(imageFileName, @"(?<=alpha_).*?(?=.png)").Value + ".flac";

            var filePath = Path.Combine(_fileService.AlphabetSoundsDirectory, audioFileName);
            await PlaySoundFile(filePath);
        }
    }
}
