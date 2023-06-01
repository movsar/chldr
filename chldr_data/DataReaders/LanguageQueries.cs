using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.RealmEntities;

namespace chldr_data.Readers
{
    public class LanguageQueries : DataQueries<RealmLanguage, LanguageModel>
    {
        public IEnumerable<LanguageModel> GetAllLanguages()
        {
            throw new NotImplementedException();
        }
    }
}
