using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_data.Services;
using chldr_utils;
using chldr_utils.Services;
using Newtonsoft.Json;

namespace chldr_shared.Services
{
    public class UserService : IDboService<UserModel, UserDto>
    {
        private readonly IDataProvider _dataProvider;
        private readonly RequestService _requestService;
        private readonly ExceptionHandler _exceptionHandler;

        public event Action<SessionInformation>? UserStateHasChanged;

        private readonly LocalStorageService _localStorageService;
        public SessionInformation CurrentSession = new SessionInformation();

        public UserService(IDataProvider dataProvider, RequestService requestService, LocalStorageService localStorageService)
        {
            _requestService = requestService;
            _dataProvider = dataProvider;
            _localStorageService = localStorageService;

            _dataProvider.DatabaseInitialized += RealmService_DatasourceInitialized;
        }
        private void RealmService_DatasourceInitialized()
        {
            // If there are things to be done after local database is initialized, do them here
        }

        public async Task<string> RegisterNewUserAsync(string email, string password)
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
                //AccessTokenExpiresIn = data.AccessTokenExpiresIn!,
                Status = SessionStatus.LoggedIn,
                UserDto = JsonConvert.DeserializeObject<UserDto>(data.User.ToString())
            };

            await SaveActiveSession();

            UserStateHasChanged?.Invoke(CurrentSession);

            // TODO: Improve this
            return "";
        }

        public async Task SendPasswordResetRequestAsync(string email)
        {
            await _requestService.PasswordResetRequestAsync(email);
        }

        public async Task UpdatePasswordAsync(string token, string newPassword)
        {
            await _requestService.UpdatePasswordAsync(token, newPassword);
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
            try
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
                    //AccessTokenExpiresIn = data.AccessTokenExpiresIn!,
                    Status = SessionStatus.LoggedIn,
                    UserDto = JsonConvert.DeserializeObject<UserDto>(data.User.ToString())
                };

                await SaveActiveSession();

                UserStateHasChanged?.Invoke(CurrentSession);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task LogOutAsync()
        {
            CurrentSession.Clear();

            await SaveActiveSession();

            UserStateHasChanged?.Invoke(CurrentSession);
        }

        public async Task<SessionInformation> RefreshTokens(string refreshToken)
        {
            var response = await _requestService.RefreshTokens(refreshToken);

            if (!response.Success)
            {
                return new SessionInformation();
            }

            var data = RequestResult.GetData<dynamic>(response);

            return new SessionInformation()
            {
                AccessToken = data.AccessToken!,
                RefreshToken = data.RefreshToken!,
                //AccessTokenExpiresIn = data.AccessTokenExpiresIn!,
                Status = SessionStatus.LoggedIn,
                UserDto = JsonConvert.DeserializeObject<UserDto>(data.User.ToString())
            };
        }

        public async Task RestoreLastSession()
        {
            // Get last session info from the local storage
            var session = await _localStorageService.GetItem<SessionInformation>("session");
            if (session != null)
            {
                CurrentSession = session;
                UserStateHasChanged?.Invoke(CurrentSession);
            }

            //var expired = DateTimeOffset.UtcNow > CurrentSession.AccessTokenExpiresIn;
            //if (expired && !string.IsNullOrWhiteSpace(CurrentSession.RefreshToken))
            //{
            //    // Try to refresh Access Token
            //    CurrentSession = await RefreshTokens(CurrentSession.RefreshToken);
            //    await SaveActiveSession();
            //    UserStateHasChanged?.Invoke(CurrentSession);
            //}
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
