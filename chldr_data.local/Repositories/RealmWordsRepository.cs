using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.Models.Words;
using chldr_data.Enums;
using chldr_data.Interfaces.Repositories;
using chldr_data.local.RealmEntities;
using chldr_data.Models;
using chldr_data.ResponseTypes;
using chldr_utils.Interfaces;
using chldr_utils;
using GraphQL;
using Realms;

namespace chldr_data.Repositories
{
    public class RealmWordsRepository : RealmRepository<RealmEntry, WordModel, WordDto>, IWordsRepository
    {
        public RealmWordsRepository(Realm context, ExceptionHandler exceptionHandler, IGraphQLRequestSender graphQLRequestSender) : base(context, exceptionHandler, graphQLRequestSender) { }

        protected override RecordType RecordType => RecordType.Word;

        //public static WordModel FromEntity(RealmEntry word)
        //{
        //    return WordModel.FromEntity(
        //                            word.Entry.Word,
        //                            word.Entry,
        //                            word.Entry.Source,
        //                            word.Entry.Translations
        //                                .Select(t => new KeyValuePair<ILanguageEntity, ITranslationEntity>(t.Language, t)));
        //}
        public EntryModel GetByEntryId(string entryId)
        {
            throw new NotImplementedException();

            //var word = _dbContext.Find<RealmEntry>(entryId)!.Word;
            //if (word == null)
            //{
            //    throw new Exception("There is no such word in the database");
            //}

            //return FromEntity(word);
        }

        public List<WordModel> GetRandomWords(int limit)
        {
            //var words = _dbContext.All<RealmEntry>().AsEnumerable().Take(limit);
            //return words.Select(w => FromEntity(w)).ToList();
            throw new NotImplementedException();

        }
        public override WordModel Get(string entityId)
        {
            throw new NotImplementedException();

            //var word = _dbContext.All<RealmWord>().FirstOrDefault(w => w.WordId == entityId);
            //if (word == null)
            //{
            //    throw new Exception("There is no such word in the database");
            //}

            //return FromEntity(word);
        }

        public override void Insert(WordDto dto)
        {
            throw new NotImplementedException();
        }

        public override void Delete(string entityId)
        {
            throw new NotImplementedException();
        }
        private void ApplyEntryTranslationChanges(EntryDto existingEntryDto, EntryDto updatedEntryDto, ITranslationsRepository translationsRepository)
        {
            var existingTranslationIds = existingEntryDto.Translations.Select(t => t.TranslationId).ToHashSet();
            var updatedTranslationIds = updatedEntryDto.Translations.Select(t => t.TranslationId).ToHashSet();

            var insertedTranslations = updatedEntryDto.Translations.Where(t => !existingTranslationIds.Contains(t.TranslationId));
            var deletedTranslations = existingEntryDto.Translations.Where(t => !updatedTranslationIds.Contains(t.TranslationId));
            var updatedTranslations = updatedEntryDto.Translations.Where(t => existingTranslationIds.Contains(t.TranslationId) && updatedTranslationIds.Contains(t.TranslationId));

            foreach (var translationDto in insertedTranslations)
            {
                translationsRepository.Insert(translationDto);
            }

            foreach (var translationDto in deletedTranslations)
            {
                translationsRepository.Delete(translationDto.TranslationId);
            }

            foreach (var translationDto in updatedTranslations)
            {
                translationsRepository.Update(translationDto);
            }
        }

        public override void Update(WordDto updatedWordDto)
        {
            var existingWordDto = WordDto.FromModel(Get(updatedWordDto.WordId));
         
            // Apply changes to the entry entity
            var entryChanges = Change.GetChanges<EntryDto>(updatedWordDto, existingWordDto);
            if (entryChanges.Count != 0)
            {
                ApplyChanges<RealmEntry>(updatedWordDto.EntryId, entryChanges);
            }
        }

        public void Update(EntryDto updatedEntryDto, ITranslationsRepository translationsRepository)
        {
            var updatedWordDto = (WordDto)updatedEntryDto;
            var existingWordDto = WordDto.FromModel(Get(updatedWordDto.WordId));

            // Update translations
            ApplyEntryTranslationChanges(existingWordDto, updatedWordDto, (RealmTranslationsRepository)translationsRepository);

            // Update word
            Update((WordDto)existingWordDto);
        }
    }
}