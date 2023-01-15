using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public abstract class TargetModel
    {
        public ObjectId TargetId { get; set; }
    }
}
