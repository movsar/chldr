using System.ComponentModel.DataAnnotations.Schema;

namespace chldr_data.Entities;
[Table("Query")]
public partial class SqlQuery
{
    public string QueryId { get; set; } =  Guid.NewGuid().ToString();
    public string UserId { get; set; } = null!;
    public string Content { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; } = DateTime.Now;
    public DateTimeOffset UpdatedAt { get; set; } = DateTime.Now;
    public virtual SqlUser User { get; set; } = null!;
}
