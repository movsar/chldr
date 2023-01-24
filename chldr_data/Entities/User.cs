using chldr_data.Enums;
using MongoDB.Bson;
using Realms;
using RequiredAttribute = Realms.RequiredAttribute;

namespace chldr_data.Entities
{
    // Used as additional source of user data coupled with App xUsers from MongoDB
    public class User : RealmObject
    {
        [PrimaryKey]
        public ObjectId _id { get; set; }
        public string? Email { get; set; }
        public int RateWeight { get; set; } = 1;
        public int Rate { get; set; } = 1;
        public string? Username { get; set; }
        public string? ImagePath { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Patronymic { get; set; }
        public int Status { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.Now;
    }
}