using Plugin.Maui.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace chldr_ui.ViewModels
{
    internal class AlphabetPageViewModel
    {
        //private async Task PlaySoundFile(string filePath)
        //{
        //    using (var fileStream = File.Open(filePath, FileMode.Open))
        //    {
        //        var player = _audioRecorder.CreatePlayer(fileStream);
        //        var tcs = new TaskCompletionSource<bool>();

        //        // Event handler for playback completion
        //        void PlaybackCompleted(object? sender, EventArgs args)
        //        {
        //            player.PlaybackEnded -= PlaybackCompleted;
        //            tcs.SetResult(true);
        //        }
        //        player.PlaybackEnded += PlaybackCompleted;

        //        player.Play();

        //        await tcs.Task;

        //        player?.Dispose();
        //    }
        //}

        //private async Task OnImageButtonClicked(object imageSource)
        //{
        //    string imageFileName = (imageSource as dynamic).File;
        //    string audioFileName = Regex.Match(imageFileName, @"(?<=alpha_).*?(?=.png)").Value + ".flac";

        //    var filePath = Path.Combine(_fileService.AlphabetSoundsDirectory, audioFileName);
        //    await PlaySoundFile(filePath);
        //}
    }
}
