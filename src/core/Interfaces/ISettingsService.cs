namespace core.Interfaces
{
    public interface ISettingsService
    {
        Task<T?> GetItem<T>(string key);
        Task SetItem<T>(string key, T value);
        Task RemoveItem(string key);
    }
}
