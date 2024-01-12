using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive;

namespace dosham.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        [Reactive] public string Email { get; set; }
        [Reactive] public string Password { get; set; }
        public ReactiveCommand<Unit, Unit> LoginCommand { get; }

        public LoginPageViewModel()
        {            
            LoginCommand = ReactiveCommand.CreateFromTask(OnLogin);
        }

        private async Task OnLogin()
        {

        }

    }
}
