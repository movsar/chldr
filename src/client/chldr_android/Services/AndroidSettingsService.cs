using Android.Content;
using domain.Interfaces;
using System.Text.Json;

namespace chldr_android.Services
{
    public class AndroidSettingsService : ISettingsService
    {
        private readonly ISharedPreferences _preferences;

        public AndroidSettingsService(Context context)
        {
            _preferences = context.GetSharedPreferences("appSettings", FileCreationMode.Private)!;
        }

        public T? GetItem<T>(string key)
        {
            var json = _preferences.GetString(key, null);
            return json == null ? default : JsonSerializer.Deserialize<T>(json);
        }

        public void SetItem<T>(string key, T value)
        {
            var editor = _preferences.Edit()!;
            var json = JsonSerializer.Serialize(value);
            editor.PutString(key, json);
            editor.Apply();
        }

        public void RemoveItem(string key)
        {
            var editor = _preferences.Edit()!;
            editor.Remove(key);
            editor.Apply();
        }
    }
}
