using chldr_data.DatabaseObjects.Dtos;
using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.ResponseTypes;
using Newtonsoft.Json;

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

        public RequestResult Take(string recordTypeName, int offset, int limit)
        {

            var unitOfWork = _dataProvider.CreateUnitOfWork();
            object? dtos = null;


            var recordType = (RecordType)Enum.Parse(typeof(RecordType), recordTypeName);
            switch (recordType)
            {
                case RecordType.Entry:
                    dtos = unitOfWork.Entries.Take(offset, limit).Select(EntryDto.FromModel);
                    break;

                case RecordType.User:
                    dtos = unitOfWork.Users.Take(offset, limit).Select(UserDto.FromModel);
                    break;

                case RecordType.Source:
                    dtos = unitOfWork.Sources.Take(offset, limit).Select(SourceDto.FromModel);
                    break;

                case RecordType.Sound:
                    break;

                case RecordType.Translation:
                    dtos = unitOfWork.Translations.Take(offset, limit).Select(TranslationDto.FromModel);
                    break;

                default:
                    break;
            }

            return new RequestResult()
            {
                Success = true,
                SerializedData = JsonConvert.SerializeObject(dtos)
            };
        }

        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<EntryDto> GetEntries(int limit)
        {
            var unitOfWork = _dataProvider.CreateUnitOfWork();
            var words = unitOfWork.Entries.Take(limit);
            return words.Select(w => EntryDto.FromModel(w)).AsQueryable();
        }

        public IQueryable<ChangeSetDto> RetrieveLatestChangeSets(int minIndex)
        {
            var unitOfWork = _dataProvider.CreateUnitOfWork();
            var changeSets = unitOfWork.ChangeSets.GetLatest(minIndex);
            return changeSets.Select(c => ChangeSetDto.FromModel(c)).AsQueryable();
        }

    }
}