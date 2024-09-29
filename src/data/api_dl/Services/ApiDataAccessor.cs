using api_dl.ApiDataProvider.Repositories;
using domain.Interfaces;
using domain.Interfaces.Repositories;
using domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api_dl.ApiDataProvider.Services
{
    internal class ApiDataAccessor : IDataAccessor, IDisposable
    {
        private ApiTranslationsRepository _translationsRepository;
        private ApiEntriesRepository _entriesRepository;
        private ApiUsersRepository _usersRepository;
        private ApiChangeSetsRepository _changeSetsRepository;
        private ApiSoundsRepository _soundsRepository;
        private ApiSourcesRepository _sourcesRepository;
        private readonly IRequestService _requestService;

        public ISoundsRepository Sounds => _soundsRepository ??= new ApiSoundsRepository(_requestService);
        public IChangeSetsRepository ChangeSets => _changeSetsRepository ??= new ApiChangeSetsRepository(_requestService);
        public IEntriesRepository Entries => _entriesRepository ??= new ApiEntriesRepository(_requestService);
        public ITranslationsRepository Translations => _translationsRepository ??= new ApiTranslationsRepository(_requestService);
        public ISourcesRepository Sources => _sourcesRepository ??= new ApiSourcesRepository(_requestService);
        public IUsersRepository Users => _usersRepository ??= new ApiUsersRepository(_requestService);

        public ApiDataAccessor(IRequestService requestService)
        {
            _requestService = requestService;
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
