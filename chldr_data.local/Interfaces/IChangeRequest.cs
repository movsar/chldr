﻿using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;

namespace chldr_data.Interfaces
{
    internal interface IChangeRequest
    {
        Task Add(string userId, WordDto dto);
        Task Update(string userId, WordDto dto);
        Task Delete(string userId, string entityId);
    }
}