using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_ui.Dto
{
    public class UserInfoDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordConfirmation { get; set; }
        public int RateWeight { get; set; }
        public int Rate { get; set; }
        public string Patronymic { get; set; } 
        public string FirstName { get; set; } 
        public string LastName { get; set; } 
    }
}
