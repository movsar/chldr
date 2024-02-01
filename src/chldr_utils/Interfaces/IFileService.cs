using Microsoft.AspNetCore.Http;

namespace chldr_utils.Interfaces
{
    public interface IFileService
    {
        string AlphabetSoundsDirectory { get; }
        string AppDataDirectory { get; }
        string DataArchiveFilePath { get; }
        string DatabaseFilePath { get; }
        string EntrySoundsDirectory { get; }

        void AddEntrySound(string fileName, string b64data);
        void DeleteEntrySound(string fileName);
        Task SaveSoundAsync(IFormFile contents, string fileName);
    }
}
