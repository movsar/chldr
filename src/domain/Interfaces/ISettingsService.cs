namespace domain.Interfaces
{
    public interface ISettingsService
    {
        T? GetItem<T>(string key);
        void SetItem<T>(string key, T value);
        void RemoveItem(string key);
    }
}
