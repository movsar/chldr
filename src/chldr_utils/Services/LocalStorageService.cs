using System.Text.Json;
using Xamarin.Essentials;

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
        private ExceptionHandler _exceptionHandler;

        public LocalStorageService(ExceptionHandler exceptionHandler)
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