using core.DatabaseObjects.Models.Words;
using core.Enums.WordDetails;
using core.Enums;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Diagnostics;
using api_domain.Services;
using api_domain.SqlEntities;
using api_domain;

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

            var transaction = _context.Database.BeginTransaction();

            try
            {
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
                    entry.User = _context.Users.First() as SqlUser;
                    entry.UserId = entry.User.Id;

                    var matches = Regex.Matches(translationText, @"\[.*?\]");
                    if (matches.Count() == 0)
                    {
                        TrySetEntryType(entry, translationText!);
                        AddTranslations(entry, translationText, rusNotes, cheNotes);

                        _context.Entries.Add(entry);
                        _context.SaveChanges();
                        continue;
                    }


                    // Deal with synonyms
                    for (int i = 0; i < matches.Count; i++)
                    {
                        Match match = matches[i];

                        var subEntry = (i == 0) ? entry : CreateSubEntry(entry, legacyEntry.Phrase!);

                        var t = translationText.Replace(match.Value, "").Trim();

                        var subEntryStartIndex = t.IndexOf("[");
                        if (subEntryStartIndex != -1)
                        {
                            t = t.Substring(0, subEntryStartIndex).Trim();
                        }

                        TrySetEntryType(subEntry, t!);
                        AddTranslations(subEntry, t, rusNotes, cheNotes);
                        _context.Entries.Add(subEntry);

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

                        if (string.IsNullOrWhiteSpace(t) || string.IsNullOrWhiteSpace(legacyEntry.Phrase))
                        {
                            Debug.WriteLine($"DBG: {entry.Content} : {translationText}");
                        }

                        int[] grammaticalClasses = ParseGrammaticalClass(rawForms);

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
                            var verbDetails = new VerbDetails()
                            {
                                Tense = VerbTense.Infinitive,
                                NumeralType = IsPlural(rawForms) ? NumeralType.Plural : NumeralType.Singular,
                            };

                            bool transitiveness = translationText.Contains("объект в ");
                            bool inTransitiveness = translationText.Contains("субъект в ");
                            if (transitiveness)
                            {
                                verbDetails.Transitiveness = Transitiveness.Transitive;
                            }
                            else if (inTransitiveness)
                            {
                                verbDetails.Transitiveness = Transitiveness.Intransitive;
                            }

                            if (grammaticalClasses.Length == 1)
                            {
                                verbDetails.Class = grammaticalClasses[0];
                            }

                            subEntry.Details = JsonConvert.SerializeObject(verbDetails);

                            AddVerbForms(subEntry, forms, t, grammaticalClasses, verbDetails);
                        }
                        else if (grammaticalClasses.Length == 2)
                        {
                            subEntry.Subtype = (int)WordType.Pronoun;
                            var pronounDetails = new PronounDetails()
                            {
                                Case = Case.Absolutive,
                            };

                            if (grammaticalClasses.Length > 0)
                            {
                                pronounDetails.Classes.AddRange(grammaticalClasses);
                            }

                            subEntry.Details = JsonConvert.SerializeObject(pronounDetails);

                            AddPronounForms(subEntry, forms, t, grammaticalClasses, pronounDetails);
                        }
                        else if (formsCount > 4)
                        {
                            // Noun
                            subEntry.Subtype = (int)WordType.Noun;
                            var nounDetails = new NounDetails()
                            {
                                Case = Case.Absolutive,
                                Class = grammaticalClasses.Length > 0 ? grammaticalClasses[0] : 0,
                                NumeralType = !translationText.Contains("только мн") ? NumeralType.Singular : NumeralType.Plural,
                            };
                            subEntry.Details = JsonConvert.SerializeObject(nounDetails);

                            AddNounForms(subEntry, forms, t, grammaticalClasses, nounDetails);
                        }
                        else
                        {
                            if (IsPlural(rawForms) && (subEntry.Subtype == (int)WordType.Noun || subEntry.Subtype == 0))
                            {
                                // Set plural
                                subEntry.Subtype = (int)WordType.Noun;

                                var nounDetails = new NounDetails();
                                nounDetails.NumeralType = NumeralType.Plural;
                                if (grammaticalClasses.Length > 0)
                                {
                                    nounDetails.Class = grammaticalClasses[0];
                                }
                                subEntry.Details = JsonConvert.SerializeObject(nounDetails);
                            }

                            t = t + " [" + rawForms + "]";
                        }
                        _context.SaveChanges();

                        // Prepare for the next cycle
                        try
                        {
                            translationText = translationText.Substring(match.Value.Length + t.Length + 1).Trim();
                        }
                        catch (Exception ex)
                        { }
                    }
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                transaction.Rollback();
            }
        }

        static bool IsPlural(string str)
        {
            var singularVsPluralPattern = @"мн\W";
            var plurality = Regex.Match(str, singularVsPluralPattern);
            return plurality.Success;
        }
        static SqlContext GetSqlContext()
        {
            var options = new DbContextOptionsBuilder<SqlContext>().UseMySQL(/*Constants.LocalDatabaseConnectionString*/).Options;
            var context = new SqlContext(options);

            return context;
        }

        static bool TestFor(string str, string needle)
        {
            string pattern = $"(?<={needle}\\W?\\s?)[1ӀӏА-яA-z]+";
            return Regex.Match(str, pattern, RegexOptions.CultureInvariant).Success;
        }

        private static int[] ParseGrammaticalClass(string rawForms)
        {
            rawForms = rawForms.Replace("5", "б");

            var ClassesMap = new Dictionary<int, string>()
            {
                { 1 ,"в, б/д"},
                { 2 ,"й, б/д"},
                { 3 ,"й, й"},
                { 4 ,"д, д"},
                { 5 ,"б, б/й"},
                { 6 ,"б, д"},
            };

            string pattern = @"(?<=\s|\W)(д|в|б|й)(?=\W|$)";
            var matches = Regex.Matches(rawForms, pattern);
            var count = matches.Count();


            if (count == 3)
            {
                // Search by first and last
                // Search by second and last

                var gClass1 = ClassesMap.First(kvp => kvp.Value.Contains(matches[0].Value + ",")
                                      && kvp.Value.Substring(kvp.Value.IndexOf(",")).Contains(matches[2].Value)).Key;

                var gClass2 = ClassesMap.First(kvp => kvp.Value.Contains(matches[1].Value + ",")
                                      && kvp.Value.Substring(kvp.Value.IndexOf(",")).Contains(matches[2].Value)).Key;

                return new int[] { gClass1, gClass2 };
            }
            if (count == 2)
            {
                var c1 = matches[0].Value;
                var c2 = matches[1].Value;

                if (c1 == "й" && c2 == "д")
                {
                    c1 = "д";
                }

                if (c1 == "д" && c2 == "й")
                {
                    c1 = "й";
                }

                var gClass1 = ClassesMap.First(kvp => kvp.Value.Contains(c1 + ",")
                                      && kvp.Value.Substring(kvp.Value.IndexOf(",")).Contains(c2)).Key;

                return new int[] { gClass1 };
            }
            if (count == 1)
            {
                var gClass1 = ClassesMap.First(kvp => kvp.Value.Contains(matches[0].Value + ",")).Key;

                return new int[] { gClass1 };
            }

            return new int[] { };
        }
        private static void AddPronounForms(SqlEntry entry, List<string> forms, string translationText, int[] grammaticalClasses, PronounDetails pronounDetails)
        {
            var count = forms.Count();

            // Родительный
            var genitive = CreateSubEntry(entry, forms[0]);
            pronounDetails.Case = Case.Genitive;
            genitive.Details = JsonConvert.SerializeObject(pronounDetails);
            _context.Entries.Add(genitive);

            // Дательный
            var dative = CreateSubEntry(entry, forms[1]);
            pronounDetails.Case = Case.Dative;
            dative.Details = JsonConvert.SerializeObject(pronounDetails);
            _context.Entries.Add(dative);

            // Эргативный
            var ergative = CreateSubEntry(entry, forms[2]);
            pronounDetails.Case = Case.Ergative;
            ergative.Details = JsonConvert.SerializeObject(pronounDetails);
            _context.Entries.Add(ergative);

            // Местный
            var locative = CreateSubEntry(entry, forms[3]);
            pronounDetails.Case = Case.Locative;
            locative.Details = JsonConvert.SerializeObject(pronounDetails);
            _context.Entries.Add(locative);

            // Именительный во множественном 
            var pluralAbsolutive = CreateSubEntry(entry, forms[4]);
            pronounDetails.Case = Case.Absolutive;
            pluralAbsolutive.Details = JsonConvert.SerializeObject(pronounDetails);
            _context.Entries.Add(pluralAbsolutive);
        }
        private static void AddNounForms(SqlEntry entry, List<string> forms, string translationText, int[] grammaticalClasses, NounDetails nounDetails)
        {
            var count = forms.Count();

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

        private static void AddVerbForms(SqlEntry entry, List<string> forms, string translationText, int[] grammaticalClasses, VerbDetails verbDetails)
        {
            var count = forms.Count();

            // first - present simple
            var presentSimple = CreateSubEntry(entry, forms[0]);
            verbDetails.Tense = VerbTense.PresentSimple;
            presentSimple.Details = JsonConvert.SerializeObject(verbDetails);
            _context.Entries.Add(presentSimple);

            // second - witnessed past
            var witnessedPast = CreateSubEntry(entry, forms[1]);
            verbDetails.Tense = VerbTense.PastWitnessed;
            presentSimple.Details = JsonConvert.SerializeObject(verbDetails);
            _context.Entries.Add(witnessedPast);

            // third - past perfect
            var pastPerfect = CreateSubEntry(entry, forms[2]);
            verbDetails.Tense = VerbTense.PastPerfect;
            presentSimple.Details = JsonConvert.SerializeObject(verbDetails);
            _context.Entries.Add(pastPerfect);

            if (count == 3)
            {
                return;
            }

            // fourth - future simple
            var futureSimple = CreateSubEntry(entry, forms[3]);
            verbDetails.Tense = VerbTense.FutureSimple;
            presentSimple.Details = JsonConvert.SerializeObject(verbDetails);
            _context.Entries.Add(futureSimple);

            if (count == 4)
            {
                return;
            }

            // fifth - future continuous
            var futureContinuous = CreateSubEntry(entry, forms[4]);
            verbDetails.Tense = VerbTense.FutureContinuous;
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
                UserId = entry.UserId,
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
                    UserId = entry.UserId,
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
