using chldr_data.Interfaces;
using MongoDB.Bson;
using Realms;
using System.Text;

namespace chldr_data.Entities
{
    public class Word : RealmObject, IEntity
    {
        public const string EmptyRawWordDeclensionsValue = ";;;;;;;;;;;;;;;";
        public const string EmptyRawWordTensesValue = ";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;";

        [PrimaryKey]
        public ObjectId _id { get; set; } = ObjectId.GenerateNewId(DateTime.Now);
        public Entry Entry { get; set; }
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
        public string GetRawContents()
        {
            var allWordForms = new HashSet<string>();

            var declensions = String.IsNullOrEmpty(NounDeclensions) ? null : ParseNounDeclensions(NounDeclensions).Values.Where(v => !String.IsNullOrWhiteSpace(v)).ToList();
            var tenses = String.IsNullOrEmpty(VerbTenses) ? null : ParseVerbTenses(VerbTenses).Values.Where(v => !String.IsNullOrWhiteSpace(v)).ToList();
            var forms = String.IsNullOrEmpty(Forms) ? null : Forms.Split(";");

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

            allWordForms.Add(Content.ToLower());

            return string.Join("; ", allWordForms.Select(w => w)).ToLower();
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
        public static string StringifyVerbTenses(Dictionary<string, string> tensesMap)
        {
            if (tensesMap.Count == 0)
            {
                return EmptyRawWordTensesValue;
            }

            StringBuilder sb = new StringBuilder();

            sb.AppendJoin(";", tensesMap["VerbTensePresentContinious"]);
            sb.AppendJoin(";", tensesMap["VerbTensePresentContinuousCausative"]);
            sb.AppendJoin(";", tensesMap["VerbTensePresentContinuousInceptive"]);
            sb.AppendJoin(";", tensesMap["VerbTensePresentContinuousPermissive"]);
            sb.AppendJoin(";", tensesMap["VerbTensePresentContinuousPermissiveCausative"]);
            sb.AppendJoin(";", tensesMap["VerbTensePresentContinuousPotential"]);
            sb.AppendJoin(";", tensesMap["VerbTensePresentSimple"]);
            sb.AppendJoin(";", tensesMap["VerbTensePresentSimpleCausative"]);
            sb.AppendJoin(";", tensesMap["VerbTensePresentSimpleInceptive"]);
            sb.AppendJoin(";", tensesMap["VerbTensePresentSimplePermissive"]);
            sb.AppendJoin(";", tensesMap["VerbTensePresentSimplePermissiveCausative"]);
            sb.AppendJoin(";", tensesMap["VerbTensePresentSimplePotential"]);
            sb.AppendJoin(";", tensesMap["VerbTenseFuturePossible"]);
            sb.AppendJoin(";", tensesMap["VerbTenseFuturePossibleCausative"]);
            sb.AppendJoin(";", tensesMap["VerbTenseFuturePossibleInceptive"]);
            sb.AppendJoin(";", tensesMap["VerbTenseFuturePossiblePermissive"]);
            sb.AppendJoin(";", tensesMap["VerbTenseFuturePossiblePermissiveCausative"]);
            sb.AppendJoin(";", tensesMap["VerbTenseFuturePossiblePotential"]);
            sb.AppendJoin(";", tensesMap["VerbTenseFutureReal"]);
            sb.AppendJoin(";", tensesMap["VerbTenseFutureRealCausative"]);
            sb.AppendJoin(";", tensesMap["VerbTenseFutureRealInceptive"]);
            sb.AppendJoin(";", tensesMap["VerbTenseFutureRealPermissive"]);
            sb.AppendJoin(";", tensesMap["VerbTenseFutureRealPermissiveCausative"]);
            sb.AppendJoin(";", tensesMap["VerbTenseFutureRealPotential"]);
            sb.AppendJoin(";", tensesMap["VerbTensePastImperfect"]);
            sb.AppendJoin(";", tensesMap["VerbTensePastImperfectCausative"]);
            sb.AppendJoin(";", tensesMap["VerbTensePastImperfectInceptive"]);
            sb.AppendJoin(";", tensesMap["VerbTensePastImperfectPermissive"]);
            sb.AppendJoin(";", tensesMap["VerbTensePastImperfectPermissiveCausative"]);
            sb.AppendJoin(";", tensesMap["VerbTensePastImperfectPotential"]);
            sb.AppendJoin(";", tensesMap["VerbTensePastPerfect"]);
            sb.AppendJoin(";", tensesMap["VerbTensePastPerfectCausative"]);
            sb.AppendJoin(";", tensesMap["VerbTensePastPerfectInceptive"]);
            sb.AppendJoin(";", tensesMap["VerbTensePastPerfectPermissive"]);
            sb.AppendJoin(";", tensesMap["VerbTensePastPerfectPermissiveCausative"]);
            sb.AppendJoin(";", tensesMap["VerbTensePastPerfectPotential"]);
            sb.AppendJoin(";", tensesMap["VerbTensePastRecent"]);
            sb.AppendJoin(";", tensesMap["VerbTensePastRecentCausative"]);
            sb.AppendJoin(";", tensesMap["VerbTensePastRecentInceptive"]);
            sb.AppendJoin(";", tensesMap["VerbTensePastRecentPermissive"]);
            sb.AppendJoin(";", tensesMap["VerbTensePastRecentPermissiveCausative"]);
            sb.AppendJoin(";", tensesMap["VerbTensePastRecentPotential"]);
            sb.AppendJoin(";", tensesMap["VerbTensePastWitnessed"]);
            sb.AppendJoin(";", tensesMap["VerbTensePastWitnessedCausative"]);
            sb.AppendJoin(";", tensesMap["VerbTensePastWitnessedInceptive"]);
            sb.AppendJoin(";", tensesMap["VerbTensePastWitnessedPermissive"]);
            sb.AppendJoin(";", tensesMap["VerbTensePastWitnessedPermissiveCausative"]);
            sb.AppendJoin(";", tensesMap["VerbTensePastWitnessedPotential"]);
            sb.AppendJoin(";", tensesMap["VerbTensePastRemote"]);
            sb.AppendJoin(";", tensesMap["VerbTensePastRemoteCausative"]);
            sb.AppendJoin(";", tensesMap["VerbTensePastRemoteInceptive"]);
            sb.AppendJoin(";", tensesMap["VerbTensePastRemotePermissive"]);
            sb.AppendJoin(";", tensesMap["VerbTensePastRemotePermissiveCausative"]);
            sb.AppendJoin(";", tensesMap["VerbTensePastRemotePotential"]);

            return sb.ToString();
        }
        public static Dictionary<string, string> ParseVerbTenses(string rawTenses)
        {
            if (rawTenses.Length < 1)
            {
                return new Dictionary<string, string>();
            }

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
            verbTenses[10] = "VerbTensePresentSimplePermissiveCausative";
            verbTenses[11] = "VerbTensePresentSimplePotential";

            verbTenses[12] = "VerbTenseFuturePossible";
            verbTenses[13] = "VerbTenseFuturePossibleCausative";
            verbTenses[14] = "VerbTenseFuturePossibleInceptive";
            verbTenses[15] = "VerbTenseFuturePossiblePermissive";
            verbTenses[16] = "VerbTenseFuturePossiblePermissiveCausative";
            verbTenses[17] = "VerbTenseFuturePossiblePotential";

            verbTenses[18] = "VerbTenseFutureReal";
            verbTenses[19] = "VerbTenseFutureRealCausative";
            verbTenses[20] = "VerbTenseFutureRealInceptive";
            verbTenses[21] = "VerbTenseFutureRealPermissive";
            verbTenses[22] = "VerbTenseFutureRealPermissiveCausative";
            verbTenses[23] = "VerbTenseFutureRealPotential";

            verbTenses[24] = "VerbTensePastImperfect";
            verbTenses[25] = "VerbTensePastImperfectCausative";
            verbTenses[26] = "VerbTensePastImperfectInceptive";
            verbTenses[27] = "VerbTensePastImperfectPermissive";
            verbTenses[28] = "VerbTensePastImperfectPermissiveCausative";
            verbTenses[29] = "VerbTensePastImperfectPotential";

            verbTenses[30] = "VerbTensePastPerfect";
            verbTenses[31] = "VerbTensePastPerfectCausative";
            verbTenses[32] = "VerbTensePastPerfectInceptive";
            verbTenses[33] = "VerbTensePastPerfectPermissive";
            verbTenses[34] = "VerbTensePastPerfectPermissiveCausative";
            verbTenses[35] = "VerbTensePastPerfectPotential";

            verbTenses[36] = "VerbTensePastRecent";
            verbTenses[37] = "VerbTensePastRecentCausative";
            verbTenses[38] = "VerbTensePastRecentInceptive";
            verbTenses[39] = "VerbTensePastRecentPermissive";
            verbTenses[40] = "VerbTensePastRecentPermissiveCausative";
            verbTenses[41] = "VerbTensePastRecentPotential";

            verbTenses[42] = "VerbTensePastWitnessed";
            verbTenses[43] = "VerbTensePastWitnessedCausative";
            verbTenses[44] = "VerbTensePastWitnessedInceptive";
            verbTenses[45] = "VerbTensePastWitnessedPermissive";
            verbTenses[46] = "VerbTensePastWitnessedPermissiveCausative";
            verbTenses[47] = "VerbTensePastWitnessedPotential";

            verbTenses[48] = "VerbTensePastRemote";
            verbTenses[49] = "VerbTensePastRemoteCausative";
            verbTenses[50] = "VerbTensePastRemoteInceptive";
            verbTenses[51] = "VerbTensePastRemotePermissive";
            verbTenses[52] = "VerbTensePastRemotePermissiveCausative";
            verbTenses[53] = "VerbTensePastRemotePotential";

            Dictionary<string, string> tensesMap = new Dictionary<string, string>();

            string[] tenseValue = rawTenses.Split(";");
            for (int i = 0; i < 54; i++)
            {
                tensesMap.Add(verbTenses[i], tenseValue[i]);
            }

            return tensesMap;
        }
        public static string StringifyNounDeclensions(Dictionary<string, string> declensionsMap)
        {
            if (declensionsMap.Values.Count() == 0)
            {
                return EmptyRawWordDeclensionsValue;
            }

            StringBuilder sb = new StringBuilder();

            sb.AppendJoin(";", declensionsMap["NounDeclensionSingularAbsolutive"]);
            sb.AppendJoin(";", declensionsMap["NounDeclensionSingularAllative"]);
            sb.AppendJoin(";", declensionsMap["NounDeclensionSingularComparative"]);
            sb.AppendJoin(";", declensionsMap["NounDeclensionSingularDative"]);
            sb.AppendJoin(";", declensionsMap["NounDeclensionSingularErgative"]);
            sb.AppendJoin(";", declensionsMap["NounDeclensionSingularGenitive"]);
            sb.AppendJoin(";", declensionsMap["NounDeclensionSingularInstrumental"]);
            sb.AppendJoin(";", declensionsMap["NounDeclensionSingularLocative"]);
            sb.AppendJoin(";", declensionsMap["NounDeclensionPluralAbsolutive"]);
            sb.AppendJoin(";", declensionsMap["NounDeclensionPluralAllative"]);
            sb.AppendJoin(";", declensionsMap["NounDeclensionPluralComparative"]);
            sb.AppendJoin(";", declensionsMap["NounDeclensionPluralDative"]);
            sb.AppendJoin(";", declensionsMap["NounDeclensionPluralErgative"]);
            sb.AppendJoin(";", declensionsMap["NounDeclensionPluralGenitive"]);
            sb.AppendJoin(";", declensionsMap["NounDeclensionPluralInstrumental"]);
            sb.AppendJoin(";", declensionsMap["NounDeclensionPluralLocative"]);

            return sb.ToString();
        }
        public static Dictionary<string, string> ParseNounDeclensions(string rawDeclensions)
        {
            if (rawDeclensions.Length < 1)
            {
                return new Dictionary<string, string>();
            }

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
        internal static string StringifyForms(List<string> forms)
        {
            return string.Join(";", forms);
        }
    }
}