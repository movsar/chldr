using chldr_application.ApiDataProvider.Interfaces;
using core.DatabaseObjects.Dtos;
using core.DatabaseObjects.Interfaces;
using core.DatabaseObjects.Models;
using core.Interfaces;
using core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_application.ApiDataProvider.Repositories
{
    internal class ApiTranslationsRepository : ApiRepository<TranslationModel, TranslationDto>, ITranslationsRepository
    {
        public ApiTranslationsRepository(IRequestService requestService) : base(requestService) { }

        public override Task Add(TranslationDto dto)
        {
            throw new NotImplementedException();
        }

        public override Task<TranslationModel> GetAsync(string entityId)
        {
            throw new NotImplementedException();
        }

        public override Task<List<TranslationModel>> GetRandomsAsync(int limit)
        {
            throw new NotImplementedException();
        }

        public Task<ChangeSetModel> Promote(ITranslation translation)
        {
            throw new NotImplementedException();
        }

        public override Task Remove(string entityId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveRange(string[] translations)
        {
            throw new NotImplementedException();
        }

        public override Task<List<TranslationModel>> TakeAsync(int offset, int limit)
        {
            throw new NotImplementedException();
        }

        public override Task Update(TranslationDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
