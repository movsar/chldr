using chldr_data.DatabaseObjects.Dtos;
using chldr_data.Enums;
using chldr_data.Models;
namespace chldr_data.Interfaces
{
    public interface IRequestService
    {
        bool IsNetworUp { get; }

        event Action NetworkStateHasChanged;

        Task<RequestResult> AddEntryAsync(EntryDto entryDto);
        Task<RequestResult> AddSoundAsync(PronunciationDto pronunciation);
        Task<RequestResult> ConfirmUserAsync(string token);
        Task<RequestResult> GetRandomsAsync(int count);
        Task<RequestResult> LogInEmailPasswordAsync(string email, string password);
        Task<RequestResult> PasswordResetRequestAsync(string email);
        Task<RequestResult> PromoteAsync(RecordType recordType, string entityId);
        Task<RequestResult> RefreshTokens(string accessToken, string refreshToken);
        Task<RequestResult> RegisterUserAsync(string email, string password);
        Task<RequestResult> RemoveEntry(string entityId);
        Task<RequestResult> TakeAsync(RecordType recordType, int offset, int limit);
        Task<RequestResult> TakeLastAsync(RecordType recordType, int count);
        Task<RequestResult> UpdateEntry(EntryDto entryDto);
        Task<RequestResult> UpdatePasswordAsync(string email, string token, string newPassword);
    }
}