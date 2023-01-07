using Data.Entities;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class SourceModel
    {
        public ObjectId Id { get; }
        public string Name { get; }
        public string Notes { get; }
        public SourceModel(SourceEntity source)
        {
            Name = source.Name;
            Id = source._id;
            Notes = source.Notes;
        }
    }
}