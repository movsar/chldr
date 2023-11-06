using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using Realms;
using chldr_data.Interfaces.Repositories;
using chldr_utils.Interfaces;
using chldr_utils;
using chldr_utils.Services;
using chldr_data.Models;
using System.Threading.Channels;
using chldr_data.Services;
using chldr_data.DatabaseObjects.Interfaces;

namespace chldr_data.Repositories
{
    public class ApiTranslationsRepository : ApiRepository< TranslationModel, TranslationDto>, ITranslationsRepository
    {
        public ApiTranslationsRepository(
            ExceptionHandler exceptionHandler, 
            FileService fileService,
            RequestService requestService) : base(exceptionHandler, fileService, requestService) { }

        protected override RecordType RecordType => RecordType.Translation;
    
        public override async Task<List<ChangeSetModel>> Add(TranslationDto dto)
        {
            throw new NotImplementedException();

        }
        public override async Task<List<ChangeSetModel>> Update(TranslationDto dto)
        {
            throw new NotImplementedException();

        }

        public override Task<List<ChangeSetModel>> Remove(string entityId)
        {
            throw new NotImplementedException();
        }

        public Task<ChangeSetModel> Promote(ITranslation translation)
        {
            throw new NotImplementedException();
        }
    }
}
