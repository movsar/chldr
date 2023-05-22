
using chldr_data.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.RealmEntities;
using MongoDB.Bson;

namespace chldr_data.Repositories
{
    public class LanguagesRepository : Repository
    {
        public LanguagesRepository(IDataAccess dataAccess) : base(dataAccess) { }

        public List<LanguageModel> GetAllLanguages()
        {
            try
            {
                Console.WriteLine("before langs");
                var languages = Database.All<RealmLanguage>().AsEnumerable().Select(l =>  LanguageModel.FromEntity(l));
                Console.WriteLine("after langs");
                return languages.ToList();

            }
            catch (Exception ex)
            {
                //throw new Exception(ex.Message);
                return new List<LanguageModel>();
            }
        }

    }
}
