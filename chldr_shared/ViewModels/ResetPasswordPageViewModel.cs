using Microsoft.AspNetCore.Components;
using Realms.Sync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_shared.ViewModels
{
    public class ResetPasswordPageViewModel : ViewModelBase
    {
        public string? Email { get; set; }
        public async void SendPasswordResetRequest()
        {
            await MyUserStore.SendPasswordResetRequestAsync(Email);
        }
    }
}
