using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    internal class UserModel
    {
        public ObjectId EntityId { get; }
        public string Email { get; }
        public string Username { get; }
        public int RateWeight { get; }
        public int Rate { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string Patronymic { get; }
        public DateTimeOffset CreatedAt { get; }
        public DateTimeOffset UpdatedAt { get; }
    }
}
