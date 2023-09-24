using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace chldr_data.remote.SqlEntities;
[Table("Query")]
public class SqlQuery
{
    [Key]
    public string QueryId { get; set; } 
    public string UserId { get; set; } = null!;
    public string Content { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; } = DateTime.Now;
    public DateTimeOffset UpdatedAt { get; set; } = DateTime.Now;
    public virtual SqlUser User { get; set; } = null!;
}
