using chldr_data.Interfaces;
using System.Text.Json;

namespace chldr_utils.Services
{
    public class MauiSettingsService : ISettingsService
    {
        private IExceptionHandler _exceptionHandler;

        public MauiSettingsService
            (IExceptionHandler exceptionHandler)
        {
            _exceptionHandler = exceptionHandler;
        }

        public async Task<T> GetItem<T>(string key)
        {            
            try
            {
                var json = Preferences.Get(key, null);

                if (json == null)
                    return default;

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
            try
            {
                var json = JsonSerializer.Serialize(value);
                Preferences.Set(key, json);
            }
            catch (Exception ex)
            {
                _exceptionHandler.LogDebug(ex.Message);
            }
        }

        public async Task RemoveItem(string key)
        {
            Preferences.Remove(key);
        }
    }
}