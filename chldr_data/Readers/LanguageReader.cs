using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.RealmEntities;

namespace chldr_data.Readers
{
    public class LanguageReader : DataReader<RealmLanguage, LanguageModel>
    {
        public List<LanguageModel> GetAllLanguages()
        {
            var languages = Database.All<RealmLanguage>().AsEnumerable().Select(l => LanguageModel.FromEntity(l));
            return languages.ToList();
        }
    }
}
