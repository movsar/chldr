using api_dl.ApiDataProvider.Interfaces;
using domain.DatabaseObjects.Dtos;
using domain.DatabaseObjects.Models;
using domain.Interfaces;
using domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api_dl.ApiDataProvider.Repositories
{
    internal class ApiSourcesRepository : ApiRepository<SourceModel, SourceDto>, ISourcesRepository
    {
        public ApiSourcesRepository(IRequestService requestService) : base(requestService)
        {
        }

        public override Task Add(SourceDto dto)
        {
            throw new NotImplementedException();
        }

        public override Task<SourceModel> GetAsync(string entityId)
        {
            throw new NotImplementedException();
        }

        public override Task<List<SourceModel>> GetRandomsAsync(int limit)
        {
            throw new NotImplementedException();
        }

        public override Task Remove(string entityId)
        {
            throw new NotImplementedException();
        }

        public override Task<List<SourceModel>> TakeAsync(int offset, int limit)
        {
            throw new NotImplementedException();
        }

        public override Task Update(SourceDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
