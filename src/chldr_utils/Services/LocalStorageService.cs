﻿using Microsoft.JSInterop;
using System.Text.Json;

namespace chldr_utils.Services
{
    public interface ILocalStorageService
    {
        Task<T> GetItem<T>(string key);
        Task SetItem<T>(string key, T value);
        Task RemoveItem(string key);
    }

    public class LocalStorageService : ILocalStorageService
    {
        private IJSRuntime _jsRuntime;
        private ExceptionHandler _exceptionHandler;

        public LocalStorageService(IJSRuntime jsRuntime, ExceptionHandler exceptionHandler)
        {
            _jsRuntime = jsRuntime;
            _exceptionHandler = exceptionHandler;
        }

        public async Task<T> GetItem<T>(string key)
        {
            var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key);

            if (json == null)
                return default;
            try
            {

                return JsonSerializer.Deserialize<T>(json);
            }
            catch (Exception ex)
            {
                _exceptionHandler.LogDebug(ex.Message);
                return default;
            }
        }

        public async Task SetItem<T>(string key, T value)
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, JsonSerializer.Serialize(value));
        }

        public async Task RemoveItem(string key)
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
        }
    }
}
