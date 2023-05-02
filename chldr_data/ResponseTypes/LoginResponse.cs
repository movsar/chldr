using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_data.ResponseTypes
{
    public class LoginResponse : MutationResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public DateTimeOffset ExpiresIn { get; set; }
    }
}
