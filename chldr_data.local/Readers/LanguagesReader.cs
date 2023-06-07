using chldr_data.DatabaseObjects.Models;
using chldr_data.local.RealmEntities;

namespace chldr_data.Readers
{
    public class LanguagesReader : DataReader<RealmLanguage, LanguageModel>
    {
        public List<LanguageModel> GetAllLanguages()
        {
            var languages = Database.All<RealmLanguage>().AsEnumerable().Select(l => LanguageModel.FromEntity(l));
            return languages.ToList();
        }
    }
}
