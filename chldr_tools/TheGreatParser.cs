using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models.Words;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums.WordDetails;
using chldr_data.Enums;
using chldr_data.remote.SqlEntities;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using chldr_data.local.Services;
using chldr_data.remote.Services;
using chldr_utils;
using Microsoft.EntityFrameworkCore;
using chldr_data;
using Newtonsoft.Json;
using chldr_data.Interfaces;
using chldr_data.Helpers;
using chldr_maintenance.Entities;

namespace chldr_maintenance
{
    internal class TheGreatParser
    {
        private static SqlContext _context;
        internal static void ParseEverything()
        {
            _context = GetSqlContext();

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

                // main - inifinitive
                entry.Content = legacyEntry.Phrase;

                entry.Type = (int)EntryType.Word;
                entry.Source = _context.Sources.First(s => s.Name.Equals("Maciev"));
                entry.SourceId = entry.Source.SourceId;
                entry.Rate = 10;
                entry.EntryId = Guid.NewGuid().ToString();
                entry.User = _context.Users.First();
                entry.UserId = entry.User.UserId;

                var matches = Regex.Matches(translationText, @"\[.*?\]");
                if (matches.Count() == 0)
                {
                    TrySetEntryType(entry, translationText!);
                    AddTranslations(entry, translationText, rusNotes, cheNotes);

                    _context.Entries.Add(entry);
                    //_context.SaveChanges();
                    continue;
                }

                // Deal with synonyms
                foreach (Match match in matches)
                {
                    // Create a new object for the synonym
                    var subEntry = CreateSubEntry(entry, legacyEntry.Phrase!);

                    var t = translationText.Replace(match.Value, "").Trim();

                    var subEntryStartIndex = t.IndexOf("[");
                    if (subEntryStartIndex != -1)
                    {
                        t = t.Substring(0, subEntryStartIndex).Trim();
                    }

                    TrySetEntryType(subEntry, t!);
                    AddTranslations(subEntry, t, rusNotes, cheNotes);
                    _context.Entries.Add(subEntry);

                    try
                    {
                        translationText = translationText.Substring(match.Value.Length + t.Length + 1).Trim();
                    }
                    catch (Exception ex)
                    {

                    }

                    var rawForms = match.Value.Replace("[", "").Replace("]", "").Trim();

                    var mnMatch = Regex.Match(t, @"мн..*\W.*?(?=\])");
                    if (mnMatch.Success)
                    {
                        if (!t.StartsWith(mnMatch.Value))
                        {
                            break;
                        }

                        if (!rawForms.Contains(mnMatch.Value))
                        {
                            rawForms = rawForms + "; " + mnMatch.Value;
                        }
                        t = t.Replace(mnMatch.Value, "").Substring(1);
                    }

                    int grammaticalClass = ParseGrammaticalClass(rawForms);

                    var forms = Regex.Matches(rawForms, @"[1ӀӏА-я]{2,}").Select(m => m.Value).ToList();
                    var isVerb = forms.Contains("или");

                    forms.RemoveAll(form => form.Equals("или") || form.Equals("ду") || form.Equals("ву") || form.Equals("йу") || form.Equals("бу") || form.Equals("мн"));
                    var formsCount = forms.Count();

                    if (!isVerb)
                    {
                        isVerb = formsCount == 3 || formsCount == 4;
                    }

                    if (isVerb)
                    {
                        // Verb
                        subEntry.Subtype = (int)WordType.Verb;
                        AddVerbForms(subEntry, forms, t);
                    }
                    else if (formsCount > 4)
                    {
                        // Noun
                        subEntry.Subtype = (int)WordType.Noun;
                        AddNounForms(subEntry, forms, t);
                    }
                    else
                    {
                        var classPartPattern = @"\s\w$";
                        var classLiteral = Regex.Match(rawForms, classPartPattern);
                        if (classLiteral.Success)
                        {
                            // Set class
                            var classLiteralTrimmed = classLiteral.Value.Trim();
                        }

                        var singularVsPluralPattern = @"мн\W";
                        var plurality = Regex.Match(rawForms, singularVsPluralPattern);
                        if (plurality.Success)
                        {
                            // Set plural
                        }

                        t = t + "(" + rawForms + ")";
                    }

                    //_context.SaveChanges();

                    // Update Id for the next subEntry
                    entry.EntryId = Guid.NewGuid().ToString();
                }
            }
        }

