using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_shared.Models
{
    public class SimpleValidationResult
    {
        public bool Success { get; set; } = false;
        public List<string> Errors { get; set; } = new List<string>();
    }
}