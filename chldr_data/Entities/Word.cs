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

        // Must be removed with migrations ===============<
        internal int GrammaticalClass { get; set; }
        internal string Forms { get; set; } = string.Empty;
        // ================================================

        [PrimaryKey]
        public ObjectId _id { get; set; } = ObjectId.GenerateNewId(DateTime.Now);
        public Entry Entry { get; set; }
        [Indexed]
        public string Content { get; set; } = string.Empty;
        [Indexed]
        public string Notes { get; set; } = string.Empty;
        public IList<int> GrammaticalClasses { get; } = new List<int>();
        public int PartOfSpeech { get; set; }
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
        public static string StringifyVerbTenses(Dictionary<string, string> tensesMap)
        {
            if (tensesMap.Count == 0)
            {
                return EmptyRawWordTensesValue;
            }

            var sb = new StringBuilder();
            sb.AppendJoin(";",
                tensesMap["VerbTensePresentContinious"],
                tensesMap["VerbTensePresentContinuousCausative"],
                tensesMap["VerbTensePresentContinuousInceptive"],
                tensesMap["VerbTensePresentContinuousPermissive"],
                tensesMap["VerbTensePresentContinuousPermissiveCausative"],
                tensesMap["VerbTensePresentContinuousPotential"],
                tensesMap["VerbTensePresentSimple"],
                tensesMap["VerbTensePresentSimpleCausative"],
                tensesMap["VerbTensePresentSimpleInceptive"],
                tensesMap["VerbTensePresentSimplePermissive"],
                tensesMap["VerbTensePresentSimplePermissiveCausative"],
                tensesMap["VerbTensePresentSimplePotential"],
                tensesMap["VerbTenseFuturePossible"],
                tensesMap["VerbTenseFuturePossibleCausative"],
                tensesMap["VerbTenseFuturePossibleInceptive"],
                tensesMap["VerbTenseFuturePossiblePermissive"],
                tensesMap["VerbTenseFuturePossiblePermissiveCausative"],
                tensesMap["VerbTenseFuturePossiblePotential"],
                tensesMap["VerbTenseFutureReal"],
                tensesMap["VerbTenseFutureRealCausative"],
                tensesMap["VerbTenseFutureRealInceptive"],
                tensesMap["VerbTenseFutureRealPermissive"],
                tensesMap["VerbTenseFutureRealPermissiveCausative"],
                tensesMap["VerbTenseFutureRealPotential"],
                tensesMap["VerbTensePastImperfect"],
                tensesMap["VerbTensePastImperfectCausative"],
                tensesMap["VerbTensePastImperfectInceptive"],
                tensesMap["VerbTensePastImperfectPermissive"],
                tensesMap["VerbTensePastImperfectPermissiveCausative"],
                tensesMap["VerbTensePastImperfectPotential"],
                tensesMap["VerbTensePastPerfect"],
                tensesMap["VerbTensePastPerfectCausative"],
                tensesMap["VerbTensePastPerfectInceptive"],
                tensesMap["VerbTensePastPerfectPermissive"],
                tensesMap["VerbTensePastPerfectPermissiveCausative"],
                tensesMap["VerbTensePastPerfectPotential"],
                tensesMap["VerbTensePastRecent"],
                tensesMap["VerbTensePastRecentCausative"],
                tensesMap["VerbTensePastRecentInceptive"],
                tensesMap["VerbTensePastRecentPermissive"],
                tensesMap["VerbTensePastRecentPermissiveCausative"],
                tensesMap["VerbTensePastRecentPotential"],
                tensesMap["VerbTensePastWitnessed"],
                tensesMap["VerbTensePastWitnessedCausative"],
                tensesMap["VerbTensePastWitnessedInceptive"],
                tensesMap["VerbTensePastWitnessedPermissive"],
                tensesMap["VerbTensePastWitnessedPermissiveCausative"],
                tensesMap["VerbTensePastWitnessedPotential"],
                tensesMap["VerbTensePastRemote"],
                tensesMap["VerbTensePastRemoteCausative"],
                tensesMap["VerbTensePastRemoteInceptive"],
                tensesMap["VerbTensePastRemotePermissive"],
                tensesMap["VerbTensePastRemotePermissiveCausative"],
                tensesMap["VerbTensePastRemotePotential"]);

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

            var sb = new StringBuilder();

            sb.AppendJoin(";",
                declensionsMap["NounDeclensionSingularAbsolutive"],
                declensionsMap["NounDeclensionSingularAllative"],
                declensionsMap["NounDeclensionSingularComparative"],
                declensionsMap["NounDeclensionSingularDative"],
                declensionsMap["NounDeclensionSingularErgative"],
                declensionsMap["NounDeclensionSingularGenitive"],
                declensionsMap["NounDeclensionSingularInstrumental"],
                declensionsMap["NounDeclensionSingularLocative"],
                declensionsMap["NounDeclensionPluralAbsolutive"],
                declensionsMap["NounDeclensionPluralAllative"],
                declensionsMap["NounDeclensionPluralComparative"],
                declensionsMap["NounDeclensionPluralDative"],
                declensionsMap["NounDeclensionPluralErgative"],
                declensionsMap["NounDeclensionPluralGenitive"],
                declensionsMap["NounDeclensionPluralInstrumental"],
                declensionsMap["NounDeclensionPluralLocative"]);

            var res = sb.ToString();
            return res;
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