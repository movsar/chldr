using chldr_maintenance.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_maintenance
{
    public static class LegacyEntriesProvider
    {
        public static List<LegacyEntry> GetAllLegacyEntries()
        {
            var entries = new List<LegacyEntry>();

            using (var context = new EfContext())
            {
                entries = context.LegacyEntries.ToList();
            }

            return entries;
        }

        public static List<LegacyPhraseEntry> GetLegacyPhraseEntries()
        {
            var entries = new List<LegacyPhraseEntry>();

            using (var context = new EfContext())
            {
                entries = context.LegacyPhraseEntries.Where(phrase =>
                phrase.Source != "MACIEV" && phrase.Source != "ANATSLOVAR" && phrase.Source != "MALAEV"
                && phrase.Rate > 0).ToList();
            }

            return entries;
        }

        public static List<LegacyTranslationEntry> GetLegacyPhraseTranslationEntries()
        {
            var entries = new List<LegacyTranslationEntry>();

            using (var context = new EfContext())
            {
                entries = context.LegacyTranslationEntries.Where(t => t.Rate > 0).ToList();
            }

            return entries;
        }
    }
}
