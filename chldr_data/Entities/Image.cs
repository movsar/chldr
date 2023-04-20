using chldr_data.Interfaces;
using Realms;

namespace chldr_data.Entities;
public  class Image : RealmObject, IEntity
{
    [PrimaryKey]
    public string ImageId { get; set; } = Guid.NewGuid().ToString();
    public User? User { get; set; }
    public Entry Entry { get; set; } = null!;
    public string? FileName { get; set; }
    public int Rate { get; set; }
    public DateTimeOffset? CreatedAt { get; set; } 
    public DateTimeOffset? UpdatedAt { get; set; }
}
