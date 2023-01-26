using chldr_data.Models;
using chldr_shared.Enums;
using chldr_shared.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_ui.ViewModels
{
    public class SearchResultsViewModel : ViewModelBase
    {
        #region Properties
        [Parameter]
        public List<SearchResultModel> SearchResults { get; set; }
        #endregion

        protected override Task OnParametersSetAsync()
        {
            StateHasChanged();
            return base.OnParametersSetAsync();
        }

    }
}
