using MongoDB.Bson;
using Realms;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
namespace Data.Entities
{
    [MapTo("Word")]
    public class WordEntity: RealmObject
    {
        public const string EmptyRawWordDeclensionsValue = ";;;;;;;;;;;;;;;";
        public const string EmptyRawWordTensesValue = ";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;";

        [Ignored]
        public class WordType
        {
            public const byte Verb = 1;
            public const byte Noun = 2;
        };

        [Key]
        [PrimaryKey] public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
        public EntryEntity Entry { get; set; }
        [Indexed]
        public string Content { get; set; } = string.Empty;
        [Indexed]
        public string Notes { get; set; } = string.Empty;
        public int GrammaticalClass { get; set; }
        public int PartOfSpeech { get; set; }
        // To help search work better
        public string Forms { get; set; } = string.Empty;
        public string VerbTenses { get; set; } = string.Empty;
        // something; thing; whatever; - order matters!
        public string NounDeclensions { get; set; } = string.Empty;
        // something; thing; whatever; - order matters!
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.Now;
        public static string[] GetAllUniqueWordForms(string content, string rawForms, string rawWordDeclensions, string rawWordTenses, bool excludeTitle = false)
        {
            var allWordForms = new HashSet<string>();


            var declensions = String.IsNullOrEmpty(rawWordDeclensions) ? null : ParseNounDeclensions(rawWordDeclensions).Values.Where(v => !String.IsNullOrWhiteSpace(v)).ToList();
            var tenses = String.IsNullOrEmpty(rawWordTenses) ? null : ParseVerbTenses(rawWordTenses).Values.Where(v => !String.IsNullOrWhiteSpace(v)).ToList();
            var forms = String.IsNullOrEmpty(rawForms) ? null : rawForms.Split(";");

            if (declensions != null)
            {
                foreach (var item in declensions)
                {
                    allWordForms.Add(item);
                }
            }

            if (tenses != null)
            {
                foreach (var item in tenses)
                {
                    allWordForms.Add(item);
                }
            }

            if (forms != null)
            {
                foreach (var item in forms)
                {
                    allWordForms.Add(item);
                }
            }

            if (excludeTitle)
            {
                allWordForms.Remove(content.ToLower());
            }
            else
            {
                allWordForms.Add(content.ToLower());
            }

            return allWordForms.ToArray();
        }
        public static string ParseGrammaticalClass(int grammaticalClass)
        {
            var ClassesMap = new Dictionary<int, string>()
            {
                { 1 ,"в, б/д"},
                { 2 ,"й, б/д"},
                { 3 ,"й, й"},
                { 4 ,"д, д"},
                { 5 ,"б, б/й"},
                { 6 ,"б, д"},
            };

            if (grammaticalClass == 0)
            {
                return null;
            }

            return ClassesMap[grammaticalClass];
        }
        public static Dictionary<string, string> ParseVerbTenses(string rawTenses)
        {
            string[] verbTenses = new string[54];
            verbTenses[0] = "VerbTensePresentContinious";
            verbTenses[1] = "VerbTensePresentContinuousCausative";
            verbTenses[2] = "VerbTensePresentContinuousInceptive";
            verbTenses[3] = "VerbTensePresentContinuousPermissive";
            verbTenses[4] = "VerbTensePresentContinuousPermissiveCausative";
            verbTenses[5] = "VerbTensePresentContinuousPotential";

            verbTenses[6] = "VerbTensePresentSimple";
            verbTenses[7] = "VerbTensePresentSimpleCausative";
            verbTenses[8] = "VerbTensePresentSimpleInceptive";
            verbTenses[9] = "VerbTensePresentSimplePermissive";
            verbTenses[10] = "legacyWord.VerbTensePresentSimplePermissiveCausative";
            verbTenses[11] = "legacyWord.VerbTensePresentSimplePotential";

            verbTenses[12] = "legacyWord.VerbTenseFuturePossible";
            verbTenses[13] = "legacyWord.VerbTenseFuturePossibleCausative";
            verbTenses[14] = "legacyWord.VerbTenseFuturePossibleInceptive";
            verbTenses[15] = "legacyWord.VerbTenseFuturePossiblePermissive";
            verbTenses[16] = "legacyWord.VerbTenseFuturePossiblePermissiveCausative";
            verbTenses[17] = "legacyWord.VerbTenseFuturePossiblePotential";

            verbTenses[18] = "legacyWord.VerbTenseFutureReal";
            verbTenses[19] = "legacyWord.VerbTenseFutureRealCausative";
            verbTenses[20] = "legacyWord.VerbTenseFutureRealInceptive";
            verbTenses[21] = "legacyWord.VerbTenseFutureRealPermissive";
            verbTenses[22] = "legacyWord.VerbTenseFutureRealPermissiveCausative";
            verbTenses[23] = "legacyWord.VerbTenseFutureRealPotential";

            verbTenses[24] = "legacyWord.VerbTensePastImperfect";
            verbTenses[25] = "legacyWord.VerbTensePastImperfectCausative";
            verbTenses[26] = "legacyWord.VerbTensePastImperfectInceptive";
            verbTenses[27] = "legacyWord.VerbTensePastImperfectPermissive";
            verbTenses[28] = "legacyWord.VerbTensePastImperfectPermissiveCausative";
            verbTenses[29] = "legacyWord.VerbTensePastImperfectPotential";

            verbTenses[30] = "legacyWord.VerbTensePastPerfect";
            verbTenses[31] = "legacyWord.VerbTensePastPerfectCausative";
            verbTenses[32] = "legacyWord.VerbTensePastPerfectInceptive";
            verbTenses[33] = "legacyWord.VerbTensePastPerfectPermissive";
            verbTenses[34] = "legacyWord.VerbTensePastPerfectPermissiveCausative";
            verbTenses[35] = "legacyWord.VerbTensePastPerfectPotential";

            verbTenses[36] = "legacyWord.VerbTensePastRecent";
            verbTenses[37] = "legacyWord.VerbTensePastRecentCausative";
            verbTenses[38] = "legacyWord.VerbTensePastRecentInceptive";
            verbTenses[39] = "legacyWord.VerbTensePastRecentPermissive";
            verbTenses[40] = "legacyWord.VerbTensePastRecentPermissiveCausative";
            verbTenses[41] = "legacyWord.VerbTensePastRecentPotential";

            verbTenses[42] = "legacyWord.VerbTensePastWitnessed";
            verbTenses[43] = "legacyWord.VerbTensePastWitnessedCausative";
            verbTenses[44] = "legacyWord.VerbTensePastWitnessedInceptive";
            verbTenses[45] = "legacyWord.VerbTensePastWitnessedPermissive";
            verbTenses[46] = "legacyWord.VerbTensePastWitnessedPermissiveCausative";
            verbTenses[47] = "legacyWord.VerbTensePastWitnessedPotential";

            verbTenses[48] = "legacyWord.VerbTensePastRemote";
            verbTenses[49] = "legacyWord.VerbTensePastRemoteCausative";
            verbTenses[50] = "legacyWord.VerbTensePastRemoteInceptive";
            verbTenses[51] = "legacyWord.VerbTensePastRemotePermissive";
            verbTenses[52] = "legacyWord.VerbTensePastRemotePermissiveCausative";
            verbTenses[53] = "legacyWord.VerbTensePastRemotePotential";

            Dictionary<string, string> tensesMap = new Dictionary<string, string>();

            string[] tenseValue = rawTenses.Split(";");
            for (int i = 0; i < 54; i++)
            {
                tensesMap.Add(verbTenses[i], tenseValue[i]);
            }

            return tensesMap;
        }
        public static Dictionary<string, string> ParseNounDeclensions(string rawDeclensions)
        {
            string[] declensionNames = new string[16];
            declensionNames[0] = "NounDeclensionSingularAbsolutive";
            declensionNames[1] = "NounDeclensionSingularAllative";
            declensionNames[2] = "NounDeclensionSingularComparative";
            declensionNames[3] = "NounDeclensionSingularDative";
            declensionNames[4] = "NounDeclensionSingularErgative";
            declensionNames[5] = "NounDeclensionSingularGenitive";
            declensionNames[6] = "NounDeclensionSingularInstrumental";
            declensionNames[7] = "NounDeclensionSingularLocative";

            declensionNames[8] = "NounDeclensionPluralAbsolutive";
            declensionNames[9] = "NounDeclensionPluralAllative";
            declensionNames[10] = "NounDeclensionPluralComparative";
            declensionNames[11] = "NounDeclensionPluralDative";
            declensionNames[12] = "NounDeclensionPluralErgative";
            declensionNames[13] = "NounDeclensionPluralGenitive";
            declensionNames[14] = "NounDeclensionPluralInstrumental";
            declensionNames[15] = "NounDeclensionPluralLocative";

            Dictionary<string, string> declensionsMap = new Dictionary<string, string>();

            string[] declensionValues = rawDeclensions.Split(";");
            for (int i = 0; i < 16; i++)
            {
                declensionsMap.Add(declensionNames[i], declensionValues[i]);
            }

            return declensionsMap;
        }
    }
}
