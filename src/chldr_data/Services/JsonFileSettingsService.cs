using chldr_data.Interfaces;
using chldr_utils;
using chldr_utils.Services;
using System.Text.Json;

namespace chldr_data.Services
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

        public async Task<T> GetItem<T>(string key)
        {
            try
            {
                if (!File.Exists(_filePath))
                    return default;

                var json = await File.ReadAllTextAsync(_filePath);
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

        public async Task SetItem<T>(string key, T value)
        {
            try
            {
                Dictionary<string, string> settings;

                if (File.Exists(_filePath))
                {
                    var json = await File.ReadAllTextAsync(_filePath);
                    settings = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
                }
                else
                {
                    settings = new Dictionary<string, string>();
                }

                settings[key] = JsonSerializer.Serialize(value);
                var updatedJson = JsonSerializer.Serialize(settings);
                await File.WriteAllTextAsync(_filePath, updatedJson);
            }
            catch (Exception ex)
            {
                _exceptionHandler.LogDebug(ex.Message);
            }
        }

        public async Task RemoveItem(string key)
        {
            try
            {
                if (!File.Exists(_filePath))
                    return;

                var json = await File.ReadAllTextAsync(_filePath);
                var settings = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

                if (settings != null && settings.Remove(key))
                {
                    var updatedJson = JsonSerializer.Serialize(settings);
                    await File.WriteAllTextAsync(_filePath, updatedJson);
                }
            }
            catch (Exception ex)
            {
                _exceptionHandler.LogDebug(ex.Message);
            }
        }
    }
}
