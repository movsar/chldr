using GraphQL;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Reflection.Metadata;

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
        public static Action<string> OnRemoveAudio { get; set; }
        
        public JsInteropService(IJSRuntime jsRuntime)
        {
            moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./_content/chldr_ui/jsInterop.js").AsTask());
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

        public async Task StartRecording()
        {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("startRecording");
        }

        public async Task<byte[]?> StopRecording()
        {
            var module = await moduleTask.Value;
            var result = await module.InvokeAsync<string>("stopRecording");
            var deserializedResult = JsonConvert.DeserializeObject<(int recordingIndex, string base64String)>(result);
            int recordingIndex = deserializedResult.recordingIndex;
            string base64String = deserializedResult.base64String;     //byte[] recording = result.base64String;
            //int recordingIndex = result.recordingIndex;
            ////if (string.IsNullOrEmpty(base64String))
            //{
            //    return null;
            //}
            //var recording = Convert.FromBase64String(base64String);

            return null;
        }

        [JSInvokable]
        public static void WordEdit_RemoveSound_ClickHandler(string recordingId)
        {
            OnRemoveAudio?.Invoke(recordingId);
        }
    }
}