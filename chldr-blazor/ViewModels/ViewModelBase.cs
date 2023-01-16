using CommunityToolkit.Mvvm.ComponentModel;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_blazor.ViewModels
{
    [ObservableObject]
    public partial class ViewModelBase
    {
        protected void NotifyOfChanges()
        {
            PropertyChanged?.Invoke(null, null);
        }

        // Used for viewmodels that need parameters from razor pages
        public virtual void OnInitialized(ObjectId modelId) { }
    }
}
