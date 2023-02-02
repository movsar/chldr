using chldr_data.Enums;
using chldr_data.Interfaces;
using MongoDB.Bson;
using Realms;
using RequiredAttribute = Realms.RequiredAttribute;

namespace chldr_data.Entities
{
    // Used as additional source of user data coupled with App xUsers from MongoDB
    public class User : RealmObject, IEntity
    {
        [PrimaryKey]
        public ObjectId _id { get; set; }
        public string? Email { get; set; }
        // This has to be remoed and shouldn't be used
        public int RateWeight { get; set; } = 1;
        public int Rate { get; set; } = 1;
        public string? ImagePath { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Patronymic { get; set; }
        private bool? IsModerator { get; set; }
        public int Status { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.Now;
    }
}