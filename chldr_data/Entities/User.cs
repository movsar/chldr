﻿using chldr_data.Enums;
using MongoDB.Bson;
using Realms;
using RequiredAttribute = Realms.RequiredAttribute;

namespace chldr_data.Entities
{
    // Used as additional source of user data coupled with App Users from MongoDB
    public class User : RealmObject
    {
        [PrimaryKey]
        public ObjectId _id { get; set; }
        public string Email { get; set; } = string.Empty;
        public int RateWeight { get; set; } = 1;
        public int Rate { get; set; } = 1;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Patronymic { get; set; } = string.Empty;
        public int Status { get; set; } = (int)UserStatus.Active;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.Now;
    }
}