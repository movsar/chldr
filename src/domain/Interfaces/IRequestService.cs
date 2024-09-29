using domain.DatabaseObjects.Dtos;
using domain;
using domain.Models;
using domain.Enums;
namespace domain.Interfaces
{
    public interface IRequestService
    {
        bool IsNetworUp { get; }

        event Action NetworkStateHasChanged;

        Task<RequestResult> FindAsync(string inputText, FiltrationFlags? filtrationFlags);
        Task<RequestResult> AddEntryAsync(EntryDto entryDto);
        Task<RequestResult> AddSoundAsync(SoundDto pronunciation);
        Task<RequestResult> PromoteAsync(RecordType recordType, string entityId);
        Task<RequestResult> RemoveEntry(string entityId);
        Task<RequestResult> TakeAsync(RecordType recordType, int offset, int limit);
        Task<RequestResult> TakeLastAsync(RecordType recordType, int count);
        Task<RequestResult> UpdateEntry(EntryDto entryDto);
        Task<RequestResult> GetRandomsAsync(int count);
        
        Task<RequestResult> ConfirmUserAsync(string token);
        Task<RequestResult> LogInEmailPasswordAsync(string email, string password);
        Task<RequestResult> PasswordResetRequestAsync(string email);
        Task<RequestResult> RegisterUserAsync(string email, string password);
        Task<RequestResult> RefreshTokens(string accessToken, string refreshToken);
        Task<RequestResult> UpdatePasswordAsync(string email, string token, string newPassword);
        Task<RequestResult> CountAsync(RecordType entry, FiltrationFlags? filtrationFlags);
    }
}