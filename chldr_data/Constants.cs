using chldr_utils;

namespace chldr_data
{
    public static class Constants
    {
        public const string DevApiHost = "https://localhost:7065";
        public const string ProdApiHost = "https://api.nohchiyn-mott.com";

        public const int ChangeSetsToApply = 1000;
        public const string DefaultUserId = "63a816205d1af0e432fba6dd";
        public const string TestDatabaseConnectionString = "server=localhost;port=3306;database=chldr;user=admin;password=password;charset=utf8mb4";

        public const int RealmSchemaVersion = 16;
    }
}
