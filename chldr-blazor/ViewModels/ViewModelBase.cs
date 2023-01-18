using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.AspNetCore.Components;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_native.ViewModels
{
    [ObservableObject]
    public partial class ViewModelBase : ComponentBase
    {
        protected void NotifyOfChanges()
        {
            PropertyChanged?.Invoke(null, null);
        }
    }
}
