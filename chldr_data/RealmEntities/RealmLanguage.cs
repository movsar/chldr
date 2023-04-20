using chldr_data.Interfaces;
using Realms;
using System;
using System.Collections.Generic;

namespace chldr_data.Entities;
public  class RealmLanguage : RealmObject, IEntity
{
    [PrimaryKey]
    public string LanguageId { get; set; } = Guid.NewGuid().ToString();
    public RealmUser? User { get; set; }
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; } = DateTime.Now;
    public DateTimeOffset UpdatedAt { get; set; } = DateTime.Now;
    public IList<RealmTranslation> Translations { get; } 
}
