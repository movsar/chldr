using domain.Interfaces;
using domain.Models;
using Microsoft.JSInterop;
using Newtonsoft.Json;

namespace chldr_blazor_server.Services
{
    public class LocalStorageSettingsService : ISettingsService
    {
        private IJSRuntime _jsRuntime;
        private IExceptionHandler _exceptionHandler;

        public LocalStorageSettingsService(IJSRuntime jsRuntime, IExceptionHandler exceptionHandler)
        {
            _jsRuntime = jsRuntime;
            _exceptionHandler = exceptionHandler;
        }

        public async Task<T?> GetItem<T>(string key)
        {
            var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key);

            if (json == null)
                return default;
            try
            {

                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                _exceptionHandler.LogDebug(ex.Message);
                return default;
            }
        }

        public async Task RemoveItem(string key)
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
        }

        public async Task SetItem<T>(string key, T value)
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, JsonConvert.SerializeObject(value));
        }
    }
}
