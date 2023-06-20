﻿using chldr_data.DatabaseObjects.Dtos;
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
    // This class provides an example of how JavaScript functionality can be wrapped
    // in a .NET class for easy consumption. The associated JavaScript module is
    // loaded on demand when first needed.
    //
    // This class can be registered as scoped DI service and then injected into Blazor
    // components for use.

    public class JsInteropService : IAsyncDisposable
    {
        private readonly Lazy<Task<IJSObjectReference>> moduleTask;
        private readonly IStringLocalizer<AppLocalizations> _localizer;

        public static Action<string> OnRemoveAudio { get; set; }

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
            if (moduleTask.IsValueCreated)
            {
                var module = await moduleTask.Value;
                await module.DisposeAsync();
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

            return base64String;
        }


        [JSInvokable]
        public static void WordEdit_RemoveSound_ClickHandler(string recordingId)
        {
            OnRemoveAudio?.Invoke(recordingId);
        }

        public async Task AddExistingEntryRecording(SoundDto soundDto)
        {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("addExistingEntryRecording", soundDto);
        }
    }
}