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

                var match = Regex.Match(translationText, @"\[.*?\]");

                translationText = translationText.Substring(translationText.IndexOf("]")+1).Trim();

                FirstGate(entry, translationText!, rusNotes, cheNotes);

                // Set entry details, add related forms

                if (!match.Success)
                {
                    // TODO: Add word

                    continue;
                }
                var rawForms = match.Value;

                var forms = Regex.Matches(rawForms, @"[1ӀӏА-я]{2,}").Select(m => m.Value).ToList();
                var isVerb = forms.Contains("или");

                forms.RemoveAll(form => form.Equals("или") || form.Equals("ду") || form.Equals("ву") || form.Equals("йу") || form.Equals("бу") || form.Equals("мн"));
                var formsCount = forms.Count();
              
                if (!isVerb)
                {
                    isVerb = formsCount == 2 || formsCount == 3;
                }

                if (isVerb)
                {
                    // Verb
                    AddVerbDetails(entry, forms, translationText);
                }
                else if (formsCount > 0)
                {
                    // Noun
                    AddNounDetails(entry, forms, translationText);
                }
                else
                {

                }
            }
        }

        private static void AddNounDetails(SqlEntry entry, List<string> forms, string rawTranslation)
        {
            var nounDetails = new NounDetails();
            entry.Subtype = (int)WordType.Noun;

            nounDetails.Case = Case.Absolutive;
            nounDetails.NumeralType = !rawTranslation.Contains("только мн") ? NumeralType.Singular : NumeralType.Plural;
            // Add Forms
        }

        private static void AddVerbDetails(SqlEntry entry, List<string> forms, string rawTranslation)
        {
            var verbDetails = new VerbDetails();
            entry.Subtype = (int)WordType.Verb;

            var model = EntryModel.FromEntity(entry, entry.Source, entry.Translations, entry.Sounds);
            var dto = EntryDto.FromModel(model);

            foreach (var form in forms)
            {

                // Insert related word
            }
        }

        private static void AddTranslations(SqlEntry entry, string content, string rusNotes, string cheNotes)
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
        private static void FirstGate(SqlEntry entry, string translationText, string rusNotes, string cheNotes)
        {
            AddTranslations(entry, translationText, rusNotes, cheNotes);

            if (TestFor(translationText, "сущ"))
            {
                entry.Subtype = (int)WordType.Noun;
            }
            else if (TestFor(translationText, "гл"))
            {
                entry.Subtype = (int)WordType.Verb;
            }
            else if (TestFor(translationText, "прил"))
            {
                entry.Subtype = (int)WordType.Adjective;
            }
            else if (TestFor(translationText, "мест"))
            {
                entry.Subtype = (int)WordType.Pronoun;
            }
            else if (TestFor(translationText, "частица"))
            {
                entry.Subtype = (int)WordType.Conjunction;
            }
            else if (TestFor(translationText, "частица"))
            {
                entry.Subtype = (int)WordType.Particle;
            }
            else if (TestFor(translationText, "прич"))
            {
                entry.Subtype = (int)WordType.Verb;
            }
            else if (TestFor(translationText, "неопр"))
            {
                entry.Subtype = (int)WordType.Verb;
            }
            else if (TestFor(translationText, "нареч"))
            {
                entry.Subtype = (int)WordType.Adverb;
            }
            else if (TestFor(translationText, "числ"))
            {
                entry.Subtype = (int)WordType.Numeral;
            }
            else if (TestFor(translationText, "масд"))
            {
                entry.Subtype = (int)WordType.Masdar;
            }
        }
    }
}