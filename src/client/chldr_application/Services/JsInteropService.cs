using core.DatabaseObjects.Dtos;
using core.Resources.Localizations;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Diagnostics;
using System.Text.Json;

namespace chldr_application.Services
{
    public class JsInteropService : IAsyncDisposable
    {
        private readonly Lazy<Task<IJSObjectReference>> moduleTask;
        private readonly IStringLocalizer<AppLocalizations> _localizer;

        public static Action<string> OnRemoveAudio { get; set; }
        public static Action<string> OnPromoteAudio { get; set; }

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

        public async Task<string?> StopRecording()
        {
            var module = await moduleTask.Value;
            var jsonDocument = await module.InvokeAsync<JsonDocument>("stopRecording");
            if (jsonDocument == null)
            {
                // Happens when recording is too quickly stopped
                return null;
            }

            string recordingId = jsonDocument.RootElement.GetProperty("id").GetString();
            string base64String = jsonDocument.RootElement.GetProperty("data").GetString();
            if (string.IsNullOrEmpty(base64String))
            {
                throw new Exception("Pronunciation data is empty");
            }

            await module.InvokeVoidAsync("showStartButton");
            await module.InvokeAsync<JsonDocument>("addRecordingListItem", recordingId, base64String, false, true);
            
            return base64String;
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