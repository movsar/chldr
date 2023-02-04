using Realms;

namespace chldr_data.Interfaces
{
    public interface IRealmService
    {
        Realm GetDatabase();
        void InitializeConfiguration();

    }
}
