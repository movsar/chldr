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
using chldr_data.Enums.WordDetails;
using System.Configuration;
using chldr_data.DatabaseObjects.Models.Words;
using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using GraphQL;
using chldr_data.DatabaseObjects.Interfaces;

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
                var rusNotes = Regex.Match(rawNotes, @"(?<=RUS:).*?(?=Ψ)").Value ?? string.Empty;
                var cheNotes = Regex.Match(rawNotes, @"(?<=CHE:).*?(?=Ψ)").Value ?? string.Empty;

                var translationText = legacyTranslations
                    .Where(t => t.LanguageCode!.Equals("RUS") && t.PhraseId.Equals(legacyEntry.Id))
                    .DistinctBy(t => t.LanguageCode).Single().Translation!;

                var entry = new SqlEntry();
                entry.Content = legacyEntry.Phrase;
                entry.Type = (int)EntryType.Word;
                entry.Source = context.Sources.First(s => s.Name.Equals("Maciev"));
                entry.SourceId = entry.Source.SourceId;
                entry.Rate = 10;
                entry.EntryId = Guid.NewGuid().ToString();
                entry.User = context.Users.First();
                entry.UserId = entry.User.UserId;

                SetPartOfSpeech(entry, translationText!);

                // Set entry details, add related forms
                var match = Regex.Match(translationText, @"\[.*?\]");
                if (!match.Success)
                {
                    // TODO: Add word

                    AddTranslation(entry, legacyEntry.Phrase!, rusNotes, cheNotes);

                    continue;
                }

                var nounDetails = new NounDetails();


                var commasCount = match.Value.Count(c => c.Equals(','));

                if (commasCount == 2 || commasCount == 3)
                {
                    entry.Subtype = (int)WordType.Verb;

                    var model = EntryModel.FromEntity(entry, entry.Source, entry.Translations, entry.Sounds);
                    var dto = EntryDto.FromModel(model);
                    var forms = Regex.Matches(match.Value, @"[1ӀӏА-я]{2,}").Select(m => m.Value).ToList();

                    if (forms.Contains("или"))
                    {

                    }

                    forms.RemoveAll(form => form.Equals("или") || form.Equals("ду") || form.Equals("ву") || form.Equals("йу") || form.Equals("бу"));

                    foreach (var form in forms)
                    {


                        // Insert related word
                    }
                }
                else if (commasCount >= 4 || commasCount <= 6)
                {
                    entry.Subtype = (int)WordType.Noun;

                    nounDetails.Case = Case.Absolutive;
                    nounDetails.NumeralType = !translationText.Contains("только мн") ? NumeralType.Singular : NumeralType.Plural;

                    // Add Forms
                }


            }
        }

        private static void AddTranslation(SqlEntry entry, string content, string rusNotes, string cheNotes)
        {
            var rusTranslation = new SqlTranslation()
            {
                TranslationId = Guid.NewGuid().ToString(),
                Content = content,
                Entry = entry,
                EntryId = entry.EntryId,
                LanguageCode = "RUS",
                Notes = rusNotes,
                User = entry.User,
                UserId = entry.User.UserId,
                Rate = 10,
            };
            // TODO: Add

            if (!string.IsNullOrEmpty(cheNotes))
            {
                var cheTranslation = new SqlTranslation()
                {
                    TranslationId = Guid.NewGuid().ToString(),
                    Content = cheNotes,
                    Entry = entry,
                    EntryId = entry.EntryId,
                    LanguageCode = "RUS",
                    User = entry.User,
                    UserId = entry.User.UserId,
                    Rate = 10,
                };

                // TODO: Add
            }
        }
        private static void SetPartOfSpeech(SqlEntry entry, string input)
        {
            if (TestFor(input, "сущ"))
            {
                entry.Subtype = (int)WordType.Noun;
            }
            else if (TestFor(input, "гл"))
            {
                entry.Subtype = (int)WordType.Verb;
            }
            else if (TestFor(input, "прил"))
            {
                entry.Subtype = (int)WordType.Adjective;
            }
            else if (TestFor(input, "мест"))
            {
                entry.Subtype = (int)WordType.Pronoun;
            }
            else if (TestFor(input, "частица"))
            {
                entry.Subtype = (int)WordType.Conjunction;
            }
            else if (TestFor(input, "частица"))
            {
                entry.Subtype = (int)WordType.Particle;
            }
            else if (TestFor(input, "прич"))
            {
                entry.Subtype = (int)WordType.Verb;
            }
            else if (TestFor(input, "неопр"))
            {
                entry.Subtype = (int)WordType.Verb;
            }
            else if (TestFor(input, "нареч"))
            {
                entry.Subtype = (int)WordType.Adverb;
            }
            else if (TestFor(input, "числ"))
            {
                entry.Subtype = (int)WordType.Numeral;
            }
            else if (TestFor(input, "масд"))
            {
                entry.Subtype = (int)WordType.Masdar;
            }
        }
    }
}