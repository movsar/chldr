namespace domain
{
    public static class Constants
    {
        public const string EncKey = "251:64:10:202:36:166:97:179:3:217:154:66:136:107:161:214:200:33:40:158:5:245:206:113:88:249:185:37:186:71:199:218:114:100:200:175:21:80:21:66:217:200:241:51:98:204:2:164:235:253:147:15:138:173:37:24:219:154:211:245:12:250:74:5";

        public static class DataErrorMessages
        {
            public const string NetworkIsDown = "Network is down";

            public const string AnonymousUser = "Not a registered user";

            public const string AppNotInitialized = "App is not initialized";

            public const string NoUserInfo = "No custom user info is found";
        }

        public const string DevApiHost = "https://localhost:7065";
        public const string ProdApiHost = "https://api.dosham.app";

        public const string ProdFrontHost = "https://dosham.app";

        public const int ChangeSetsToApply = 1000;
        public const int RealmSchemaVersion = 18;

        public const string TestingDatabaseConnectionString = "server=localhost;port=3306;database=test_chldr;user=root;password=password;charset=utf8mb4";
        public const string TestsFileServicePath = "Temp";

        public static int EntriesApproximateCoount = 75000;

        public const int TimeInHrsToAllowForEntryRemoval = 12;

        public const int MaxSoundsPerEntry = 2;
        public const int MaxTranslationsPerEntry = 2;
    }
}
