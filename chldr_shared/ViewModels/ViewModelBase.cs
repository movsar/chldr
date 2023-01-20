using chldr_shared.Stores;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_shared.ViewModels
{
    public class ViewModelBase : ComponentBase
    {
        [Inject] protected ContentStore MyContentStore { get; set; }
        [Inject] protected UserStore MyUserStore { get; set; }
    }
}
