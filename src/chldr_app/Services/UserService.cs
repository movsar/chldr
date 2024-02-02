using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_data.Services;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace chldr_app.Services
{
    public class UserService : IDboService<UserModel, UserDto>
    {
        private readonly IDataProvider _dataProvider;
        private readonly IRequestService _requestService;
        private readonly IExceptionHandler _exceptionHandler;

        public event Action<SessionInformation>? UserStateHasChanged;

        private readonly ISettingsService _localStorageService;
        public SessionInformation CurrentSession = new SessionInformation();

        public UserService(
            IDataProvider dataProvider, 
            IRequestService requestService,
            ISettingsService localStorageService)
        {
            _requestService = requestService;
            _localStorageService = localStorageService;
            _dataProvider = dataProvider;
            _dataProvider.DatabaseInitialized += RealmService_DatasourceInitialized;
        }
        private void RealmService_DatasourceInitialized()
        {
            // If there are things to be done after local database is initialized, do them here
        }

        public async Task RegisterNewUserAsync(string email, string password)
        {
            var response = await _requestService.RegisterUserAsync(email, password);
            if (!response.Success)
            {
                throw new Exception(response.ErrorMessage);
            }

            var data = RequestResult.GetData<dynamic>(response);

            CurrentSession = new SessionInformation()
            {
                AccessToken = data.AccessToken!,
                RefreshToken = data.RefreshToken!,
                Status = SessionStatus.LoggedIn,
                UserDto = JsonConvert.DeserializeObject<UserDto>(data.User.ToString())
            };

            await SaveActiveSession();

            UserStateHasChanged?.Invoke(CurrentSession);
        }

        public async Task SendPasswordResetRequestAsync(string email)
        {
            await _requestService.PasswordResetRequestAsync(email);
        }

        public async Task UpdatePasswordAsync(string email, string token, string newPassword)
        {
            await _requestService.UpdatePasswordAsync(email, token, newPassword);
        }

        public async Task ConfirmUserAsync(string token)
        {
            await _requestService.ConfirmUserAsync(token);
        }
        private async Task SaveActiveSession()
        {
            await _localStorageService.SetItem<SessionInformation>("session", CurrentSession);
        }

        public async Task LogInEmailPasswordAsync(string email, string password)
        {
            var response = await _requestService.LogInEmailPasswordAsync(email, password);
            if (!response.Success)
            {
                throw new Exception(response.ErrorMessage);
            }

            var data = RequestResult.GetData<dynamic>(response);

            CurrentSession = new SessionInformation()
            {
                AccessToken = data.AccessToken!,
                RefreshToken = data.RefreshToken!,
                Status = SessionStatus.LoggedIn,
                UserDto = JsonConvert.DeserializeObject<UserDto>(data.User.ToString())
            };

            await SaveActiveSession();
            UserStateHasChanged?.Invoke(CurrentSession);
        }

        public async Task LogOutAsync()
        {
            CurrentSession.Clear();

            await SaveActiveSession();

            UserStateHasChanged?.Invoke(CurrentSession);
        }

        public async Task<SessionInformation> RefreshTokens(string accessToken, string refreshToken)
        {
            var response = await _requestService.RefreshTokens(accessToken, refreshToken);
            if (!response.Success)
            {
                return new SessionInformation();
            }

            var data = RequestResult.GetData<dynamic>(response);

            var session = new SessionInformation()
            {
                AccessToken = data.AccessToken!,
                RefreshToken = data.RefreshToken!,
                Status = SessionStatus.LoggedIn,
                UserDto = JsonConvert.DeserializeObject<UserDto>(data.User.ToString())
            };

            return session;
        }
        public bool IsTokenExpired(string accessToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(accessToken);

            var expirationClaim = jwtToken.Claims.First(claim => claim.Type == "exp");
            if (expirationClaim == null)
            {
                throw new InvalidOperationException("The token does not have an expiration claim.");
            }

            var expirationTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expirationClaim.Value));
            return expirationTime.UtcDateTime <= DateTime.UtcNow;
        }
        public async Task RestoreLastSession()
        {
            // Get last session info from the local storage
            var session = await _localStorageService.GetItem<SessionInformation>("session");
            if (session == null || string.IsNullOrEmpty(session.AccessToken))
            {
                return;
            }

            CurrentSession = session;
            UserStateHasChanged?.Invoke(CurrentSession);

            CurrentSession = await RefreshTokens(CurrentSession.AccessToken, CurrentSession.RefreshToken);
            await SaveActiveSession();
            UserStateHasChanged?.Invoke(CurrentSession);
        }

        public Task AddAsync(UserDto entryDto, string userId)
        {
            throw new NotImplementedException();
        }

        public Task<UserModel> GetAsync(string entryId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(UserModel entry, string userId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(UserDto entryDto, string userId)
        {
            throw new NotImplementedException();
        }
    }
}
