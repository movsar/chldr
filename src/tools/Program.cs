﻿using core.Services;
using chldr_utils.Services;
using chldr_utils;
using Realms;
using System.Text.RegularExpressions;
using chldr_domain.Services;
using core.Enums;
using api_domain.Services;
using chldr_domain.RealmEntities;
using api_domain.SqlEntities;
using core.Interfaces;
using core.Models;
using api_domain;

namespace chldr_tools
{
    internal class Program
    {
        private static IFileService _fileService;
        private static IExceptionHandler _exceptionHandler;
        private static EnvironmentService _environmentService;
        private static RequestService _requestService;
        private static SyncService _syncService;

        static Program()
        {
            _fileService = new FileService(AppContext.BaseDirectory);
            _exceptionHandler = new ExceptionHandler(_fileService);
            _environmentService = new EnvironmentService(Platforms.Windows, true);

            var graphQlClient = new GraphQLClient(_exceptionHandler, _environmentService, null);
            _requestService = new RequestService(graphQlClient);

            TrimDownTheSqlDatabaseForTesting();
        }

        private static void TrimDownTheSqlDatabaseForTesting()
        {
            var sqlDataProvider = GetSqlDataProvider();
            //var randomEntries = unitOfWork.Entries.GetRandomsAsync(100);
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
            var connectionString = "My Mega Connection String";

            //var sqlContext = new SqlDataProvider(_fileService, _exceptionHandler, connectionString);
            //sqlContext.Initialize();

            return null;
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
            //var localRealmContext = new RealmDataProvider(_fileService, _exceptionHandler, _requestService, _syncService);
            //localRealmContext.Initialize();

            return null;
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