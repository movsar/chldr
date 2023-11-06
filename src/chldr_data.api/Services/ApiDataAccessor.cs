using chldr_data.Interfaces;
using chldr_data.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_data.api.Services
{
    internal class ApiDataAccessor : IDataAccessor
    {
        public IPronunciationsRepository Sounds => throw new NotImplementedException();

        public IChangeSetsRepository ChangeSets => throw new NotImplementedException();

        public IEntriesRepository Entries => throw new NotImplementedException();

        public ITranslationsRepository Translations => throw new NotImplementedException();

        public ISourcesRepository Sources => throw new NotImplementedException();

        public IUsersRepository Users => throw new NotImplementedException();
    }
}
