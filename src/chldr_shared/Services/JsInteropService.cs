using chldr_data.DatabaseObjects.Dtos;
using chldr_data.Resources.Localizations;
using GraphQL;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Text.Json;

namespace chldr_shared
{
    public class JsInteropService : IAsyncDisposable
    {
        private readonly Lazy<Task<IJSObjectReference>> moduleTask;
        private readonly IStringLocalizer<AppLocalizations> _localizer;

        public static Action<string> OnRemoveAudio { get; set; }
        public static Action<string> OnPromoteAudio { get; set; }
        public static Action<string, string, string> OnAudioRecorded { get; set; }

        public JsInteropService(IJSRuntime jsRuntime, IStringLocalizer<AppLocalizations> localizer)
        {
            moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./_content/chldr_ui/jsInterop.js").AsTask());

            _localizer = localizer;
        }


        public async ValueTask<string> Prompt(string message)
        {
            var module = await moduleTask.Value;
            return await module.InvokeAsync<string>("showPrompt", message);
        }

        public async ValueTask ClickShowRandoms()
        {
            try
            {
                var module = await moduleTask.Value;
                await module.InvokeAsync<string>("clickShowRandoms");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error while clicking");
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (moduleTask.IsValueCreated && moduleTask.Value != null)
            {
                var module = await moduleTask.Value;
                try
                {
                    await module.DisposeAsync();
                }
                catch (Exception ex)
                {

                }
            }
        }
        public async ValueTask Disable(string selector)
        {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("disable", selector);
        }
        public async ValueTask Enable(string selector)
        {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("enable", selector);
        }

        public async ValueTask DisableAll(string selector)
        {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("disableAll", selector);
        }

        public async Task StartRecording(string recordingId)
        {
            var lblPronunciation = _localizer["Pronunciation"].ToString();
            var lblStopRecording = _localizer["StopRecording"].ToString();

            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("startRecording", recordingId, lblPronunciation, lblStopRecording);
        }

        public async Task StopRecording(string entryId)
        {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("stopRecording", entryId);
        }

        [JSInvokable]
        public static void WordEdit_StopRecording_ClickHandler(string entryId, string soundId, string soundB64)
        {
            OnAudioRecorded?.Invoke(entryId, soundId, soundB64);
        }

        [JSInvokable]
        public static void WordEdit_PromoteSound_ClickHandler(string recordingId)
        {
            OnPromoteAudio?.Invoke(recordingId);
        }

        [JSInvokable]
        public static void WordEdit_RemoveSound_ClickHandler(string recordingId)
        {
            OnRemoveAudio?.Invoke(recordingId);
        }

        public async Task AddExistingEntryRecording(PronunciationDto soundDto, bool canPromote, bool canRemove)
        {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("addExistingEntryRecording", soundDto);
        }

        public async Task Expand(string selector)
        {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("removeClass", selector, "collapse");
        }

        public async Task Collapse(string selector)
        {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("addClass", selector, "collapse");
        }
    }
}