﻿namespace chldr_data.DatabaseObjects.Interfaces
{
    public interface ILanguage : IEntity
    {
        string Code { get; set; }
        string LanguageId { get; }
        string Name { get; set; }
    }
}