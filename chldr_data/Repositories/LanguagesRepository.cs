using chldr_data.Dto;
using chldr_data.Entities;
using chldr_data.Interfaces;
using chldr_data.Models;
using MongoDB.Bson;

namespace chldr_data.Repositories
{
    public class LanguagesRepository : Repository
    {
        public LanguagesRepository(IRealmServiceFactory realmServiceFactory) : base(realmServiceFactory) { }

        public List<LanguageModel> GetAllLanguages()
        {
            try
            {
                Console.WriteLine("before langs");
                var languages = Database.All<Language>().AsEnumerable().Select(l => new LanguageModel(l));
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
