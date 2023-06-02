using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models.Words;
using chldr_data.DatabaseObjects.SqlEntities;
using chldr_tools;

namespace chldr_data.Repositories
{
    public class WordsRepository : Repository<SqlWord, WordModel, WordDto>
    {
        public WordsRepository(SqlContext context) : base(context)
        {
        }
    }
}