using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive;

namespace dosham.ViewModels
{
    public class RegistrationPageViewModel : EditFormViewModelBase
    {
        [Reactive] public string Username { get; set; }
        [Reactive] public string Email { get; set; }
        [Reactive] public string Password { get; set; }
        [Reactive] public string PasswordConfirmation { get; set; }
        public ReactiveCommand<Unit, Unit> RegisterCommand { get; }

        public RegistrationPageViewModel()
        {
            RegisterCommand = ReactiveCommand.CreateFromTask(OnRegister);
        }

        private async Task OnRegister()
        {
            ErrorMessage = "";

            try
            {
                await UserStore.RegisterNewUser(Email!, Password!);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

    }
}
