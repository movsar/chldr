using domain.Interfaces;
using System.Text.Json;

namespace domain.Services
{
    public class JsonFileSettingsService : ISettingsService
    {
        private readonly string _filePath;
        private readonly IExceptionHandler _exceptionHandler;

        public JsonFileSettingsService(IFileService fileService, IExceptionHandler exceptionHandler)
        {
            _exceptionHandler = exceptionHandler;
            _filePath = Path.Combine(fileService.AppDataDirectory, "settings.json");
        }

        public T? GetItem<T>(string key)
        {
            try
            {
                if (!File.Exists(_filePath))
                    return default;

                var json = File.ReadAllText(_filePath);
                var settings = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

                if (settings != null && settings.TryGetValue(key, out var value))
                {
                    return JsonSerializer.Deserialize<T>(value);
                }

                return default;
            }
            catch (Exception ex)
            {
                _exceptionHandler.LogDebug(ex.Message);
                return default;
            }
        }

        public void SetItem<T>(string key, T value)
        {
            try
            {
                Dictionary<string, string> settings;

                if (File.Exists(_filePath))
                {
                    var json = File.ReadAllText(_filePath);
                    settings = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
                }
                else
                {
                    settings = new Dictionary<string, string>();
                }

                settings[key] = JsonSerializer.Serialize(value);
                var updatedJson = JsonSerializer.Serialize(settings);
                File.WriteAllText(_filePath, updatedJson);
            }
            catch (Exception ex)
            {
                _exceptionHandler.LogDebug(ex.Message);
            }
        }

        public void RemoveItem(string key)
        {
            try
            {
                if (!File.Exists(_filePath))
                    return;

                var json = File.ReadAllText(_filePath);
                var settings = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

                if (settings != null && settings.Remove(key))
                {
                    var updatedJson = JsonSerializer.Serialize(settings);
                    File.WriteAllText(_filePath, updatedJson);
                }
            }
            catch (Exception ex)
            {
                _exceptionHandler.LogDebug(ex.Message);
            }
        }
    }
}
