using chldr_data.Interfaces;
using Realms;
using System.ComponentModel.DataAnnotations.Schema;

namespace chldr_data.Entities;
[Table("Query")]
public partial class Query : RealmObject, IEntity
{
    [Realms.PrimaryKey]
    public string QueryId { get; set; } =  Guid.NewGuid().ToString();
    public string UserId { get; set; } = null!;
    public string Content { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public virtual User User { get; set; } = null!;
}
