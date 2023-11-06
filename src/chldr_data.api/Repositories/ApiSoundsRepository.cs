using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_data.Interfaces.Repositories;
using chldr_data.Models;
using chldr_data.Repositories;
using chldr_data.Services;
using chldr_utils;
using chldr_utils.Services;

namespace chldr_data.realm.Repositories
{
    public class ApiSoundsRepository : ApiRepository< PronunciationModel, PronunciationDto>, IPronunciationsRepository
    {
        public ApiSoundsRepository(
                ExceptionHandler exceptionHandler,
                FileService fileService,
                RequestService requestService
            ) : base(exceptionHandler, fileService, requestService) { }
        protected override RecordType RecordType => RecordType.Sound;
   
        public override async Task<List<ChangeSetModel>> Add(PronunciationDto dto)
        {
            throw new NotImplementedException();

        }
        public override async Task<List<ChangeSetModel>> Update(PronunciationDto dto)
        {
            throw new NotImplementedException();

        }
        public override async Task<List<ChangeSetModel>> Remove(string entityId)
        {
            throw new NotImplementedException();

        }

        public Task<ChangeSetModel> Promote(IPronunciation sound)
        {
            throw new NotImplementedException();
        }
    }
}