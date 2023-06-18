using chldr_data.DatabaseObjects.Dtos;
using chldr_data.Interfaces;
using chldr_data.ResponseTypes;
using chldr_utils;
using chldr_utils.Services;
using Microsoft.AspNetCore.Mvc;
using Realms.Sync;

namespace chldr_api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SoundController : ControllerBase
    {
        private readonly IDataProvider _dataProvider;
        private readonly FileService _fileService;
        private readonly ExceptionHandler _exceptionHandler;

        public SoundController(IDataProvider dataProvider, FileService fileService, ExceptionHandler exceptionHandler)
        {
            _dataProvider = dataProvider;
            _fileService = fileService;
            _exceptionHandler = exceptionHandler;
        }

        // POST api/<SoundController>
        [HttpPost]
        public async Task<OperationResult> Post([FromForm] string userId, [FromForm] string entryId, [FromForm] IFormFile file)
        {
            using var unitOfWork = (ISqlUnitOfWork)_dataProvider.CreateUnitOfWork(userId);
            unitOfWork.BeginTransaction();  
            try
            {
                var soundDto = new SoundDto()
                {
                    UserId = userId, 
                    EntryId = entryId
                };

                await _fileService.SaveSoundAsync(file, soundDto.FileName);

                unitOfWork.Sounds.Add(soundDto);
                unitOfWork.Commit();

                return new OperationResult()
                {
                    Success = true,
                };
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                _exceptionHandler.LogError(ex);
                return new OperationResult() { Success = false };
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        // GET: api/<SoundController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<SoundController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }


        // PUT api/<SoundController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<SoundController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
