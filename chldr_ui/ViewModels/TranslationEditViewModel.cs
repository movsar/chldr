using chldr_data.Models;
using chldr_shared.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_ui.ViewModels
{
    public class TranslationEditViewModel : EditFormViewModelBase<TranslationModel, TranslationValidator>
    {
        public override Task ValidateAndSubmit()
        {
            throw new NotImplementedException();
        }
    }
}
