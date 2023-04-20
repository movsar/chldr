using chldr_data.Interfaces;
using Realms;
using System.ComponentModel.DataAnnotations.Schema;

namespace chldr_data.Entities;
[MapTo("Query")]
public class RealmQuery : RealmObject, IEntity
{
    [PrimaryKey]
    public string QueryId { get; set; } =  Guid.NewGuid().ToString();
    public RealmUser User { get; set; } = null!;
    public string Content { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; } = DateTime.Now;
    public DateTimeOffset UpdatedAt { get; set; } = DateTime.Now;
}
