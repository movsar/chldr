using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.Models.Words;
using chldr_data.DatabaseObjects.RealmEntities;

namespace chldr_data.Readers
{
    public class WordsReader : DataReader<RealmWord, WordModel>
    {
        public EntryModel GetByEntryId(string entryId)
        {
            var word = Database.Find<RealmEntry>(entryId)!.Word;
            if (word == null)
            {
                throw new Exception("There is no such word in the database");
            }

            return WordModel.FromEntity(word);
        }

        public WordModel GetById(string entityId)
        {
            var word = Database.All<RealmWord>().FirstOrDefault(w => w.WordId == entityId);
            if (word == null)
            {
                throw new Exception("There is no such word in the database");
            }

            return WordModel.FromEntity(word);
        }

        public List<WordModel> GetRandomWords(int limit)
        {
            var words = Database.All<RealmWord>().AsEnumerable().Take(limit);
            return words.Select(w => WordModel.FromEntity(w)).ToList();
        }

    }
}
