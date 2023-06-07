using chldr_data.Interfaces;

namespace chldr_data.provider
{
    public class ActiveDataProvider
    {
        private readonly IDataProvider _dataProvider;
        public ActiveDataProvider(IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public IUnitOfWork CreateUnitOfWork()
        {
            return _dataProvider.CreateUnitOfWork();
        }
    }
}