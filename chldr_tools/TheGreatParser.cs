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

namespace chldr_maintenance
{
    internal class TheGreatParser
    {
        internal static void ParseEverything()
        {
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

                var matches = Regex.Matches(translationText, @"\[.*?\]");
                if (matches.Count() == 0)
                {
                    TrySetEntryType(entry, translationText!);
                    AddTranslations(entry, translationText, rusNotes, cheNotes);

                    // TODO: Add word
                    // context.Entries.Add(entry);
                    continue;
                }

                var content = legacyEntry.Phrase;

                // Add with multiple forms
                foreach (Match match in matches)
                {
                    // deal with synonyms

                    var t = translationText.Replace(match.Value, "").Trim();

                    var subEntryStartIndex = t.IndexOf("[");
                    if (subEntryStartIndex != -1)
                    {
                        t = t.Substring(0, subEntryStartIndex).Trim();
                    }

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

                    int grammaticalClass = ParseGrammatocalClass(rawForms);

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
                        AddVerbDetails(entry, forms, t);
                    }
                    else if (formsCount > 4)
                    {
                        // Noun
                        AddNounDetails(entry, forms, t);
                    }
                    else
                    {
                        t = t + "(" + rawForms + ")";
                    }


                    TrySetEntryType(entry, t!);
                    AddTranslations(entry, t, rusNotes, cheNotes);

                    // TODO: Add word
                    // context.Entries.Add(entry);

                    InsertRelatedWords(entry, t, forms);

                    // Update Id for the next subEntry
                    entry.EntryId = Guid.NewGuid().ToString();
                }
            }
        }

        private static void InsertRelatedWords(SqlEntry parentEntry, string translation, List<string> forms)
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
        private static int ParseGrammatocalClass(string rawForms)
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

        private static void AddNounDetails(SqlEntry entry, List<string> forms, string translationText)
        {
            var nounDetails = new NounDetails();
            entry.Subtype = (int)WordType.Noun;

            nounDetails.Case = Case.Absolutive;
            nounDetails.NumeralType = !translationText.Contains("только мн") ? NumeralType.Singular : NumeralType.Plural;
            // Add Forms
        }

        private static void AddVerbDetails(SqlEntry entry, List<string> forms, string translationText)
        {
            var verbDetails = new VerbDetails();
            entry.Subtype = (int)WordType.Verb;

            var model = EntryModel.FromEntity(entry, entry.Source, entry.Translations, entry.Sounds);
            var dto = EntryDto.FromModel(model);

            var count = forms.Count();

            var main = entry.Content;
            var presentSimple = forms[0];
            var witnessedPast = forms[1];
            var pastPerfect = forms[2];

            if (count == 3)
            {
                return;
            }

            var futureSimple = forms[3];

            if (count == 4)
            {
                return;
            }

            var futureContinuous = forms[4];
            // main - inifinitive
            // first - present simple
            // second - witnessed past
            // third - past perfect
            // fourth - future simple
            // fifth - future continuous

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
