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
    internal class ApiChangeSetsRepository : ApiRepository<ChangeSetModel, ChangeSetDto>, IChangeSetsRepository
    {
        public ApiChangeSetsRepository(IRequestService requestService) : base(requestService)
        {
        }

        public override Task Add(ChangeSetDto dto)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ChangeSetModel> Get(string[] changeSetIds)
        {
            throw new NotImplementedException();
        }

        public override Task<ChangeSetModel> GetAsync(string entityId)
        {
            throw new NotImplementedException();
        }

        public override Task<List<ChangeSetModel>> GetRandomsAsync(int limit)
        {
            throw new NotImplementedException();
        }

        public override Task Remove(string entityId)
        {
            throw new NotImplementedException();
        }

        public override Task<List<ChangeSetModel>> TakeAsync(int offset, int limit)
        {
            throw new NotImplementedException();
        }

        public Task<List<ChangeSetModel>> TakeLastAsync(int count)
        {
            throw new NotImplementedException();
        }

        public override Task Update(ChangeSetDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
