﻿using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_data.Interfaces
{
    public interface IRealmService
    {
        Realm GetDatabase();
        void InitializeConfiguration();

    }
}