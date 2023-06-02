using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models.Words;
using chldr_data.DatabaseObjects.SqlEntities;
using chldr_data.Interfaces;
using chldr_tools;
using Microsoft.EntityFrameworkCore;

namespace chldr_data.Repositories
{
    public class WordsRepository : Repository<SqlWord, WordModel, WordDto>, IWordsRepository
    {
        public WordsRepository(SqlContext context) : base(context) { }

        public override void Add(WordDto dto)
        {
            var entity = SqlWord.FromDto(dto);
            SqlContext.Add(entity);
        }

        public void AddRange(IEnumerable<WordDto> dtos)
        {
            var entities = dtos.Select(d => SqlWord.FromDto(d));
            SqlContext.AddRange(entities);
        }

        public override WordModel Get(string entityId)
        {
            var word = SqlContext.Words
                          .Include(w => w.Entry)
                          .Include(w => w.Entry.Source)
                          .Include(w => w.Entry.User)
                          .Include(w => w.Entry.Translations)
                          .ThenInclude(t => t.Language)
                          .FirstOrDefault(w => w.WordId.Equals(entityId));

            if (word == null)
            {
                throw new ArgumentException($"Entity not found: {entityId}");
            }

            return WordModel.FromEntity(word);
        }


        public override void Update(WordDto dto)
        {
            throw new NotImplementedException();
        }
    }
}