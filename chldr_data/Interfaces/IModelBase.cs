﻿using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_data.Interfaces
{
    public abstract class IModelBase
    {
        public ObjectId Id { get; }
    }
}
