using api_dl.ApiDataProvider.Interfaces;
using domain.DatabaseObjects.Models;
using domain.Interfaces;
using domain.Responses;
using domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api_dl.ApiDataProvider.Repositories
{
    internal abstract class ApiRepository<TModel, TDto> : IApiRepository<TModel, TDto>
    {
        protected readonly IRequestService _requestService;

        public ApiRepository(IRequestService requestService) {
            _requestService = requestService;
        }

        public abstract Task<List<TModel>> TakeAsync(int offset, int limit);
        public abstract Task<List<TModel>> GetRandomsAsync(int limit);
        public abstract Task<TModel> GetAsync(string entityId);

        public abstract Task Add(TDto dto);
        public abstract Task Update(TDto dto);
        public abstract Task Remove(string entityId);

        public async Task AddRange(IEnumerable<TDto> added)
        {
            foreach (var dto in added)
            {
                await Add(dto);
            }
        }
        public async Task UpdateRange(IEnumerable<TDto> updated)
        {
            foreach (var dto in updated)
            {
                await Update(dto);

            }
        }
        public async Task RemoveRange(IEnumerable<string> removed)
        {
            foreach (var id in removed)
            {
                await Remove(id);
            }
        }
    }
}
