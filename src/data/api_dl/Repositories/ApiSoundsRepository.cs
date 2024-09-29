using api_dl.ApiDataProvider.Interfaces;
using domain.DatabaseObjects.Dtos;
using domain.DatabaseObjects.Interfaces;
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
    internal class ApiSoundsRepository : ApiRepository<SoundModel, SoundDto>, ISoundsRepository
    {
        public ApiSoundsRepository(IRequestService requestService) : base(requestService) { }

        public override Task Add(SoundDto dto)
        {
            throw new NotImplementedException();
        }

        public override Task<SoundModel> GetAsync(string entityId)
        {
            throw new NotImplementedException();
        }

        public override Task<List<SoundModel>> GetRandomsAsync(int limit)
        {
            throw new NotImplementedException();
        }

        public Task<ChangeSetModel> Promote(ISound sound)
        {
            throw new NotImplementedException();
        }

        public override Task Remove(string entityId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveRange(string[] sounds)
        {
            throw new NotImplementedException();
        }

        public override Task<List<SoundModel>> TakeAsync(int offset, int limit)
        {
            throw new NotImplementedException();
        }

        public override Task Update(SoundDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
