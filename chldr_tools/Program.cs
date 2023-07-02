using chldr_data.Services;
using chldr_utils.Services;
using chldr_utils;
using chldr_data.local.Services;
using chldr_data.local.RealmEntities;
using Realms;
using chldr_data.remote.Services;
using chldr_data.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using chldr_data;
using chldr_maintenance;
using System.Text.RegularExpressions;
using chldr_data.remote.SqlEntities;
using chldr_data.Enums;
using System.Diagnostics;

namespace chldr_tools
{
    internal class Program
    {
        private static FileService _fileService;
        private static ExceptionHandler _exceptionHandler;
        private static EnvironmentService _environmentService;
        private static RequestService _requestSender;
        private static SyncService _syncService;

        static Program()
        {
            _fileService = new FileService(AppContext.BaseDirectory);
            _exceptionHandler = new ExceptionHandler(_fileService);
            _environmentService = new EnvironmentService(chldr_shared.Enums.Platforms.Windows, true);

            var graphQlClient = new GraphQLClient(_exceptionHandler, _environmentService);
            _requestSender = new RequestService(graphQlClient);
        }

        static void ContentUpdater(Realm realmDatabase, SqlContext sqlDatabase)
        {

            var entries = realmDatabase.All<RealmEntry>();
            int count = 0;
            Console.WriteLine("Starting the update ...");

            foreach (var entry in entries)
            {
                var sqlEntry = sqlDatabase.Entries.FirstOrDefault(e => e.EntryId.Equals(entry.EntryId));
                if (sqlEntry == null)
                {
                    Console.Write("failed updating: ");
                    Console.WriteLine(entry.EntryId);
                    continue;
                }

                sqlEntry.Content = entry.Content;
                sqlEntry.Subtype = entry.Subtype;
                if (count % 100 == 0 || count > entries.Count() - 50)
                {
                    Console.WriteLine($"Saving {count}");
                    sqlDatabase.SaveChanges();
                }
                count++;
            }
            Console.WriteLine("Saving ...");
            Console.WriteLine($"Successfully saved {count} entries");
        }

        static void LanguageCodeLocal(Realm realmDatabase)
        {
            var translations = realmDatabase.All<RealmTranslation>();
            int count = 0;
            Console.WriteLine("Starting the update ...");
            var total = translations.Count();
            realmDatabase.Write(() =>
            {

                foreach (var translation in translations)
                {
                    //translation.LanguageCode = translation.Language.Code;
                    if (count % 100 == 0 || count > total - 50)
                    {
                        Console.WriteLine($"Processed {count}");
                    }
                    count++;
                }
            });

            Console.WriteLine("Saving ...");
            Console.WriteLine($"Successfully saved {count} entries");

            realmDatabase.WriteCopy(new RealmConfiguration("new.realm"));
        }

        static RealmDataProvider GetRealmDataProvider()
        {
            var localRealmContext = new RealmDataProvider(_fileService, _exceptionHandler, _syncService);
            localRealmContext.Initialize();

            return localRealmContext;
        }

        static SqlDataProvider GetSqlDataProvider()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                        { "ConnectionStrings:SqlContext",  Constants.TestDatabaseConnectionString}
                }!).Build();

            var sqlContext = new SqlDataProvider(_fileService, _exceptionHandler, configuration);
            sqlContext.Initialize();
            return sqlContext;
        }

        static SqlContext GetSqlContext()
        {
            var options = new DbContextOptionsBuilder<SqlContext>().UseMySQL(Constants.TestDatabaseConnectionString).Options;
            var context = new SqlContext(options);

            return context;
        }

        static bool TestFor(string str, string needle)
        {
            string pattern = $"(?<={needle}\\W?\\s?)[1ӀӏА-яA-z]+";
            return Regex.Match(str, pattern, RegexOptions.CultureInvariant).Success;
        }

        static void SetEntryContents(SqlEntry entry, string translation)
        {

            // Часть речи
            // entry.Subtype = 

            if (TestFor("понуд", translation))
            {

            }
            else if (TestFor("потенц", translation))
            {

            }
            else if (TestFor("прил", translation))
            {

            }
            else if (TestFor("масд", translation))
            {

            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            var context = GetSqlContext();

            context.SaveChanges();

            using var legacyContext = new EfContext();

            var legacyEntries = legacyContext.LegacyPhraseEntries.Where(phrase => phrase.Source == "MACIEV");
            var legacyEntryIndeces = legacyEntries.Select(e => e.Id).ToArray();
            var legacyTranslations = legacyContext.LegacyTranslationEntries.Where(t => legacyEntryIndeces.Contains(t.PhraseId)).ToList();

            foreach (var legacyEntry in legacyEntries)
            {
                var rawNotes = legacyEntry.Notes ?? string.Empty;
                var rusNotes = Regex.Match(rawNotes, @"(?<=RUS:).*?(?=Ψ)").ToString();
                var cheNotes = Regex.Match(rawNotes, @"(?<=CHE:).*?(?=Ψ)").ToString();

                var translation = legacyTranslations
                    .Where(t => t.LanguageCode!.Equals("RUS") && t.PhraseId.Equals(legacyEntry.Id))
                    .DistinctBy(t => t.LanguageCode).Single();

                var entry = new SqlEntry();
                entry.Type = (int)EntryType.Word;
                entry.SourceId = context.Sources.First(s => s.Name.Equals("Maciev")).SourceId;
                entry.Rate = 10;
                entry.EntryId = Guid.NewGuid().ToString();
                entry.UserId = context.Users.First().UserId;

                SetEntryContents(entry, translation.Translation!);

                // Set rawcontents
                // entry.RawContents = 
            }
        }
    }
}