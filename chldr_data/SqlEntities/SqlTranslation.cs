﻿using chldr_data.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace chldr_data.Entities;

[Table("Translation")]

public partial class SqlTranslation : ITranslation
{
    public string TranslationId { get; set; } = Guid.NewGuid().ToString();
    public string LanguageId { get; set; } = null!;
    public string EntryId { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string RawContents { get; set; } = null!;
    public string? Notes { get; set; }
    public int Rate { get; set; } = 0;
    public virtual SqlEntry Entry { get; set; } = null!;
    public virtual SqlLanguage Language { get; set; } = null!;
    public virtual SqlUser User { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; } = DateTime.Now;
    public DateTimeOffset UpdatedAt { get; set; } = DateTime.Now;
    internal string GetRawContents()
    {
        return Content.ToString();
    }

    public SqlTranslation()
    {

    }

    public SqlTranslation(ITranslation translation)
    {
        TranslationId = translation.TranslationId;
        LanguageId = translation.LanguageId;
        EntryId = translation.EntryId;
        UserId = translation.UserId;
        Content = translation.Content;
        Rate = translation.Rate;
        Notes = translation.Notes;
    }

}
