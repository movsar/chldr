using Data.Entities;
using Data.Interfaces;
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
        public ObjectId EntityId { get; }
        public string Name { get; }
        public string Notes { get; }
        public SourceModel(Source source)
        {
            Name = source.Name;
            EntityId = source._id;
            Notes = source.Notes;
        }
    }
}