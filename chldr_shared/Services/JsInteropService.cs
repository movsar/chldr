using Microsoft.JSInterop;
using System.Diagnostics;

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
    }
}