        private static void InsertRelatedWords(SqlEntry parentEntry, string translation)
        {

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

        private static int ParseGrammaticalClass(string rawForms)
        {
            string pattern = @"(?<=\W)(в|б|й|д)(?=\W)";
            var matches = Regex.Matches(rawForms, pattern);
            var count = matches.Count();

            if (count == 2)
            {
                // TODO
                return 2;
            }
            if (count == 1)
            {
                // TODO
                return 1;
            }

            return 0;
        }

        private static void AddNounForms(SqlEntry entry, List<string> forms, string translationText)
        {
            var count = forms.Count();

            var nounDetails = new NounDetails();
            nounDetails.NumeralType = !translationText.Contains("только мн") ? NumeralType.Singular : NumeralType.Plural;

            // Родительный
            var genitive = CreateSubEntry(entry, forms[0]);
            nounDetails.Case = Case.Genitive;
            genitive.Details = JsonConvert.SerializeObject(nounDetails);
            _context.Entries.Add(genitive);

            // Дательный
            var dative = CreateSubEntry(entry, forms[1]);
            nounDetails.Case = Case.Dative;
            dative.Details = JsonConvert.SerializeObject(nounDetails);
            _context.Entries.Add(dative);

            // Эргативный
            var ergative = CreateSubEntry(entry, forms[2]);
            nounDetails.Case = Case.Ergative;
            ergative.Details = JsonConvert.SerializeObject(nounDetails);
            _context.Entries.Add(ergative);

            // Местный
            var locative = CreateSubEntry(entry, forms[3]);
            nounDetails.Case = Case.Locative;
            locative.Details = JsonConvert.SerializeObject(nounDetails);
            _context.Entries.Add(locative);

            // Именительный во множественном 
            var pluralAbsolutive = CreateSubEntry(entry, forms[4]);
            nounDetails.Case = Case.Absolutive;
            nounDetails.NumeralType = NumeralType.Plural;
            pluralAbsolutive.Details = JsonConvert.SerializeObject(nounDetails);
            _context.Entries.Add(pluralAbsolutive);
        }

        private static SqlEntry CreateSubEntry(SqlEntry entry, string content)
        {
            return new SqlEntry()
            {
                EntryId = Guid.NewGuid().ToString(),
                ParentEntryId = entry.EntryId,
                SourceId = entry.SourceId,
                UserId = entry.UserId,
                Content = content,
                Details = entry.Details,
                Rate = entry.Rate,
                Type = entry.Type,
                Subtype = entry.Subtype,
            };
        }

        private static void AddVerbForms(SqlEntry entry, List<string> forms, string translationText)
        {
            var count = forms.Count();

            // first - present simple
            var presentSimple = CreateSubEntry(entry, forms[0]);
            var verbDetails = new VerbDetails();
            verbDetails.Tense = VerbTense.PresentSimple;
            presentSimple.Details = JsonConvert.SerializeObject(verbDetails);
            _context.Entries.Add(presentSimple);

            // second - witnessed past
            var witnessedPast = CreateSubEntry(entry, forms[1]);
            verbDetails = new VerbDetails();
            verbDetails.Tense = VerbTense.PastWitnessed;
            presentSimple.Details = JsonConvert.SerializeObject(verbDetails);
            _context.Entries.Add(witnessedPast);

            // third - past perfect
            var pastPerfect = CreateSubEntry(entry, forms[2]);
            verbDetails = new VerbDetails();
            verbDetails.Tense = VerbTense.PastPerfect;
            presentSimple.Details = JsonConvert.SerializeObject(verbDetails);
            _context.Entries.Add(pastPerfect);

            if (count == 3)
            {
                return;
            }

            // fourth - future simple
            var futureSimple = CreateSubEntry(entry, forms[3]);
            verbDetails = new VerbDetails();
            verbDetails.Tense = VerbTense.FutureSimple;
            presentSimple.Details = JsonConvert.SerializeObject(verbDetails);
            _context.Entries.Add(futureSimple);

            if (count == 4)
            {
                return;
            }

            // fifth - future continuous
            var futureContinuous = CreateSubEntry(entry, forms[4]);
            verbDetails = new VerbDetails();
            verbDetails.Tense = VerbTense.FutureContinous;
            presentSimple.Details = JsonConvert.SerializeObject(verbDetails);
            _context.Entries.Add(futureContinuous);
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
            entry.Translations.Add(rusTranslation);

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
                entry.Translations.Add(cheTranslation);
            }
        }
        private static void TrySetEntryType(SqlEntry entry, string translationText)
        {

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
