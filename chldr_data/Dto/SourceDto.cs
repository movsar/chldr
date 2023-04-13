using chldr_data.Models;

namespace chldr_data.Dto
{
    public class SourceDto
    {
        public string? SourceId { get; }
        public string Name { get; set; }
        public string Notes { get; set; }

        public SourceDto()
        { }

        public SourceDto(SourceModel source)
        {
            SourceId = source.Id.ToString();
            Name = source.Name;
            Notes = source.Notes;
        }
    }
}
