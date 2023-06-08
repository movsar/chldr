using chldr_data.DatabaseObjects.Dtos;
using chldr_data.Interfaces;

namespace chldr_api
{
    public class Query
    {
        private readonly IDataProvider _dataProvider;
        protected readonly IConfiguration _configuration;

        public Query(
            IDataProvider dataProvider,
            IConfiguration configuration
            )
        {
            _dataProvider = dataProvider;
            _configuration = configuration;
        }

        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<WordDto> GetEntries(int limit)
        {
            var unitOfWork = _dataProvider.CreateUnitOfWork();
            var words = unitOfWork.Words.Take(limit);
            return words.Select(w => WordDto.FromModel(w)).AsQueryable();
        }

        //public IQueryable<ChangeSetDto> RetrieveLatestChangeSets(int index)
        //{
        //    var unitOfWork = _dataProvider.CreateUnitOfWork();
        //    var changeSets = unitOfWork.ChangeSets.GetLatest(index);
        //    return changeSets.Select(c => ChangeSetDto.FromModel(c)).AsQueryable();
        //}

    }
}