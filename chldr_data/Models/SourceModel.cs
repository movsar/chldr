using chldr_data.Entities;
using chldr_data.Interfaces;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_data.Models
{
    public class SourceModel
    {
        public ObjectId SourceId { get; }
        public string Name { get; }
        public string Notes { get; }
        public SourceModel(Source source)
        {
            Name = source.Name;
            SourceId = source._id;
            Notes = source.Notes;
        }
    }
}