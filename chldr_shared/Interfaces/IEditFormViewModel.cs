
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_shared.Interfaces
{
    public interface IEditFormViewModel<TFormDto, TFormValidator>
    {
        bool FormDisabled { get; set; }
        bool FormSubmitted { get; set; }
        Task ValidateAndSubmit(TFormDto formDto, string[] validationRuleSets, Func<Task<bool>> func);
    }
}
