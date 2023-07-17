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
using chldr_data.Enums.WordDetails;
using chldr_data.DatabaseObjects.Models.Words;
using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using GraphQL;
using FluentValidation.Internal;

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
        static SqlDataProvider GetSqlDataProvider()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                        { "ConnectionStrings:SqlContext",  Constants.LocalDatabaseConnectionString}
                }!).Build();

            var sqlContext = new SqlDataProvider(_fileService, _exceptionHandler, configuration);
            sqlContext.Initialize();
            return sqlContext;
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

        private static void TrySetParentId(SqlEntry entry, string translation)
        {
            // Links must be established after all the words are added
            var translationText = translation?.ToLower();

            string[] prefixesToSearch = {
                "см",
                "понуд.? от",
                "потенц.? от",
                "прил.? к",
                "масд.? от"
            };

            foreach (var prefix in prefixesToSearch)
            {

                string pattern = $"(?<={prefix}\\W?\\s?)[1ӀӏА-яA-z]+";
                var match = Regex.Match(translationText, pattern, RegexOptions.CultureInvariant);

                if (!match.Success)
                {
                    return;
                }

                var parentWord = match.ToString();
            }
        }


        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            //TheGreatParser.ParseEverything();
        }
    }
}