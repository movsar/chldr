﻿using chldr_data.Entities;
using System.Threading.Tasks;

namespace chldr_data.Interfaces.DatabaseEntities
{
    public interface ILanguageEntity : ILanguage
    {
        string? UserId { get; }
        DateTimeOffset CreatedAt { get; set; }
        DateTimeOffset UpdatedAt { get; set; }
    }
}
