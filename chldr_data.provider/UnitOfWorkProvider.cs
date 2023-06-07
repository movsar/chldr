using chldr_data.Interfaces;
using chldr_data.local.Services;
using chldr_data.Services;

namespace chldr_data.provider
{
    public class UnitOfWorkProvider
    {
        public IUnitOfWork CreateUnitOfWork(bool dataSourceIsRealm = true)
        {
            // Logic to select the data source based on the configuration
            // You can use app settings or any other mechanism to determine the data source

            // Example: Selecting RealmDataProvider for Realm and MySqlDataProvider for MySQL
            if (dataSourceIsRealm)
            {
                return new RealmUnitOfWork();
            }
            else
            {
                return new SqlUnitOfWork();
            }
        }
    }
}