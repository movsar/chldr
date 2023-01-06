using MongoDB.Bson;
using Realms;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
namespace Data.Entities
{
    [MapTo("User")]
    public class UserEntity: RealmObject
    {
        [Key]
        [PrimaryKey]

        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
        public string ThirdParty { get; set; } = string.Empty; 
        public string ThirdPartyId { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool Confirmed { get; set; }
        public string Password { get; set; } = string.Empty;
        public IList<string> Tokens { get; } = new List<string>();
        public string PointsPerLang { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Patronymic { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; } = DateTimeOffset.Now;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.Now;
    }
}