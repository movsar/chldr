using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models.Words;
using chldr_data.DatabaseObjects.SqlEntities;
using chldr_data.Interfaces;
using chldr_tools;

namespace chldr_data.Repositories
{
    public class WordsRepository : Repository<SqlWord, WordModel, WordDto>, IWordsRepository
    {
        public WordsRepository(SqlContext context) : base(context) { }

        public override void Add(WordDto dto)
        {
            throw new NotImplementedException();
        }

        public override WordModel Get(string entityId)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<WordModel> GetAll()
        {
            throw new NotImplementedException();
        }

        public override void Update(WordDto dto)
        {
            throw new NotImplementedException();
        }
    }
}